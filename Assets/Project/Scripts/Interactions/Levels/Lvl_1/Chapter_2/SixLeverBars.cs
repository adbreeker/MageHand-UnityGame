using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SixLeverBars : MonoBehaviour
{
    public LeverBehavior lever1, lever2, lever3, lever4, lever5, lever6;
    public GameObject barsPassage;

    bool barsOpen = false;

    private void Update()
    {
        if (lever1.leverON && lever2.leverON && !lever3.leverON && !lever4.leverON && lever5.leverON && !lever6.leverON)
        {
            if (!barsOpen)
            {
                barsPassage.SendMessage("Interaction");
                barsOpen = true;
            }
        }
        else
        {
            if(barsOpen)
            {
                barsPassage.SendMessage("Interaction");
                barsOpen = false;
            }
        }
    }
}
