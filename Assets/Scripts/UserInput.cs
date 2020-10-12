﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    private Solitaire solitaire;

    void Start()
    {
        solitaire = FindObjectOfType<Solitaire>();
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
                    Card();
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

    void Card()
    {
        Debug.Log("click Card");
    }

    void Top()
    {
        Debug.Log("click Top");
    }

    void Bottom()
    {
        Debug.Log("click Bottom");
    }
}
