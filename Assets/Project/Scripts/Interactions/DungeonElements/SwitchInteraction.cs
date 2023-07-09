using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchInteraction : MonoBehaviour
{
    [Header("Object to interact")]
    public GameObject interactedObject;
    
    public void Interact() //invoking Interaction method on assigned object
    {
        if(interactedObject != null)
        {
            interactedObject.SendMessage("Interaction");
        }
    }
}
