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


        volumeValueText.text = volumeSlider.value.ToString();
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

        if (pointedOptionMenu == 0)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                volumeSlider.value -= 1;
                keyTimeDelayer = keyTimeDelayFirst;
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                volumeSlider.value += 1;
                keyTimeDelayer = keyTimeDelayFirst;
            }



            if (keyTimeDelayer == 0 && Input.GetKey(KeyCode.A))
            {
                volumeSlider.value -= 1;
            }

            if (keyTimeDelayer == 0 && Input.GetKey(KeyCode.D))
            {
                volumeSlider.value += 1;
            }
        }
        else
        {
            if (volumeSlider.value / 100 != soundManager.GetVolume()) soundManager.ChangeVolume(volumeSlider.value / 100);
        }
    }

    public void OpenMenu(GameObject givenPointer)
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        WebCamTexture tex = new WebCamTexture(devices[0].name);
        //webcamVideoDisplay.texture = tex;
        //tex.Play();

        soundManager = FindObjectOfType<SoundManager>();
        volumeSlider.value = soundManager.GetVolume() * 100;


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
        if (volumeSlider.value / 100 != soundManager.GetVolume()) soundManager.ChangeVolume(volumeSlider.value / 100);

        pointer.SetActive(true);
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
            if(i != option)
            {
                allOptions[i].transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                if (i == 0)
                {
                    allOptions[i].transform.Find("Slider").Find("0").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    allOptions[i].transform.Find("Slider").Find("100").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    allOptions[i].transform.Find("Slider").Find("Background").GetComponent<Image>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    allOptions[i].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").Find("Value").GetComponent<TextMeshProUGUI>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                    allOptions[i].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").GetComponent<Image>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                    allOptions[i].transform.Find("Slider").Find("Fill Area").Find("Fill").GetComponent<Image>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                }
                else if (i == 1 || i == 2)
                {
                    allOptions[i].transform.Find("Desc").Find("DoublePointer").gameObject.SetActive(false);

                    allOptions[i].transform.Find("Desc").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    allOptions[i].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").Find("Arrow").GetComponent<RawImage>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    allOptions[i].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").Find("Arrow").GetComponent<RawImage>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                }
            }
        }

        if (option < allOptions.Count)
        {
            allOptions[option].transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);

            if (option == 0)
            {
                pointer.SetActive(true);
                pointer.transform.SetParent(allOptions[option].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").Find("Value"));

                allOptions[option].transform.Find("Slider").Find("0").GetComponent<TextMeshProUGUI>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                allOptions[option].transform.Find("Slider").Find("100").GetComponent<TextMeshProUGUI>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                allOptions[option].transform.Find("Slider").Find("Background").GetComponent<Image>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                allOptions[option].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").Find("Value").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
                allOptions[option].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").GetComponent<Image>().color = new Color(1f, 1f, 1f);
                allOptions[option].transform.Find("Slider").Find("Fill Area").Find("Fill").GetComponent<Image>().color = new Color(1f, 1f, 1f);
            }
            else if (option == 1 || option == 2)
            {
                pointer.SetActive(false);
                allOptions[option].transform.Find("Desc").Find("DoublePointer").gameObject.SetActive(true);

                allOptions[option].transform.Find("Desc").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
                allOptions[option].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").Find("Arrow").GetComponent<RawImage>().color = new Color(1f, 1f, 1f);
                allOptions[option].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").Find("Arrow").GetComponent<RawImage>().color = new Color(1f, 1f, 1f);
            }
        }
    }


    void GetCameras()
    {
        string camerasText = "";
        WebCamDevice[] cameras = WebCamTexture.devices;
        for (int i = 0; i < cameras.Length; i++) camerasText += " ||||| " + cameras[i].name;
        Debug.Log(camerasText);
    }

    void GetMicrophones()
    {
        string microphonesText = "";
        string[] microphones = Microphone.devices;
        for (int i = 0; i < microphones.Length; i++) microphonesText += " ||||| " + microphones[i];
        Debug.Log(microphonesText);
    }

    private void Start()
    {
        GetCameras();
        GetMicrophones();
    }
}