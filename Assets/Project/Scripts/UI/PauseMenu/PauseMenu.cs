using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject menuPrefab;
    public GameObject resetMenuPrefab;
    public GameObject settingsMenuPrefab;
    public GameObject controlsMenuPrefab;
    public GameObject gesturesMenuPrefab;
    public GameObject quitToMenuMenuPrefab;
    public GameObject quitGameMenuPrefab;


    [Header("Settings")]
    public bool ableToInteract = true;
    public bool menuOpened = false;

    private GameObject instantiatedMenu;
    private GameObject instantiatedResetMenu;
    private GameObject instantiatedSettingsMenu;
    private GameObject instantiatedControlsMenu;
    private GameObject instantiatedGesturesMenu;
    private GameObject instantiatedQuitToMenuMenu;
    private GameObject instantiatedQuitGameMenu;
    private GameObject pointer;

    private int pointedOptionMenu;
    private bool atMainMenu = false;
    private List<TextMeshProUGUI> menuOptions = new List<TextMeshProUGUI>();

    private AudioSource closeSound;
    private AudioSource openSound;
    private AudioSource changeSound;
    private AudioSource selectSound;

    private float keyTimeDelayFirst = 20f;
    private float keyTimeDelay = 10f;
    private float keyTimeDelayer = 0;

    void Update()
    {
        //Change status of atMenu depending of that if it is active or not
        if (menuOpened && instantiatedMenu.transform.Find("Menu").gameObject.activeSelf == true) atMainMenu = true;
        else atMainMenu = false;

        //Open menu if possible
        if (ableToInteract && !PlayerParams.Variables.uiActive && Input.GetKeyDown(KeyCode.Escape))
        {
            OpenMenu();
            openSound.Play();
        }

        //if menu is opened and is at main page listen to keys
        if (menuOpened && atMainMenu)
        {
            KeysListener();
            PointOption(pointedOptionMenu);
        }

        if (keyTimeDelayer > 0) keyTimeDelayer -= 75 * Time.unscaledDeltaTime;

        //freeze game while menu is opened
        if (menuOpened) Time.timeScale = 0f;
        else Time.timeScale = 1f;
    }
    void KeysListener()
    {
        //Close menu
        if (Input.GetKeyDown(KeyCode.Escape) && menuOpened && atMainMenu)
        {
            closeSound.Play();
            CloseMenu();
        }

        //Move down
        if (Input.GetKeyDown(KeyCode.S))
        {
            changeSound.Play();
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
            changeSound.Play();
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
            changeSound.Play();
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
            changeSound.Play();
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
            selectSound.Play();
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
                //Spawn QuitToMenuMenu
                instantiatedMenu.transform.Find("Menu").gameObject.SetActive(false);

                instantiatedQuitToMenuMenu = Instantiate(quitToMenuMenuPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, instantiatedMenu.transform);
                instantiatedQuitToMenuMenu.transform.localPosition = new Vector3(0, 0, 0);

                instantiatedQuitToMenuMenu.GetComponent<QuitToMenuMenu>().OpenMenu(pointer);
            }
            else if (pointedOptionMenu == 5)
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
        FindObjectOfType<SoundManager>().PauseAllAudioSources();

        instantiatedMenu = Instantiate(menuPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        //Disable other controls
        PlayerParams.Controllers.inventory.ableToInteract = false;
        PlayerParams.Controllers.spellbook.ableToInteract = false;
        PlayerParams.Controllers.journal.ableToInteract = false;
        PlayerParams.Variables.uiActive = true;
        PlayerParams.Objects.hand.SetActive(false);

        //Assing proper objects
        pointer = instantiatedMenu.transform.Find("Pointer").gameObject;

        openSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_Open);
        closeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_Close);
        changeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_ChangeOption);
        selectSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_SelectOption);

        for (int i = 1; i < 7; i++)
        {
            string text = i.ToString();
            menuOptions.Add(instantiatedMenu.transform.Find("Menu").Find("Options").Find(text).GetComponent<TextMeshProUGUI>());
        }

        pointedOptionMenu = 0;
        PointOption(pointedOptionMenu);
        menuOpened = true;
    }

    public void CloseMenu()
    {
        if (menuOpened)
        {
            FindObjectOfType<SoundManager>().UnPauseAllAudioSources();
            Destroy(openSound.gameObject, openSound.clip.length);
            Destroy(closeSound.gameObject, closeSound.clip.length);
            Destroy(changeSound.gameObject, changeSound.clip.length);
            Destroy(selectSound.gameObject, selectSound.clip.length);
        }
        Destroy(instantiatedMenu);

        //Enable other controls
        PlayerParams.Controllers.inventory.ableToInteract = true;
        PlayerParams.Controllers.spellbook.ableToInteract = true;
        PlayerParams.Controllers.journal.ableToInteract = true;
        PlayerParams.Variables.uiActive = false;
        PlayerParams.Objects.hand.SetActive(true);

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
