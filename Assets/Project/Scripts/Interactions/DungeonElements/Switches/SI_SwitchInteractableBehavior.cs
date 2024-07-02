using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SI_SwitchInteractableBehavior : SwitchInteraction
{
    public override void Interact()
    {
        InteractableBehavior interactableBehavior = interactedObject.GetComponent<InteractableBehavior>();

        interactableBehavior.isInteractable = !interactableBehavior.isInteractable;
    }
}
