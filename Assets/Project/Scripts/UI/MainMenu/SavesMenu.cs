using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SavesMenu : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject chosenSaveMenuPrefab;

    private GameObject instantiatedChosenSaveMenu;

    private GameObject pointer;
    private int pointedOptionMenu;
    private List<GameObject> menuOptions = new List<GameObject>();

    private AudioSource closeSound;
    private AudioSource changeSound;
    private AudioSource selectSound;

    private int keyTimeDelayFirst = 20;
    private int keyTimeDelay = 10;
    private int keyTimeDelayer = 0;

    void Update()
    {
        KeysListener();
        PointOption(pointedOptionMenu, menuOptions);

        if (keyTimeDelayer > 0)
        {
            keyTimeDelayer--;
        }
    }

    void KeysListener()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            closeSound.Play();
            CloseMenu();
        }

        if (Input.GetKeyDown(KeyCode.S))
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
            keyTimeDelayer = keyTimeDelayFirst;
        }

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

        if (keyTimeDelayer == 0 && Input.GetKey(KeyCode.S))
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

        if (keyTimeDelayer == 0 && Input.GetKey(KeyCode.W))
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            selectSound.Play();

            if(ProgressSaving.GetSaves().Contains("save" + pointedOptionMenu))
            {
                instantiatedChosenSaveMenu = Instantiate(chosenSaveMenuPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, transform.parent);
                instantiatedChosenSaveMenu.transform.localPosition = new Vector3(0, 0, 0);

                instantiatedChosenSaveMenu.GetComponent<ChosenSaveMenu>().OpenMenu(pointer, "save" + pointedOptionMenu);


                menuOptions.Clear();
                Destroy(closeSound.gameObject, closeSound.clip.length);
                Destroy(changeSound.gameObject, changeSound.clip.length);
                Destroy(selectSound.gameObject, selectSound.clip.length);
                Destroy(gameObject);
            }
            else
            {
                ProgressSaving.saveName = "save" + pointedOptionMenu;
                ProgressSaving.CreateNewSave(ProgressSaving.saveName);
                SceneManager.LoadScene(ProgressSaving.GetSaveByName(ProgressSaving.saveName).gameStateSave.currentLvl);
            }
        }
    }

    public void OpenMenu(GameObject givenPointer, int pointOption)
    {
        pointer = givenPointer;

        closeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_Close);
        changeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_ChangeOption);
        selectSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_SelectOption);

        for (int i = 1; i < 5; i++)
        {
            string text = i.ToString();
            menuOptions.Add(transform.Find("Menu").Find("Options").Find(text).gameObject);

            if (!ProgressSaving.GetSaves().Contains("save" + (i - 1)))
            {
                menuOptions[i - 1].transform.Find("Name").GetComponent<TextMeshProUGUI>().text = "Empty";
                menuOptions[i - 1].transform.Find("Description").gameObject.SetActive(false);
            }
            else
            {
                menuOptions[i - 1].transform.Find("Description").Find("Date").GetComponent<TextMeshProUGUI>().text =
                    ProgressSaving.GetChangeDateOfSaveByName("save" + (i - 1));


                string chapter = ProgressSaving.GetSaveByName("save" + (i - 1)).gameStateSave.currentLvl.Replace("_", " ");
                int firstSpaceIndex = chapter.IndexOf(' ');
                int secondSpaceIndex = chapter.IndexOf(' ', firstSpaceIndex + 1);
                if (firstSpaceIndex >= 0 && secondSpaceIndex > firstSpaceIndex)
                {
                    chapter = chapter.Substring(0, secondSpaceIndex) + "<br>" + chapter.Substring(secondSpaceIndex + 1);
                }
                menuOptions[i - 1].transform.Find("Description").Find("Chapter").GetComponent<TextMeshProUGUI>().text = chapter;

            }
        }

        pointedOptionMenu = pointOption;
        PointOption(pointedOptionMenu, menuOptions);
    }

    public void CloseMenu()
    {
        pointer.transform.SetParent(transform.parent.transform.Find("Menu"));
        menuOptions.Clear();
        transform.parent.transform.Find("Menu").gameObject.SetActive(true);
        Destroy(closeSound.gameObject, closeSound.clip.length);
        Destroy(changeSound.gameObject, changeSound.clip.length);
        Destroy(selectSound.gameObject, selectSound.clip.length);
        Destroy(gameObject);
    }

    void PointOption(int option, List<GameObject> allOptions)
    {
        for (int i = 0; i < allOptions.Count; i++)
        {
            allOptions[i].transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
            allOptions[i].transform.Find("Description").Find("Date").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
            allOptions[i].transform.Find("Description").Find("Chapter").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
        }

        if (option < allOptions.Count)
        {
            allOptions[option].transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
            allOptions[option].transform.Find("Description").Find("Date").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
            allOptions[option].transform.Find("Description").Find("Chapter").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);

            pointer.transform.SetParent(allOptions[option].transform.Find("Name"));
        }
    }
}