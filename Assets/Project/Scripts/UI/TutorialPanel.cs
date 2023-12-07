using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    public GameObject tutorialPanelPrefab;

    public GameObject tutorialPanelHitboxToActivate;
    public GameObject tutorialPanelHitboxToDestroy;
    private GameObject tutorialPanel;
    private bool activatePanel = true;

    private bool openedPanel;

    private AudioSource openSound;
    private AudioSource closeSound;

    private void Update()
    {
        if(openedPanel) KeysListener();

        //Activates panel while player enters bounds of object that this script is connected to
        Bounds cubeBounds = GetComponent<Renderer>().bounds;
        if (cubeBounds.Contains(PlayerParams.Objects.player.transform.position) && activatePanel)
        {
            OpenPanel();
            openSound.Play();
            activatePanel = false;
        }
    }

    private void KeysListener()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
        {
            closeSound.Play();
            ClosePanel();
        }
    }

    private void OpenPanel()
    {
        //Disable other controls (close first, because it activates movement and enable other ui)
        PlayerParams.Controllers.inventory.CloseInventory();
        PlayerParams.Controllers.spellbook.CloseSpellbook();
        PlayerParams.Controllers.pauseMenu.CloseMenu();
        PlayerParams.Controllers.journal.CloseJournal();
        PlayerParams.Controllers.inventory.ableToInteract = false;
        PlayerParams.Controllers.spellbook.ableToInteract = false;
        PlayerParams.Controllers.pauseMenu.ableToInteract = false;
        PlayerParams.Controllers.journal.ableToInteract = false;
        PlayerParams.Variables.uiActive = true;
        PlayerParams.Objects.hand.SetActive(false);

        openSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_Open);
        closeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_Close);

        tutorialPanel = Instantiate(tutorialPanelPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        openedPanel = true;
    }

    private void ClosePanel()
    {
        openedPanel = false;

        PlayerParams.Variables.uiActive = false;
        PlayerParams.Objects.hand.SetActive(true);
        PlayerParams.Controllers.inventory.ableToInteract = true;
        PlayerParams.Controllers.spellbook.ableToInteract = true;
        PlayerParams.Controllers.pauseMenu.ableToInteract = true;
        PlayerParams.Controllers.journal.ableToInteract = true;

        if (tutorialPanelHitboxToActivate != null) tutorialPanelHitboxToActivate.SetActive(true);

        Destroy(openSound.gameObject, openSound.clip.length);

        if (tutorialPanelHitboxToDestroy != null) Destroy(tutorialPanelHitboxToDestroy);
        Destroy(gameObject);
        Destroy(tutorialPanel);
    }
}
