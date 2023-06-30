using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpKnifePassage : MonoBehaviour
{
    public GameObject knife;
    public GameObject bars;

    Vector3 prevPos;

    private void Start()
    {
        prevPos = knife.transform.position;
    }

    void Update()
    {
        if(knife.transform.position != prevPos)
        {
            bars.SendMessage("Interaction");
            Destroy(this);
        }
    }
}
