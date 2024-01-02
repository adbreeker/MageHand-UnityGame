using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateTutorialOnPickUp : MonoBehaviour
{
    public GameObject tutorialEntryToActivate;

    public void OnPickUp()
    {
        tutorialEntryToActivate.SetActive(true);
    }
}
