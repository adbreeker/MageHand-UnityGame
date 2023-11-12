using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject newGameMenuPrefab;
    public GameObject quitGameMenuPrefab;
    public GameObject savesMenuPrefab;

    private GameObject instantiatedNewGameMenu;
    private GameObject instantiatedQuitGameMenu;
    private GameObject instantiatedSavesMenu;
    private GameObject pointer;

    private int pointedOptionMenu;
    private bool atMainMenu = true;
    private bool checkAtMainMenuChange = true;
    private List<TextMeshProUGUI> menuOptions = new List<TextMeshProUGUI>();

    private AudioSource changeSound;
    private AudioSource selectSound;

    private int keyTimeDelayFirst = 20;
    private int keyTimeDelay = 10;
    private int keyTimeDelayer = 0;

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
            ChangesDependentOnSaves();
            checkAtMainMenuChange = atMainMenu;
        }

                if (atMainMenu)
        {
            KeysListenerMenu();
            PointOption(pointedOptionMenu, menuOptions);
        }

        if (keyTimeDelayer > 0)
        {
            keyTimeDelayer--;
        }
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

        if (keyTimeDelayer == 0 && Input.GetKey(KeyCode.S))
        {
            changeSound.Play();
            GoDown(pointedOptionMenu);
            keyTimeDelayer = keyTimeDelay;
        }

        if (keyTimeDelayer == 0 && Input.GetKey(KeyCode.W))
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
                SceneManager.LoadScene(ProgressSaving.GetSaveByName(ProgressSaving.saveName).gameStateSave.currentLvl);
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

        changeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_ChangeOption);
        selectSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_SelectOption);

        for (int i = 1; i < 5; i++)
        {
            string text = i.ToString();
            menuOptions.Add(transform.Find("Menu").Find("Options").Find(text).GetComponent<TextMeshProUGUI>());
        }

        ChangesDependentOnSaves();
        PointOption(pointedOptionMenu, menuOptions);
    }

    public void ChangesDependentOnSaves()
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
            pointedOptionMenu = 0;
            transform.Find("Menu").Find("Options").Find("1").gameObject.SetActive(true);
            //transform.Find("Menu").Find("Options").Find("3").gameObject.SetActive(true);
        }
        else
        {
            pointedOptionMenu = 1;
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
        }

        if (option < allOptions.Count)
        {
            allOptions[option].color = new Color(1f, 1f, 1f);

            pointer.transform.SetParent(allOptions[option].transform);
        }

        //*************if option is 1 display save info on the left
    }
}
