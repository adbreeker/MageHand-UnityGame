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

    private AudioSource openSound;
    private AudioSource closeSound;
    private AudioSource selectSound;

    private void Update()
    {
        KeysListener();

        //Activates panel while player enters bounds of object that this script is connected to
        Bounds cubeBounds = GetComponent<BoxCollider>().bounds;
        if (cubeBounds.Contains(PlayerParams.Objects.player.transform.position) && activatePanelOnEntry)
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
            if (playOpenSound) openSound.Play();
            activatePanelOnEntry = false;
        }
    }

    private void KeysListener()
    {
        if (currentPanelNumber == tutorialPanelPrefabs.Count - 1 && openedPanel && Input.GetKeyDown(KeyCode.Space))
        {
            closeSound.Play();
            ClosePanel();
        }
        else if (openedPanel && Input.GetKeyDown(KeyCode.Space))
        {
            selectSound.Play();
            Destroy(tutorialPanel);
            currentPanelNumber++;
            tutorialPanel = Instantiate(tutorialPanelPrefabs[currentPanelNumber], new Vector3(0, 0, 0), Quaternion.identity);
            tutorialPanel.GetComponent<Canvas>().worldCamera = PlayerParams.Objects.uiCamera;
            tutorialPanel.GetComponent<Canvas>().planeDistance = 1.05f;
        }

        if (wasLatelyOpened && !openedPanel && !PlayerParams.Variables.uiActive && Input.GetKeyDown(KeyCode.T))
        {
            OpenPanel();
            openSound.Play();
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

        openSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_Open);
        closeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_Close);
        selectSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_SelectOption);

        currentPanelNumber = 0;

        tutorialPanel = Instantiate(tutorialPanelPrefabs[currentPanelNumber], new Vector3(0, 0, 0), Quaternion.identity);
        tutorialPanel.GetComponent<Canvas>().worldCamera = PlayerParams.Objects.uiCamera;
        tutorialPanel.GetComponent<Canvas>().planeDistance = 1.05f;

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


        if(openSound != null) Destroy(openSound.gameObject, openSound.clip.length);

        //if (tutorialPanelHitboxToActivate != null) tutorialPanelHitboxToActivate.SetActive(true);
        if (tutorialPanelHitboxToDeactivate != null) tutorialPanelHitboxToDeactivate.GetComponent<TutorialPanel>().activatePanelOnEntry = false;
        Destroy(tutorialPanel);
    }
}
