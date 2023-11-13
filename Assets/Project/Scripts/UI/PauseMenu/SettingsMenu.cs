using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public RawImage webcamVideoDisplay;
    public Slider volumeSlider;
    public TextMeshProUGUI volumeValueText;

    private SoundManager soundManager;

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

        if (keyTimeDelayer > 0) keyTimeDelayer--;


        volumeValueText.text = (Mathf.RoundToInt(volumeSlider.value * 100)).ToString();
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

        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            selectSound.Play();
            if (pointedOptionMenu == 0)
            {

            }
            else if (pointedOptionMenu == 1)
            {

            }
            else if (pointedOptionMenu == 2)
            {

            }
        }
        */

        if (pointedOptionMenu == 0)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                volumeSlider.value -= 0.01f;
                keyTimeDelayer = keyTimeDelayFirst;
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                volumeSlider.value += 0.01f;
                keyTimeDelayer = keyTimeDelayFirst;
            }



            if (keyTimeDelayer == 0 && Input.GetKey(KeyCode.A))
            {
                volumeSlider.value -= 0.01f;
            }

            if (keyTimeDelayer == 0 && Input.GetKey(KeyCode.D))
            {
                volumeSlider.value += 0.01f;
            }
        }
        else
        {
            if (volumeSlider.value != soundManager.GetVolume()) soundManager.ChangeVolume(volumeSlider.value);
        }
    }

    public void OpenMenu(GameObject givenPointer)
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        WebCamTexture tex = new WebCamTexture(devices[0].name);
        //webcamVideoDisplay.texture = tex;
        //tex.Play();

        soundManager = FindObjectOfType<SoundManager>();
        volumeSlider.value = soundManager.GetVolume();


        pointer = givenPointer;

        closeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_Close);
        changeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_ChangeOption);
        selectSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_SelectOption);

        for (int i = 1; i < 4; i++)
        {
            string text = i.ToString();
            menuOptions.Add(transform.Find("Menu").Find("Options").Find(text).gameObject);
        }

        pointedOptionMenu = 0;
        PointOption(pointedOptionMenu, menuOptions);
    }

    public void CloseMenu()
    {
        if (volumeSlider.value != soundManager.GetVolume()) soundManager.ChangeVolume(volumeSlider.value);

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
        }

        if (option < allOptions.Count)
        {
            allOptions[option].transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);

            pointer.transform.SetParent(allOptions[option].transform.Find("Name"));
        }
    }
}