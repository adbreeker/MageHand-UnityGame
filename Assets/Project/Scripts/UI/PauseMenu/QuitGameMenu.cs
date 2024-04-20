using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuitGameMenu : MonoBehaviour
{
    private GameObject pointer;
    private int pointedOptionMenu;
    private bool closing = false;
    private List<TextMeshProUGUI> menuOptions = new List<TextMeshProUGUI>();

    private AudioSource closeSound;
    private AudioSource changeSound;
    private AudioSource selectSound;

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            selectSound.Play();
            if (pointedOptionMenu == 0)
            {
                closing = true;
                GameParams.Managers.fadeInOutManager.CloseGame();
            }
            else if (pointedOptionMenu == 1)
            {
                CloseMenu();
            }
        }
    }

    public void OpenMenu(GameObject givenPointer)
    {
        pointer = givenPointer;

        closeSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.UI_Close);
        changeSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.UI_ChangeOption);
        selectSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.UI_SelectOption);

        for (int i = 1; i < 3; i++)
        {
            string text = i.ToString();
            menuOptions.Add(transform.Find("Menu").Find("Options").Find(text).GetComponent<TextMeshProUGUI>());
        }

        pointedOptionMenu = 0;
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