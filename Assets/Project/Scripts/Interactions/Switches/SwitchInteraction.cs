using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchInteraction : MonoBehaviour
{
    public GameObject interactedObject;

    [Tooltip("Values <= 0 sending only interaction, values > 0 sends also this id")]
    public int switchId = 0;
    
    public void Interact()
    {
        if(interactedObject != null)
        {
            if(switchId <= 0)
            {
                interactedObject.SendMessage("Interaction");
            }
            else
            {
                interactedObject.SendMessage("Interaction", switchId);
            }
        }
    }
}
