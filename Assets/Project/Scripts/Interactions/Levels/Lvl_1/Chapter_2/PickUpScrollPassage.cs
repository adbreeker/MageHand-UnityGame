using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpScrollPassage : MonoBehaviour
{
    [Header("Scroll")]
    public GameObject scroll;

    [Header("Bars passage")]
    public GameObject bars;

    Vector3 prevPos;

    private void Start() //get knife possition
    {
        prevPos = scroll.transform.position;
    }

    void Update() //if knife position changes (player picked it up) then open passage
    {
        if(scroll == null)
        {
            bars.SendMessage("Interaction");
            Destroy(this);
        }
        else if(scroll.transform.position != prevPos)
        {
            bars.SendMessage("Interaction");
            Destroy(this);
        }
    }
}
