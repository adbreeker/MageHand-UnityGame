using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FMODUnity;

public class PauseMenu : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject menuPrefab;
    public GameObject resetMenuPrefab;
    public GameObject settingsMenuPrefab;
    public GameObject controlsMenuPrefab;
    public GameObject gesturesMenuPrefab;
    public GameObject tutorialsMenuPrefab;
    public GameObject quitToMenuMenuPrefab;
    public GameObject quitGameMenuPrefab;


    [Header("Settings")]
    public bool ableToInteract = true;
    public bool menuOpened = false;
    public bool freezeTime = false;

    private GameObject instantiatedMenu;
    private GameObject instantiatedResetMenu;
    private GameObject instantiatedSettingsMenu;
    private GameObject instantiatedControlsMenu;
    private GameObject instantiatedGesturesMenu;
    private GameObject instantiatedTutorialsMenu;
    private GameObject instantiatedQuitToMenuMenu;
    private GameObject instantiatedQuitGameMenu;
    private GameObject pointer;

    private int pointedOptionMenu;
    private bool atMainMenu = false;
    private List<TextMeshProUGUI> menuOptions = new List<TextMeshProUGUI>();

    private float keyTimeDelayFirst = 20f;
    private float keyTimeDelay = 10f;
    private float keyTimeDelayer = 0;

    private float fadeMusicSpeed = 0.05f;

    void Update()
    {
        //Change status of atMenu depending of that if it is active or not
        if (menuOpened && instantiatedMenu.transform.Find("Menu").gameObject.activeSelf == true) atMainMenu = true;
        else atMainMenu = false;

        //Open menu if possible
        if (ableToInteract && !PlayerParams.Variables.uiActive && Input.GetKeyDown(KeyCode.Escape))
        {
            OpenMenu();
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiOpen);
        }

        //if menu is opened and is at main page listen to keys
        if (menuOpened && atMainMenu)
        {
            KeysListener();
            PointOption(pointedOptionMenu);
        }

        if (keyTimeDelayer > 0) keyTimeDelayer -= 75 * Time.unscaledDeltaTime;

        //freeze game while menu is opened
        if (freezeTime) Time.timeScale = 0f;
        else Time.timeScale = GameParams.Variables.currentTimeScale;
    }
    void KeysListener()
    {
        //Close menu
        if (Input.GetKeyDown(KeyCode.Escape) && menuOpened && atMainMenu)
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiClose);
            CloseMenu();
        }

        //Move down
        if (Input.GetKeyDown(KeyCode.S))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiChangeOption);
            if (pointedOptionMenu < menuOptions.Count-1)
            {
                pointedOptionMenu++;
            }
            else
            {
                pointedOptionMenu = 0;
            }
            keyTimeDelayer = keyTimeDelayFirst;
        }

        //Move up
        if (Input.GetKeyDown(KeyCode.W))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiChangeOption);
            if (pointedOptionMenu > 0)
            {
                pointedOptionMenu--;
            }
            else
            {
                pointedOptionMenu = menuOptions.Count - 1;
            }
            keyTimeDelayer = keyTimeDelayFirst;
        }

        if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.S))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiChangeOption);
            if (pointedOptionMenu < menuOptions.Count - 1)
            {
                pointedOptionMenu++;
            }
            else
            {
                pointedOptionMenu = 0;
            }
            keyTimeDelayer = keyTimeDelay;
        }

        if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.W))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiChangeOption);
            if (pointedOptionMenu > 0)
            {
                pointedOptionMenu--;
            }
            else
            {
                pointedOptionMenu = menuOptions.Count - 1;
            }
            keyTimeDelayer = keyTimeDelay;
        }

        //Choice
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiSelectOption);
            if (pointedOptionMenu == 0)
            {
                //Spawn ResetMenu
                instantiatedMenu.transform.Find("Menu").gameObject.SetActive(false);

                instantiatedResetMenu = Instantiate(resetMenuPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, instantiatedMenu.transform);
                instantiatedResetMenu.transform.localPosition = new Vector3(0, 0, 0);

                instantiatedResetMenu.GetComponent<ResetMenu>().OpenMenu(pointer);
            }
            else if (pointedOptionMenu == 1)
            {
                //Spawn SettingsMenu
                instantiatedMenu.transform.Find("Menu").gameObject.SetActive(false);

                instantiatedSettingsMenu = Instantiate(settingsMenuPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, instantiatedMenu.transform);
                instantiatedSettingsMenu.transform.localPosition = new Vector3(0, 0, 0);

                instantiatedSettingsMenu.GetComponent<SettingsMenu>().OpenMenu(pointer);
            }
            else if (pointedOptionMenu == 2)
            {
                //Spawn ControlsMenu
                instantiatedMenu.transform.Find("Menu").gameObject.SetActive(false);

                instantiatedControlsMenu = Instantiate(controlsMenuPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, instantiatedMenu.transform);
                instantiatedControlsMenu.transform.localPosition = new Vector3(0, 0, 0);
            }
            else if (pointedOptionMenu == 3)
            {
                //Spawn GesturesMenu
                instantiatedMenu.transform.Find("Menu").gameObject.SetActive(false);

                instantiatedGesturesMenu = Instantiate(gesturesMenuPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, instantiatedMenu.transform);
                instantiatedGesturesMenu.transform.localPosition = new Vector3(0, 0, 0);

                instantiatedGesturesMenu.GetComponent<GesturesMenu>().OpenMenu(pointer);
            }
            else if (pointedOptionMenu == 4)
            {
                //Spawn TutorialsMenu
                instantiatedMenu.transform.Find("Menu").gameObject.SetActive(false);

                instantiatedTutorialsMenu = Instantiate(tutorialsMenuPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, instantiatedMenu.transform);
                instantiatedTutorialsMenu.transform.localPosition = new Vector3(0, 0, 0);

                instantiatedTutorialsMenu.GetComponent<TutorialsMenu>().OpenMenu(pointer);
            }
            else if (pointedOptionMenu == 5)
            {
                //Spawn QuitToMenuMenu
                instantiatedMenu.transform.Find("Menu").gameObject.SetActive(false);

                instantiatedQuitToMenuMenu = Instantiate(quitToMenuMenuPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, instantiatedMenu.transform);
                instantiatedQuitToMenuMenu.transform.localPosition = new Vector3(0, 0, 0);

                instantiatedQuitToMenuMenu.GetComponent<QuitToMenuMenu>().OpenMenu(pointer);
            }
            else if (pointedOptionMenu == 6)
            {
                //Spawn QuitGameMenu
                instantiatedMenu.transform.Find("Menu").gameObject.SetActive(false);

                instantiatedQuitGameMenu = Instantiate(quitGameMenuPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, instantiatedMenu.transform);
                instantiatedQuitGameMenu.transform.localPosition = new Vector3(0, 0, 0);

                instantiatedQuitGameMenu.GetComponent<QuitGameMenu>().OpenMenu(pointer);
            }
        }
    }

    public void OpenMenu()
    {
        //GameParams.Managers.soundManager.PauseAllAudioSourcesAndFadeOutMusic();
        GameParams.Managers.audioManager.PauseSFXsFadeOutMusic(fadeMusicSpeed);

        instantiatedMenu = Instantiate(menuPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        //Disable other controls
        PlayerParams.Controllers.inventory.ableToInteract = false;
        PlayerParams.Controllers.spellbook.ableToInteract = false;
        PlayerParams.Controllers.journal.ableToInteract = false;
        PlayerParams.Variables.uiActive = true;
        PlayerParams.Objects.hand.SetActive(false);

        //Assing proper objects
        pointer = instantiatedMenu.transform.Find("Pointer").gameObject;

        for (int i = 1; i < 8; i++)
        {
            string text = i.ToString();
            menuOptions.Add(instantiatedMenu.transform.Find("Menu").Find("Options").Find(text).GetComponent<TextMeshProUGUI>());
        }

        pointedOptionMenu = 0;
        PointOption(pointedOptionMenu);
        freezeTime = true;
        menuOpened = true;
    }

    public void CloseMenu()
    {
        if (menuOpened)
        {
            //GameParams.Managers.soundManager.UnPauseAllAudioSourcesFadeInMusic();
            GameParams.Managers.audioManager.UnpauseSFXsFadeInMusic(fadeMusicSpeed);
        }
        Destroy(instantiatedMenu);

        //Enable other controls
        PlayerParams.Controllers.inventory.ableToInteract = true;
        PlayerParams.Controllers.spellbook.ableToInteract = true;
        PlayerParams.Controllers.journal.ableToInteract = true;
        PlayerParams.Variables.uiActive = false;
        PlayerParams.Objects.hand.SetActive(true);

        freezeTime = false;
        menuOpened = false;
        atMainMenu = false;
        menuOptions.Clear();
    }

    void PointOption(int option)
    {
        //Change color of text and make pointer child of chosen option
        for (int i = 0; i < menuOptions.Count; i++)
        {
            menuOptions[i].color = new Color(0.2666f, 0.2666f, 0.2666f);
        }

        if (option < menuOptions.Count)
        {
            menuOptions[option].color = new Color(1f, 1f, 1f);

            pointer.transform.SetParent(menuOptions[option].transform);
        }
    }
}
