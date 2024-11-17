using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateTutorialOnPickUp : MonoBehaviour
{
    public GameObject tutorialEntryToActivate;

    public void OnPickUp()
    {
        if (tutorialEntryToActivate != null)
            tutorialEntryToActivate.SetActive(true);
        else
            Debug.LogWarning("No tutorial added");
    }
}
