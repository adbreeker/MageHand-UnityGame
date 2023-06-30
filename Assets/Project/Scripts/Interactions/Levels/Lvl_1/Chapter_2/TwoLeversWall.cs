using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoLeversWall : MonoBehaviour
{
    public GameObject wall1, wall2;

    public LeverBehavior lever1, lever2;

    private void Update()
    {
        if (lever1.leverON && lever2.leverON)
        {
            wall1.SendMessage("Interaction");
            wall2.SendMessage("Interaction");
            Destroy(this);
        }
    }
}
