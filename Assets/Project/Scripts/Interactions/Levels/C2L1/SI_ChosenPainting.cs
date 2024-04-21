using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SI_ChosenPainting : SwitchInteraction
{
    public GameObject painting;

    public override void Interact()
    {
        interactedObject.GetComponent<ChooseOnePainting>().ChosenPaintingInteraction(painting);
    }
}
