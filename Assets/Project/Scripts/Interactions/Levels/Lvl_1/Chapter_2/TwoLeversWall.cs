using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoLeversWall : MonoBehaviour
{
    [Header("Walls to rotate")]
    public GameObject wall1;
    public GameObject wall2;

    [Header("Levers")]
    public LeverBehavior lever1;
    public LeverBehavior lever2;

    private void Update() //rotate walls if both lever were pulled and destroy this script then
    {
        if (lever1.leverON && lever2.leverON)
        {
            wall1.SendMessage("Interaction");
            wall2.SendMessage("Interaction");
            Destroy(this);
        }
    }
}
