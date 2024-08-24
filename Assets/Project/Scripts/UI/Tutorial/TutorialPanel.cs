using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    public List<GameObject> tutorialPanelPrefabs;

    //public GameObject tutorialPanelHitboxToActivate;
    public GameObject tutorialPanelHitboxToDeactivate;
    public bool playOpenSound = true;
    public bool wasLatelyOpened = false;

    private GameObject tutorialPanel;
    public bool activatePanelOnEntry = true;

    private bool openedPanel;
    private int currentPanelNumber;

    private void Update()
    {
        KeysListener();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerParams.Objects.player && activatePanelOnEntry)
        {
            foreach (TutorialPanel panel in FindObjectsOfType<TutorialPanel>())
            {
                if (panel.wasLatelyOpened)
                {
                    panel.wasLatelyOpened = false;
                    panel.ClosePanel();
                }
            }

            OpenPanel();
            wasLatelyOpened = true;
            if (playOpenSound) RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.UI_Open);
            activatePanelOnEntry = false;
        }
    }

    private void KeysListener()
    {
        if (currentPanelNumber == tutorialPanelPrefabs.Count - 1 && openedPanel && Input.GetKeyDown(KeyCode.Space))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.UI_Close);
            ClosePanel();
        }
        else if (openedPanel && Input.GetKeyDown(KeyCode.Space))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.UI_SelectOption);
            Destroy(tutorialPanel);
            currentPanelNumber++;
            tutorialPanel = Instantiate(tutorialPanelPrefabs[currentPanelNumber], new Vector3(0, 0, 0), Quaternion.identity);
            //tutorialPanel.GetComponent<Canvas>().worldCamera = PlayerParams.Objects.uiCamera;
            //tutorialPanel.GetComponent<Canvas>().planeDistance = 1.05f;
        }

        if (wasLatelyOpened && !openedPanel && !PlayerParams.Variables.uiActive && Input.GetKeyDown(KeyCode.T))
        {
            OpenPanel();
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.UI_Open);
        }
    }

    private void OpenPanel()
    {
        //Disable other controls (close first, because it activates movement and enable other ui)
        PlayerParams.Controllers.inventory.CloseInventory();
        PlayerParams.Controllers.spellbook.CloseSpellbook();
        PlayerParams.Controllers.pauseMenu.CloseMenu();
        PlayerParams.Controllers.spellsMenu.CloseMenu();
        PlayerParams.Controllers.journal.CloseJournal();
        PlayerParams.Controllers.inventory.ableToInteract = false;
        PlayerParams.Controllers.spellbook.ableToInteract = false;
        PlayerParams.Controllers.pauseMenu.ableToInteract = false;
        PlayerParams.Controllers.journal.ableToInteract = false;
        PlayerParams.Variables.uiActive = true;
        PlayerParams.Objects.hand.SetActive(false);

        currentPanelNumber = 0;

        tutorialPanel = Instantiate(tutorialPanelPrefabs[currentPanelNumber], new Vector3(0, 0, 0), Quaternion.identity);
        //tutorialPanel.GetComponent<Canvas>().worldCamera = PlayerParams.Objects.uiCamera;
        //tutorialPanel.GetComponent<Canvas>().planeDistance = 1.05f;

        openedPanel = true;
    }

    public void ClosePanel()
    {
        openedPanel = false;

        PlayerParams.Variables.uiActive = false;
        PlayerParams.Objects.hand.SetActive(true);
        PlayerParams.Controllers.inventory.ableToInteract = true;
        PlayerParams.Controllers.spellbook.ableToInteract = true;
        PlayerParams.Controllers.pauseMenu.ableToInteract = true;
        PlayerParams.Controllers.journal.ableToInteract = true;

        //if (tutorialPanelHitboxToActivate != null) tutorialPanelHitboxToActivate.SetActive(true);
        if (tutorialPanelHitboxToDeactivate != null) tutorialPanelHitboxToDeactivate.GetComponent<TutorialPanel>().activatePanelOnEntry = false;
        Destroy(tutorialPanel);
    }
}
