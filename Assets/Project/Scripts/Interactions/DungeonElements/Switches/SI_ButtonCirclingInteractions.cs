using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SI_ButtonCirclingInteractions : SwitchInteraction
{
    ButtonBehavior _button;
    private void Awake()
    {
        _button = GetComponent<ButtonBehavior>();
    }

    public override void Interact()
    {
        interactedObjects[(_button.clickCounter - 1) % interactedObjects.Count].SendMessage("Interaction");
    }
}
