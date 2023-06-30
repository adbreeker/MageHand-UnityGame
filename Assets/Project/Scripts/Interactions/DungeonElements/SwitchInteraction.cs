using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchInteraction : MonoBehaviour
{
    public GameObject interactedObject;
    
    public void Interact()
    {
        if(interactedObject != null)
        {
            interactedObject.SendMessage("Interaction");
        }
    }
}
