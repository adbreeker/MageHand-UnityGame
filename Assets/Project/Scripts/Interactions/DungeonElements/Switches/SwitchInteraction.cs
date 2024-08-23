using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchInteraction : MonoBehaviour
{
    [Header("Object to interact")]
    public List<GameObject> interactedObjects;
    
    public virtual void Interact() //invoking Interaction method on assigned object
    {
        foreach(GameObject interactedObject in interactedObjects) 
        {
            if (interactedObject != null)
            {
                interactedObject.SendMessage("Interaction");
            }
        }
    }
}
