using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowTutorialText : MonoBehaviour
{
    public GameObject text;
    public TutorialPanel firstTutorialPanel;
    private bool ableToActivate = false;
    void Update()
    {
        if (firstTutorialPanel.wasLatelyOpened) ableToActivate = true;

        if (ableToActivate && !PlayerParams.Variables.uiActive)
        {
            text.SetActive(true);
        }
        else
        {
            text.SetActive(false);
        }
    }
}
