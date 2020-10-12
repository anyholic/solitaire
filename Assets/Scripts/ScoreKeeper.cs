using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public Selectable[] topStacks;
    public GameObject highScorePanel;

    private void Update()
    {
        if(HasWon())
        {
            Win();
        }
    }

    public bool HasWon()
    {
        int i = 0;
        foreach (Selectable topstack in topStacks) 
        {
            i += topstack.value;
        }
        if(i>= 52)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Win()
    {
        Debug.Log("You have Won!");
        highScorePanel.SetActive(true);
    }
}
