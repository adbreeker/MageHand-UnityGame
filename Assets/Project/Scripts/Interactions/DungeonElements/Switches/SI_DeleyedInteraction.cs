using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SI_DeleyedInteraction : SwitchInteraction
{
    [Header("Deley time")]
    public float deley;

    public override void Interact()
    {
        StartCoroutine(Deleyed());
    }

    IEnumerator Deleyed()
    {
        yield return new WaitForSeconds(deley);
        foreach (GameObject interactedObject in interactedObjects)
        {
            if (interactedObject != null)
            {
                interactedObject.SendMessage("Interaction");
            }
        }
    }
}
