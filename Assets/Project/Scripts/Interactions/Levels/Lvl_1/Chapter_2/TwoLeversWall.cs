using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoLeversWall : MonoBehaviour
{
    public GameObject wall1, wall2;

    bool lever1 = false;
    bool lever2 = false;
    public void Interaction(int leverId)
    {
        if(leverId == 1)
        {
            lever1 = !lever1;
        }
        if (leverId == 2)
        {
            lever2 = !lever2;
        }

        if (lever1 && lever2)
        {
            wall1.SendMessage("Interaction");
            wall2.SendMessage("Interaction");
        }
    }
}
