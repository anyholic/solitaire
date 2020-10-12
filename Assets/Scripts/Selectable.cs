 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public bool top = false;
    public string suit;
    public int value;
    public int row;
    public bool faceUp = false;
    public bool inDeckPile = false;

    private string valueString;

    private void Start()
    {
        if(CompareTag("Card"))
        {
            suit = transform.name[0].ToString();

            for(int i=1;i<transform.name.Length;i++)
            {
                char c = transform.name[i];
                valueString += c.ToString();
            }

            if(valueString == "A")
            {
                value = 1;
            }
            else if (valueString == "2")
            {
                value = 2;
            }
            else if (valueString == "3")
            {
                value = 3;
            }
            else if (valueString == "4")
            {
                value = 4;
            }
            else if (valueString == "5")
            {
                value = 5;
            }
            else if (valueString == "6")
            {
                value = 6;
            }
            else if (valueString == "7")
            {
                value = 7;
            }
            else if (valueString == "8")
            {
                value = 8;
            }
            else if (valueString == "9")
            {
                value = 9;
            }
            else if (valueString == "10")
            {
                value = 10;
            }
            else if (valueString == "J")
            {
                value = 11;
            }
            else if (valueString == "Q")
            {
                value = 12;
            }
            else if (valueString == "K")
            {
                value = 13;
            }

        }
    }
}
