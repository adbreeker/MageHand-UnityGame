using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class MainMenu : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject newGameMenuPrefab;
    public GameObject quitGameMenuPrefab;
    public GameObject settingsMenuPrefab;
    public GameObject savesMenuPrefab;

    private GameObject instantiatedNewGameMenu;
    private GameObject instantiatedQuitGameMenu;
    private GameObject instantiatedSettingsMenu;
    private GameObject instantiatedSavesMenu;
    private GameObject pointer;

    private int pointedOptionMenu;
    private bool atMainMenu = true;
    private bool checkAtMainMenuChange = true;
    private bool closing = false;
    private List<TextMeshProUGUI> menuOptions = new List<TextMeshProUGUI>();

    private AudioSource changeSound;
    private AudioSource selectSound;

    private float keyTimeDelayFirst = 20f;
    private float keyTimeDelay = 10f;
    private float keyTimeDelayer = 0;

    private void Awake()
    {
        ProgressSaving.CreateSavesDirectory();
    }

    private void Start()
    {
        DisplayMenu();
    }
    void Update()
    {
        //Change status of atMenu depending of that if it is active or not
        if (transform.Find("Menu").gameObject.activeSelf == true)
        {
            atMainMenu = true;
        }
        else
        {
            atMainMenu = false;
        }

        if (checkAtMainMenuChange != atMainMenu)
        {
            ChangesDependentOnSaves(false);
            checkAtMainMenuChange = atMainMenu;
        }

        if (atMainMenu && !closing)
        {
            KeysListenerMenu();
            PointOption(pointedOptionMenu, menuOptions);
        }

        if (keyTimeDelayer > 0) keyTimeDelayer -= 75 * Time.unscaledDeltaTime;
    }

    void KeysListenerMenu()
    {
        //Move down
        if (Input.GetKeyDown(KeyCode.S))
        {
            changeSound.Play();
            GoDown(pointedOptionMenu);
            keyTimeDelayer = keyTimeDelayFirst;
        }

        //Move up
        if (Input.GetKeyDown(KeyCode.W))
        {
            changeSound.Play();
            GoUp(pointedOptionMenu);
            keyTimeDelayer = keyTimeDelayFirst;
        }

        if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.S))
        {
            changeSound.Play();
            GoDown(pointedOptionMenu);
            keyTimeDelayer = keyTimeDelay;
        }

        if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.W))
        {
            changeSound.Play();
            GoUp(pointedOptionMenu);
            keyTimeDelayer = keyTimeDelay;
        }

        //Choice
        if (Input.GetKeyDown(KeyCode.Space))
        {
            selectSound.Play();
            if (pointedOptionMenu == 0)
            {
                //Continue
                ProgressSaving.saveName = ProgressSaving.GetRecentlyChangedSave();

                //There we need to check if mediapipeProcess is loaded
                closing = true;
                GameParams.Managers.fadeInOutManager.ChangeScene("Loading_Screen");
            }
            else if (pointedOptionMenu == 1)
            {
                //Spawn NewGameMenu
                transform.Find("Menu").gameObject.SetActive(false);

                instantiatedNewGameMenu = Instantiate(newGameMenuPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
                instantiatedNewGameMenu.transform.localPosition = new Vector3(0, 0, 0);

                instantiatedNewGameMenu.GetComponent<NewGameMenu>().OpenMenu(pointer);

            }
            else if (pointedOptionMenu == 2)
            {
                //Spawn SavesMenu
                transform.Find("Menu").gameObject.SetActive(false);

                instantiatedSavesMenu = Instantiate(savesMenuPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
                instantiatedSavesMenu.transform.localPosition = new Vector3(0, 0, 0);

                instantiatedSavesMenu.GetComponent<SavesMenu>().OpenMenu(pointer);
            }
            else if (pointedOptionMenu == 3)
            {
                //Spawn SettingsMenu
                transform.Find("Menu").gameObject.SetActive(false);

                instantiatedSettingsMenu = Instantiate(settingsMenuPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
                instantiatedSettingsMenu.transform.localPosition = new Vector3(0, 0, 0);

                instantiatedSettingsMenu.GetComponent<SettingsMenu>().OpenMenu(pointer, true);
            }
            else if(pointedOptionMenu == 4)
            {
                //Spawn QuitGameMenu
                transform.Find("Menu").gameObject.SetActive(false);

                instantiatedQuitGameMenu = Instantiate(quitGameMenuPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
                instantiatedQuitGameMenu.transform.localPosition = new Vector3(0, 0, 0);

                instantiatedQuitGameMenu.GetComponent<QuitGameMenu>().OpenMenu(pointer);
            }
        }
    }

    private void GoDown(int pointedOption)
    {
        if (pointedOption < menuOptions.Count - 1)
        {
            pointedOption++;
            if (menuOptions[pointedOption].gameObject.activeSelf == false)
            {
                GoDown(pointedOption);
            }
            else
            {
                pointedOptionMenu = pointedOption;
            }
        }
        else
        {
            if (menuOptions[0].gameObject.activeSelf == false)
            {
                GoDown(0);
            }
            else
            {
                pointedOptionMenu = 0;
            }
        }
    }
    
    private void GoUp(int pointedOption)
    {
        if (pointedOption > 0)
        {
            pointedOption--;
            if (menuOptions[pointedOption].gameObject.activeSelf == false)
            {
                GoUp(pointedOption);
            }
            else
            {
                pointedOptionMenu = pointedOption;
            }
        }
        else
        {
            if (menuOptions[menuOptions.Count - 1].gameObject.activeSelf == false)
            {
                GoUp(menuOptions.Count - 1);
            }
            else
            {
                pointedOptionMenu = menuOptions.Count - 1;
            }
        }
    }

    public void DisplayMenu()
    {
        //Assing proper objects
        pointer = transform.Find("Pointer").gameObject;

        changeSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.UI_ChangeOption);
        selectSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.UI_SelectOption);

        for (int i = 1; i < 6; i++)
        {
            string text = i.ToString();
            menuOptions.Add(transform.Find("Menu").Find("Options").Find(text).GetComponent<TextMeshProUGUI>());
        }

        ChangesDependentOnSaves(true);
        PointOption(pointedOptionMenu, menuOptions);
    }

    public void ChangesDependentOnSaves(bool onStart)
    {
        if (ProgressSaving.GetSaves().Count(item => item != "TestSaveDevEdit") > 3)
        {
            transform.Find("Menu").Find("Options").Find("2").gameObject.SetActive(false);
        }
        else
        {
            transform.Find("Menu").Find("Options").Find("2").gameObject.SetActive(true);
        }

        if (ProgressSaving.GetSaves().Count(item => item != "TestSaveDevEdit") > 0)
        {
            if(onStart) pointedOptionMenu = 0;
            transform.Find("Menu").Find("Options").Find("1").gameObject.SetActive(true);
            //transform.Find("Menu").Find("Options").Find("3").gameObject.SetActive(true);
        }
        else
        {
            if(onStart) pointedOptionMenu = 1;
            transform.Find("Menu").Find("Options").Find("1").gameObject.SetActive(false);
            //transform.Find("Menu").Find("Options").Find("3").gameObject.SetActive(false);
        }
    }

    void PointOption(int option, List<TextMeshProUGUI> allOptions)
    {
        //Change color of text and make pointer child of chosen option
        for (int i = 0; i < allOptions.Count; i++)
        {
            allOptions[i].color = new Color(0.2666f, 0.2666f, 0.2666f);

            if (i == 0)
            {
                allOptions[i].transform.Find("Save").gameObject.SetActive(false);
            }
        }

        if (option < allOptions.Count)
        {
            allOptions[option].color = new Color(1f, 1f, 1f);

            pointer.transform.SetParent(allOptions[option].transform);


            if (option == 0)
            {
                ProgressSaving.GetRecentlyChangedSave();

                allOptions[option].transform.Find("Save").Find("SaveInfo").Find("Name").GetComponent<TextMeshProUGUI>().text =
                    "Save File " + ProgressSaving.GetRecentlyChangedSave().Substring(4);

                allOptions[option].transform.Find("Save").Find("SaveInfo").Find("Description").Find("Date").GetComponent<TextMeshProUGUI>().text =
                    ProgressSaving.GetChangeDateOfSaveByName(ProgressSaving.GetRecentlyChangedSave());

                string chapter = "";
                if (ProgressSaving.GetSaveByName(ProgressSaving.GetRecentlyChangedSave()).gameStateSave.currentLvl != "Level_0_Tutorial")
                {
                    chapter = ProgressSaving.GetSaveByName(ProgressSaving.GetRecentlyChangedSave()).gameStateSave.currentLvl.Replace("_", " ");
                    int firstSpaceIndex = chapter.IndexOf(' ');
                    int secondSpaceIndex = chapter.IndexOf(' ', firstSpaceIndex + 1);
                    if (firstSpaceIndex >= 0 && secondSpaceIndex > firstSpaceIndex)
                    {
                        chapter = chapter.Substring(0, secondSpaceIndex) + "<br>" + chapter.Substring(secondSpaceIndex + 1);
                    }
                }
                else
                {
                    chapter = "Tutorial";
                }

                allOptions[option].transform.transform.Find("Save").Find("SaveInfo").Find("Description").Find("Chapter").GetComponent<TextMeshProUGUI>().text = chapter;

                allOptions[option].transform.Find("Save").gameObject.SetActive(true);
            }
        }
    }
}
