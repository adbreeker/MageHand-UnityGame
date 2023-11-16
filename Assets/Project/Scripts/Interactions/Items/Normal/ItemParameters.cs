using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemParameters : MonoBehaviour
{
    [Header("Item name")]
    public string itemName;

    [Header("Item icon for inventory")]
    public GameObject itemIcon;

    public void OnPickUp()
    {
        Rigidbody  rb = GetComponent<Rigidbody>();
        ThrowObject to = GetComponent<ThrowObject>();

        if(rb != null )
        {
            Destroy(rb);
        }

        if(to != null) 
        {
            Destroy(to);
        }
    }
}
