using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Solitaire : MonoBehaviour
{
    public Sprite[] cardFaces;
    public GameObject cardPrefab;
    public GameObject deckButton;
    public GameObject[] bottomPos;
    public GameObject[] topPos;

    public static string[] suite = new string[] { "C", "D", "H", "S" };
    public static string[] values = new string[] {"A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    public List<string>[] bottoms;
    public List<string>[] tops;
    public List<string> tripsOnDisplay = new List<string>();
    public List<List<string>> deckTrips = new List<List<string>>();

    private List<string> bottom0 = new List<string>();
    private List<string> bottom1 = new List<string>();
    private List<string> bottom2 = new List<string>();
    private List<string> bottom3 = new List<string>();
    private List<string> bottom4 = new List<string>();
    private List<string> bottom5 = new List<string>();
    private List<string> bottom6 = new List<string>();

    public List<string> deck;
    public List<string> discardPile = new List<string>();
    private int deckLocation;
    private int trips;
    private int tripsRemainder;

    void Start()
    {
        bottoms = new List<string>[] { bottom0, bottom1, bottom2, bottom3, bottom4, bottom5, bottom6 };
        PlayCards();
    }

    public void PlayCards()
    {
        deck = GenerateDeck();
        Shuffle(deck);
        SolitaireSort();
        StartCoroutine(SolitaireDeal());
        SortDeckIntoTrips();
    } 

    // 덱 생성
    public static List<string> GenerateDeck()
    {
        List<string> newDeck = new List<string>();
        foreach (string s in suite)
        {
            foreach (string v in values)
            {
                newDeck.Add(s + v);
            }
        }
         
        return newDeck;
    }

    // 덱 카드 셔플
    void Shuffle<T>(List<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;
        while(n>1)
        {
            int k = random.Next(n);
            n--;
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }

    // 카드 52장에 대한 게임오브젝트 생성
    IEnumerator SolitaireDeal()
    {
        for (int i = 0; i < 7; i++)
        {
            float yOffset = 0;
            float zOffset = 0.03f;
            foreach (string card in bottoms[i])
            {
                yield return new WaitForSeconds(0.01f);
                GameObject newCard = Instantiate(cardPrefab, new Vector3(bottomPos[i].transform.position.x, bottomPos[i].transform.position.y - yOffset, bottomPos[i].transform.position.z - zOffset), Quaternion.identity, bottomPos[i].transform);
                newCard.name = card;
                newCard.GetComponent<Selectable>().row = i;
                if(card == bottoms[i][bottoms[i].Count-1])
                {
                    newCard.GetComponent<Selectable>().faceUp = true;
                }
                
                yOffset += 0.3f;
                zOffset += 0.03f;
                discardPile.Add(card);
            }
        }

        foreach(string card in discardPile)
        {
            if(deck.Contains(card))
            {
                deck.Remove(card);
            }
        }
        discardPile.Clear();
    }

    void SolitaireSort()
    {
        for (int i=0;i<7;i++)
        {
            for(int j=i;j<7;j++)
            {
                bottoms[j].Add(deck.Last<string>());
                deck.RemoveAt(deck.Count - 1);
            }
        }
    }

    public void SortDeckIntoTrips()
    {
        
        trips = deck.Count / 3;
        tripsRemainder = deck.Count % 3;
        deckTrips.Clear();

        // 덱의 카드를 3장씩 분할하여 deckTrips에 저장
        int modifier = 0;
        for(int i=0;i<trips;i++)
        {
            List<string> myTrips = new List<string>();
            for(int j=0;j<3;j++)
            {
                myTrips.Add(deck[j + modifier]);
            }
            deckTrips.Add(myTrips);
            modifier += 3;
        }

        // 3장 이하로 카드가 남았을때 deckTrips에 저장
        if (tripsRemainder != 0)
        {
            List<string> myRemainders = new List<string>();
            modifier = 0;
            for (int k = 0; k < tripsRemainder; k++)
            {
                myRemainders.Add(deck[deck.Count - tripsRemainder + modifier]);
                modifier++;
            }
            deckTrips.Add(myRemainders);
            trips++;
        }
        deckLocation = 0;
    }

    public void DealFromDeck()
    {
        // 오픈되어 있는 카드가 있다면 오픈된 카드 3장을 deck에서 제거하고 discardPiledp 추가하고 gameObject 제거
        foreach (Transform child in deckButton.transform)
        {
            if (child.CompareTag("Card"))
            {
                deck.Remove(child.name);
                discardPile.Add(child.name);
                Destroy(child.gameObject);
            }
        }

        // 카드 3장 드로우
        if (deckLocation < trips)
        {
            tripsOnDisplay.Clear(); // 오픈된 카드 제거
            float xOffset = 2.5f;
            float zOffset = -0.2f;

            // deckTrips에 저장되어 있는 카드 tripsOnDisplay로 옴기기
            foreach (string card in deckTrips[deckLocation])
            {
                GameObject newTopCard = Instantiate(cardPrefab, new Vector3(deckButton.transform.position.x + xOffset, deckButton.transform.position.y, deckButton.transform.position.z + zOffset), Quaternion.identity, deckButton.transform);
                xOffset += 0.5f;
                zOffset -= 0.2f;
                newTopCard.name = card;
                tripsOnDisplay.Add(card);
                newTopCard.GetComponent<Selectable>().faceUp = true;
                newTopCard.GetComponent<Selectable>().inDeckPile = true;
            }
            deckLocation++;
        }
        else
        {
            RestackTopDeck();
        }
    }

    void RestackTopDeck()
    {
        deck.Clear();
        // discardPile에 저장된 카드 deck으로 옴기기
        foreach (string card in discardPile)
        {
            deck.Add(card);
        }
        discardPile.Clear();
        SortDeckIntoTrips();
    }
}
