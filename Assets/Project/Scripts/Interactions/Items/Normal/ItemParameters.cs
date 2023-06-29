using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemParameters : MonoBehaviour
{
    public string itemName;
    public GameObject itemIcon;

    public bool fixItemRotationInHand = false;

    public void FixRotation()
    {
        if(fixItemRotationInHand)
        {
            GameObject player = FindObjectOfType<PlayerMovement>().gameObject;
            Vector3 front = player.transform.forward * 1000;
            transform.LookAt(front);
        }
    }
}
