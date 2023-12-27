using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateTutorialHandSpells : MonoBehaviour
{
    public GameObject tutorialEntryToActivate;
    public GameObject scroll;

    private bool activatePanel = false;

    private void Update()
    {
        if (activatePanel && !PlayerParams.Variables.uiActive) tutorialEntryToActivate.SetActive(true);
        if (scroll == null && Input.GetKeyDown(KeyCode.B)) activatePanel = true;
    }
}
