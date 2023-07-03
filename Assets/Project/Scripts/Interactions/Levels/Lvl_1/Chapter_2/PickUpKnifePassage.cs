using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpKnifePassage : MonoBehaviour
{
    [Header("Knife")]
    public GameObject knife;

    [Header("Bars passage")]
    public GameObject bars;

    Vector3 prevPos;

    private void Start() //get knife possition
    {
        prevPos = knife.transform.position;
    }

    void Update() //if knife position changes (player picked it up) then open passage
    {
        if(knife.transform.position != prevPos)
        {
            bars.SendMessage("Interaction");
            Destroy(this);
        }
    }
}
