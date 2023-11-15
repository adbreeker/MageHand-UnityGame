using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DeleteSaveMenu : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject savesMenuPrefab;
    public GameObject chosenSaveMenuPrefab;

    private GameObject instantiatedSavesMenu;
    private GameObject instantiatedChosenSaveMenu;

    private GameObject pointer;
    private int pointedOptionMenu;
    private List<TextMeshProUGUI> menuOptions = new List<TextMeshProUGUI>();
    private TextMeshProUGUI title;

    private string saveName;

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
            if (pointedOptionMenu == 0)
            {
                ProgressSaving.DeleteExistingSave(saveName);

                instantiatedSavesMenu = Instantiate(savesMenuPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, transform.parent);
                instantiatedSavesMenu.transform.localPosition = new Vector3(0, 0, 0);
                instantiatedSavesMenu.GetComponent<SavesMenu>().OpenMenu(pointer, int.Parse(saveName.Substring(saveName.Length - 1)) - 1);

                menuOptions.Clear();
                Destroy(closeSound.gameObject, closeSound.clip.length);
                Destroy(changeSound.gameObject, changeSound.clip.length);
                Destroy(selectSound.gameObject, selectSound.clip.length);
                Destroy(gameObject);
            }
            else if (pointedOptionMenu == 1)
            {
                CloseMenu();
            }
        }
    }

    public void OpenMenu(GameObject givenPointer, string givenSaveName)
    {
        pointer = givenPointer;
        saveName = givenSaveName;

        closeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_Close);
        changeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_ChangeOption);
        selectSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_SelectOption);

        for (int i = 1; i < 3; i++)
        {
            string text = i.ToString();
            menuOptions.Add(transform.Find("Menu").Find("Options").Find(text).GetComponent<TextMeshProUGUI>());
        }

        title = transform.Find("Menu").Find("Options").Find("Title").GetComponent<TextMeshProUGUI>();
        title.text = "Do you want to<br>delete Save File " + int.Parse(givenSaveName.Substring(4)) + "?";

        pointedOptionMenu = 0;
        PointOption(pointedOptionMenu, menuOptions);
    }

    public void CloseMenu()
    {
        instantiatedChosenSaveMenu = Instantiate(chosenSaveMenuPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, transform.parent);
        instantiatedChosenSaveMenu.transform.localPosition = new Vector3(0, 0, 0);
        instantiatedChosenSaveMenu.GetComponent<ChosenSaveMenu>().OpenMenu(pointer, saveName);

        menuOptions.Clear();
        Destroy(closeSound.gameObject, closeSound.clip.length);
        Destroy(changeSound.gameObject, changeSound.clip.length);
        Destroy(selectSound.gameObject, selectSound.clip.length);
        Destroy(gameObject);
    }

    void PointOption(int option, List<TextMeshProUGUI> allOptions)
    {
        for (int i = 0; i < allOptions.Count; i++)
        {
            allOptions[i].color = new Color(0.2666f, 0.2666f, 0.2666f);
        }

        if (option < allOptions.Count)
        {
            allOptions[option].color = new Color(1f, 1f, 1f);

            pointer.transform.SetParent(allOptions[option].transform);
        }
    }
}