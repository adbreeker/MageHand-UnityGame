using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SixLeverBars : MonoBehaviour
{
    [Header("Levers")]
    public LeverBehavior lever1;
    public LeverBehavior lever2;
    public LeverBehavior lever3;
    public LeverBehavior lever4;
    public LeverBehavior lever5;
    public LeverBehavior lever6;

    [Header("Bars passage to open")]
    public GameObject barsPassage;

    bool barsOpen = false;

    private void Update() //listen to all levers state
    {
        //if all lever are in necessary state, then open passage
        if (lever1.leverON && lever2.leverON && !lever3.leverON && !lever4.leverON && lever5.leverON && !lever6.leverON)
        {
            if (!barsOpen)
            {
                barsPassage.SendMessage("Interaction");
                barsOpen = true;
            }
        }
        else //else close passage;
        {
            if(barsOpen)
            {
                barsPassage.SendMessage("Interaction");
                barsOpen = false;
            }
        }
    }
}
