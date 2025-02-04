using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateTutorialOnNoteEnd : MonoBehaviour
{
    public GameObject tutorialEntryToActivate;
    public GameObject note;
    public bool activatePanel = true;

    private void Update()
    {
        if (activatePanel && note == null && !PlayerParams.Variables.uiActive)
        {
            tutorialEntryToActivate.SetActive(true);
            foreach (ActivateTutorialOnNoteEnd activator in FindObjectsByType<ActivateTutorialOnNoteEnd>(FindObjectsSortMode.None))
            {
                activator.activatePanel = false;
            }
        }
    }
}
