using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using FMODUnity;

public class ChosenSaveMenu : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject savesMenuPrefab;
    public GameObject deleteSaveMenuPrefab;

    private GameObject instantiatedSavesMenu;
    private GameObject instantiatedDeleteSaveMenu;

    private GameObject pointer;
    private int pointedOptionMenu;
    private bool closing = false;
    private List<TextMeshProUGUI> menuOptions = new List<TextMeshProUGUI>();
    private TextMeshProUGUI title;

    private string saveName;

    private float keyTimeDelayFirst = 20f;
    private float keyTimeDelay = 10f;
    private float keyTimeDelayer = 0;

    void Update()
    {
        if (!closing)
        {
            KeysListener();
            PointOption(pointedOptionMenu, menuOptions);
        }

        if (keyTimeDelayer > 0) keyTimeDelayer -= 75 * Time.unscaledDeltaTime;
    }

    void KeysListener()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiClose);
            CloseMenu();
        }

        if (Input.GetKeyDown(KeyCode.S))
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
            keyTimeDelayer = keyTimeDelayFirst;
        }

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiSelectOption);
            if (pointedOptionMenu == 0)
            {
                ProgressSaving.saveName = saveName;
                closing = true;
                GameParams.Managers.fadeInOutManager.ChangeScene("Loading_Screen");
            }
            else if (pointedOptionMenu == 1)
            {
                instantiatedDeleteSaveMenu = Instantiate(deleteSaveMenuPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, transform.parent);
                instantiatedDeleteSaveMenu.transform.localPosition = new Vector3(0, 0, 0);
                instantiatedDeleteSaveMenu.GetComponent<DeleteSaveMenu>().OpenMenu(pointer, saveName);

                menuOptions.Clear();
                Destroy(gameObject);
            }
        }
    }

    public void OpenMenu(GameObject givenPointer, string givenSaveName)
    {
        pointer = givenPointer;
        saveName = givenSaveName;

        for (int i = 1; i < 3; i++)
        {
            string text = i.ToString();
            menuOptions.Add(transform.Find("Menu").Find("Options").Find(text).GetComponent<TextMeshProUGUI>());
        }

        title = transform.Find("Menu").Find("Title").GetComponent<TextMeshProUGUI>();
        title.text = "Save File " + int.Parse(givenSaveName.Substring(4));

        pointedOptionMenu = 0;
        PointOption(pointedOptionMenu, menuOptions);
    }

    public void CloseMenu()
    {
        instantiatedSavesMenu = Instantiate(savesMenuPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, transform.parent);
        instantiatedSavesMenu.transform.localPosition = new Vector3(0, 0, 0);
        instantiatedSavesMenu.GetComponent<SavesMenu>().OpenMenu(pointer, int.Parse(saveName.Substring(saveName.Length - 1)) - 1);

        menuOptions.Clear();
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