using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UserInput : MonoBehaviour
{
    public GameObject slot1;  // 첫번째 선택한 카드 데이터
    private Solitaire solitaire;
    private float timer;
    private float doubleClickTime = 0.3f;
    private int clickCount = 0;

    void Start()
    {
        solitaire = FindObjectOfType<Solitaire>();
        slot1 = this.gameObject;
    }

    void Update()
    {
        if(clickCount == 1)
        {
            timer += Time.deltaTime;
        }
        else if(clickCount == 3)
        {
            timer = 0;
            clickCount = 1;
        }
        if(timer > doubleClickTime)
        {
            timer = 0;
            clickCount = 0;
        }

        GetMouseClick();
    }

    void GetMouseClick()
    {
        if(Input.GetMouseButtonDown(0))
        {
            clickCount++;

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit)
            {
                if(hit.collider.CompareTag("Deck"))
                {
                    Deck();
                }
                else if (hit.collider.CompareTag("Card"))
                {
                    Card(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Top"))
                {
                    Top(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Bottom"))
                {
                    Bottom(hit.collider.gameObject);
                }
            }
        }
    }

    void Deck()
    {
        Debug.Log("Click on Deck");
        solitaire.DealFromDeck();
        slot1 = this.gameObject;
    }

    void Card(GameObject selected)
    {
        Debug.Log("Clicked on Card");

        if (!selected.GetComponent<Selectable>().faceUp)  // if 클릭 한 카드가 뒷면인 경우 
        {
            if (!Blocked(selected))  // 클릭 한 카드가 차단되지 않은 경우 
            {
                // 카드 뒤집기
                selected.GetComponent<Selectable>().faceUp = true;
                slot1 = this.gameObject;
            }
        }
        else if (selected.GetComponent<Selectable>().inDeckPile)  // if 클릭 한 카드가 트립이있는 덱 더미에있는 경우 
        {
            // 차단되지 않은 경우 
            if (!Blocked(selected))
            {
                if(slot1 == selected)  // 같은 카드를 두번 클릭했을때
                {
                    if(DoubleClick())
                    {
                        AutoStack(selected);
                    }
                }
                else
                {
                    slot1 = selected;
                }
            }
        }
        else
        {
            // if 카드가 앞면이 보이면 
            // if 현재 선택된 카드가없는 경우 
            // 카드를 선택 

            if (slot1 == this.gameObject)  // gameObject를 대신 전달하므로 null이 아님
            {
                slot1 = selected;
            }

            // if 이미 선택된 카드가있는 경우 (동일한 카드가 아님) 
            else if (slot1 != selected)
            {
                // if 새 카드를 쌓을 수있는 경우  오래된 카드 
                if (Stackable(selected))
                {
                    // 쌓기 
                    Stack(selected);
                }
                else
                {
                    // 새로운 카드 선택
                    slot1 = selected;
                }


            }
            else if(slot1 == selected) // 같은 카드를 두번 클릭했을때
            {
                if(DoubleClick())
                {
                    Debug.Log("더블 클릭");
                    AutoStack(selected);
                    
                }
            }
        }
    }

    void Top(GameObject selected)
    {
        Debug.Log("Click on Top");
        if(slot1.CompareTag("Card"))
        {
            if(slot1.GetComponent<Selectable>().value == 1)  // 선택한 카드가 Ace이면 Top에 쌓음
            {
                Stack(selected);
            }
        }
    }

    void Bottom(GameObject selected)
    {
        Debug.Log("Click on Bottom");

        if(slot1.CompareTag("Card"))
        {
            if(slot1.GetComponent<Selectable>().value == 13) // 선택한 카드가 King이면 Bottom에 쌓음
            {
                Stack(selected);
            }
        }
    }

    bool Stackable(GameObject selected)
    {
        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();
        // 쌓기 가능 여부 확인

        if (!s2.inDeckPile)
        {
            if (s2.top)  // if 맨 위에있는 더미에 에이스를 킹에 맞게 스택해야하는 경우 
            {
                if (s1.suit == s2.suit || (s1.value == 1 && s2.suit == null))
                {
                    if (s1.value == s2.value + 1)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else  // if 맨 아래 파일에 다른 색상을 겹쳐서 킹에 에이스
            {
                if (s1.value == s2.value - 1)
                {
                    bool card1Red = true;
                    bool card2Red = true;

                    if (s1.suit == "C" || s1.suit == "S")
                    {
                        card1Red = false;
                    }

                    if (s2.suit == "C" || s2.suit == "S")
                    {
                        card2Red = false;
                    }

                    if (card1Red == card2Red)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void Stack (GameObject selected)
    {
        // K 상단 또는 빈 하단에 카드를 제자리에 스택 
        // 그렇지 않으면 음의 y 오프셋으로 카드를 스택

        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();
        float yOffset = 0.3f;

        if(s2.top || (!s2.top && s1.value == 13))
        {
            yOffset = 0;
        }

        slot1.transform.position = new Vector3(selected.transform.position.x, selected.transform.position.y - yOffset, selected.transform.position.z - 0.01f);
        slot1.transform.parent = selected.transform;

        if(s1.inDeckPile)
        {
            solitaire.tripsOnDisplay.Remove(slot1.name);
        }
        else if(s1.top && s2.top && s1.value == 1)
        {
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = 0;
            solitaire.topPos[s1.row].GetComponent<Selectable>().suit = null;
        }
        else if(s1.top)
        {
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = s1.value - 1;
        }
        else
        {
            solitaire.bottoms[s1.row].Remove(slot1.name);
        }

        s1.inDeckPile = false;
        s1.row = s2.row;

        if(s2.top)
        {
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = s1.value;
            solitaire.topPos[s1.row].GetComponent<Selectable>().suit = s1.suit;
            s1.top = true;
        }
        else
        {
            s1.top = false;
        }

        slot1 = this.gameObject;
    }

    bool Blocked(GameObject selected)
    {
        Selectable s2 = selected.GetComponent<Selectable>();
        if(s2.inDeckPile == true)
        {
            if (s2.name == solitaire.tripsOnDisplay.Last())
            {
                return false;
            }
            else
            {
                Debug.Log(s2.name + "is blocked by " + solitaire.tripsOnDisplay.Last());
                return true;
            }
        }
        else
        {
            if(s2.name == solitaire.bottoms[s2.row].Last())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    bool DoubleClick()
    {
        if(timer < doubleClickTime && clickCount == 2)
        {
            Debug.Log("Double Click");
            return true;
        }
        else
        {
            return false;
        }
    }

    void AutoStack(GameObject selected)
    {
        for(int i=0;i<solitaire.topPos.Length;i++)
        {
            Selectable stack = solitaire.topPos[i].GetComponent<Selectable>();
            if(selected.GetComponent<Selectable>().value == 1)  // 선택한 카드가 Ace일 경우 
            {
                if(solitaire.topPos[i].GetComponent<Selectable>().value == 0)  // Top 위치가 비었을 경우
                {
                    slot1 = selected;
                    Stack(stack.gameObject);  //  Ace 카드를 쌓음
                    break;  // 첫번째 빈 슬롯을 찾았을 경우
                }
            }
            else
            {
                if ((solitaire.topPos[i].GetComponent<Selectable>().suit == slot1.GetComponent<Selectable>().suit) && (solitaire.topPos[i].GetComponent<Selectable>().value == slot1.GetComponent<Selectable>().value - 1))
                {
                    // 마지막 카드 일경우(자식 카드가 없는 경우)
                    if (HasNoChildren(slot1))
                    {
                        slot1 = selected;
                        // 탑 슬롯을 찾은 경우 자동 쌓기 실행후 나가기
                        string lastCardname = stack.suit + stack.value.ToString();
                        if (stack.value == 1)
                        {
                            lastCardname = stack.suit + "A";
                        }
                        else if (stack.value == 11)
                        {
                            lastCardname = stack.suit + "J";
                        }
                        else if (stack.value == 12)
                        {
                            lastCardname = stack.suit + "Q";
                        }
                        else if (stack.value == 13)
                        {
                            lastCardname = stack.suit + "K";
                        }
                        GameObject lastCard = GameObject.Find(lastCardname);
                        Stack(lastCard);
                        break;
                    }
                }
            }
        }
    }

    bool HasNoChildren(GameObject card)
    {
        int i = 0;
        foreach(Transform child in card.transform)
        {
            i++;
        }
        if(i == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
