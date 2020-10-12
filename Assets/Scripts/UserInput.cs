using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    public GameObject slot1;  // 첫번째 선택한 카드 데이터
    private Solitaire solitaire;

    void Start()
    {
        solitaire = FindObjectOfType<Solitaire>();
        slot1 = this.gameObject;
    }

    void Update()
    {
        GetMouseClick();
    }

    void GetMouseClick()
    {
        if(Input.GetMouseButtonDown(0))
        {
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
                    Top();
                }
                else if (hit.collider.CompareTag("Bottom"))
                {
                    Bottom();
                }
            }
        }
    }

    void Deck()
    {
        Debug.Log("click Deck");
        solitaire.DealFromDeck();
    }

    void Card(GameObject selected)
    {
        Debug.Log("Clicked On Card");

        // if 클릭 한 카드가 뒷면이있는 경우 
            // 클릭 한 카드가 차단되지 않은 경우 
                // 뒤집어
                
        // if 클릭 한 카드가 트립이있는 덱 더미에있는 경우 
            // 차단되지 않은 경우 
                // 선택  
                
        // if 카드가 앞면이 보이면 
            // if 현재 선택된 카드가없는 경우 
                // 카드를 선택 

        if(slot1 == this.gameObject)  // gameObject를 대신 전달하므로 null이 아님
        {
            slot1 = selected;
        }
                
            // if 이미 선택된 카드가있는 경우 (동일한 카드가 아님) 

        else if(slot1 != selected)
        {
            // if 새 카드를 쌓을 수있는 경우  오래된 카드 
            if(Stackable(selected))
            {
                // 쌓기 
            }
            else
            {
                // 새로운 카드 선택
                slot1 = selected;
            }

            
        }


        // else if 이미 선택된 카드와 동일한 카드 인 경우
        // if 시간이 충분히 짧으면 더블 클릭
        // if 카드가 위로 날아갈 수있는 경우 수행합니다.
    }

    void Top()
    {
        Debug.Log("click Top");
    }

    void Bottom()
    {
        Debug.Log("click Bottom");
    }

    bool Stackable(GameObject selected)
    {
        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();
        // 쌓기 가능 여부 확인

        // if 맨 위에있는 더미에 에이스를 킹에 맞게 스택해야하는 경우 
        if(s2.top)
        {
            if(s1.suit == s2.suit || (s1.value == 1 && s2.suit == null))
            {
                if(s1.value == s2.value + 1)
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
            if(s1.value == s2.value -1)
            {
                bool card1Red = true;
                bool card2Red = true;

                if(s1.suit == "C" || s1.suit == "S")
                {
                    card1Red = false;
                }

                if(s2.suit == "C" || s2.suit == "S")
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
        return false;
    }
}
