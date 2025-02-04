using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using FMODUnity;

public class SettingsMenu : MonoBehaviour
{
    [Header("WebCam Display")]
    public WebCamDisplay webCamDisplay;
    [Header("Volume slider")]
    public Slider volumeSlider;
    public TextMeshProUGUI volumeValueText;
    [Header("Brightness slider")]
    public Slider brightnessSlider;
    public TextMeshProUGUI brightnessValueText;
    public float maxBrightnessValue = 4f;
    [Header("FPS slider")]
    public Slider fpsSlider;
    public TextMeshProUGUI fpsValueText;
    [Header("vSync slider")]
    public Slider vSyncSlider;
    public TextMeshProUGUI vSyncValueText;
    [Header("Settings info")]
    public TextMeshProUGUI infoText;
    public List<string> infoTexts = new List<string>();
    private GameObject pointer;
    private int pointedOptionMenu;
    private List<GameObject> menuOptions = new List<GameObject>();

    private ScrollRect scrollView;

    private bool speechChangeable = true;

    private float keyTimeDelayFirst = 20f;
    private float keyTimeDelay = 10f;
    private float keyTimeDelayer = 0;

    private int microphoneIndex = 0;
    private int webCamIndex = 0;

    //option indexes
    private int voluOptIndx = 0;
    private int muteOptIndx = 1;
    private int framOptIndx = 2;
    private int vsynOptIndx = 3;
    private int grapOptIndx = 4;
    private int fullOptIndx = 5;
    private int brigOptIndx = 6;
    private int micrOptIndx = 7;
    private int speeOptIndx = 8;
    private int gestOptIndx = 9;

    private bool fromMainMenu;
    void Update()
    {
        DisplayMicrophone();
        //DisplayWebCam();

        KeysListener();
        //PointOption(pointedOptionMenu);

        if (keyTimeDelayer > 0) keyTimeDelayer -= 75 * Time.unscaledDeltaTime;
        
        volumeValueText.text = volumeSlider.value.ToString();
        brightnessValueText.text = brightnessSlider.value.ToString();
        DisplayFPSOrVSyncSlider();

        if(!fromMainMenu) webCamDisplay.ShowCamera();
    }

    void KeysListener()
    {
        //General controls
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiClose);
            CloseMenu();
        }

        //W
        if (Input.GetKeyDown(KeyCode.W))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiChangeOption);
            if (pointedOptionMenu > 0)
            {
                pointedOptionMenu--;
                scrollView.verticalNormalizedPosition += 1f / (menuOptions.Count - 1);
            }
            else
            {
                pointedOptionMenu = menuOptions.Count - 1;
                scrollView.verticalNormalizedPosition = 0f;
            }
            keyTimeDelayer = keyTimeDelayFirst;
            PointOption(pointedOptionMenu);
        }
        //S
        if (Input.GetKeyDown(KeyCode.S))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiChangeOption);
            if (pointedOptionMenu < menuOptions.Count - 1)
            {
                pointedOptionMenu++;
                scrollView.verticalNormalizedPosition += -1f / (menuOptions.Count - 1);
            }
            else
            {
                pointedOptionMenu = 0;
                scrollView.verticalNormalizedPosition = 1f;
            }
            keyTimeDelayer = keyTimeDelayFirst;
            PointOption(pointedOptionMenu);
        }
        //W hold
        if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.W))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiChangeOption);
            if (pointedOptionMenu > 0)
            {
                pointedOptionMenu--;
                scrollView.verticalNormalizedPosition += 1f / (menuOptions.Count - 1);
            }
            else
            {
                pointedOptionMenu = menuOptions.Count - 1;
                scrollView.verticalNormalizedPosition = 0f;
            }
            keyTimeDelayer = keyTimeDelay;
            PointOption(pointedOptionMenu);
        }
        //S hold
        if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.S))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiChangeOption);
            if (pointedOptionMenu < menuOptions.Count - 1)
            {
                pointedOptionMenu++;
                scrollView.verticalNormalizedPosition += -1f / (menuOptions.Count - 1); 
            }
            else
            {
                pointedOptionMenu = 0;
                scrollView.verticalNormalizedPosition = 1f;
            }
            keyTimeDelayer = keyTimeDelay;
            PointOption(pointedOptionMenu);
        }

        //Volume
        if (pointedOptionMenu == voluOptIndx)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                volumeSlider.value -= 1;
                keyTimeDelayer = keyTimeDelayFirst;
                GameSettings.soundVolume = volumeSlider.value / 100;
                PlayerPrefs.SetFloat("soundVolume", GameSettings.soundVolume);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                volumeSlider.value += 1;
                keyTimeDelayer = keyTimeDelayFirst;
                GameSettings.soundVolume = volumeSlider.value / 100;
                PlayerPrefs.SetFloat("soundVolume", GameSettings.soundVolume);
            }

            if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.A))
            {
                volumeSlider.value -= 1;
                keyTimeDelayer = 1f;
                GameSettings.soundVolume = volumeSlider.value / 100;
                PlayerPrefs.SetFloat("soundVolume", GameSettings.soundVolume);
            }

            if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.D))
            {
                volumeSlider.value += 1;
                keyTimeDelayer = 1f;
                GameSettings.soundVolume = volumeSlider.value / 100;
                PlayerPrefs.SetFloat("soundVolume", GameSettings.soundVolume);
            }
        }

        //Brightness
        if (pointedOptionMenu == brigOptIndx)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                brightnessSlider.value -= 1;
                keyTimeDelayer = keyTimeDelayFirst;
                GameSettings.brightness = brightnessSlider.value * (maxBrightnessValue / 100);
                PlayerPrefs.SetFloat("brightness", GameSettings.brightness);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                brightnessSlider.value += 1;
                keyTimeDelayer = keyTimeDelayFirst;
                GameSettings.brightness = brightnessSlider.value * (maxBrightnessValue / 100);
                PlayerPrefs.SetFloat("brightness", GameSettings.brightness);
            }

            if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.A))
            {
                brightnessSlider.value -= 1;
                keyTimeDelayer = 1f;
                GameSettings.brightness = brightnessSlider.value * (maxBrightnessValue / 100);
                PlayerPrefs.SetFloat("brightness", GameSettings.brightness);
            }

            if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.D))
            {
                brightnessSlider.value += 1;
                keyTimeDelayer = 1f;
                GameSettings.brightness = brightnessSlider.value * (maxBrightnessValue / 100);
                PlayerPrefs.SetFloat("brightness", GameSettings.brightness);
            }
        }

        //Microphone
        if (pointedOptionMenu == micrOptIndx)
        {
            if (Input.GetKeyDown(KeyCode.A) && menuOptions[pointedOptionMenu].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.activeSelf)
            {
                RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiChangeOption);
                microphoneIndex += -1;
                ChangeMicrophone();
                PlayerPrefs.SetString("microphoneName", GameSettings.microphoneName);
            }

            if (Input.GetKeyDown(KeyCode.D) && menuOptions[pointedOptionMenu].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.activeSelf)
            {
                RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiChangeOption);
                microphoneIndex += 1;
                ChangeMicrophone();
                PlayerPrefs.SetString("microphoneName", GameSettings.microphoneName);
            }
        }

        /*
        //Webcam
        if (pointedOptionMenu == 8)
        {
            if (Input.GetKeyDown(KeyCode.A) && menuOptions[pointedOptionMenu].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.activeSelf)
            {
                changeSound.Play();
                webCamIndex += -1;
                ChangeWebCam();
                PlayerPrefs.SetString("webCamName", GameSettings.webCamName);
            }

            if (Input.GetKeyDown(KeyCode.D) && menuOptions[pointedOptionMenu].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.activeSelf)
            {
                changeSound.Play();
                webCamIndex += 1;
                ChangeWebCam();
                PlayerPrefs.SetString("webCamName", GameSettings.webCamName);
            }
        }
        */

        //FPS cap
        if (pointedOptionMenu == 2 && !GameSettings.vSync)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                fpsSlider.value -= 1;
                keyTimeDelayer = keyTimeDelayFirst;
                GameSettings.fpsCap = (int)fpsSlider.value;
                PlayerPrefs.SetInt("fpsCap", GameSettings.fpsCap);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                fpsSlider.value += 1;
                keyTimeDelayer = keyTimeDelayFirst;
                GameSettings.fpsCap = (int)fpsSlider.value;
                PlayerPrefs.SetInt("fpsCap", GameSettings.fpsCap);
            }


            if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.A))
            {
                fpsSlider.value -= 1;
                keyTimeDelayer = keyTimeDelay;
                GameSettings.fpsCap = (int)fpsSlider.value;
                PlayerPrefs.SetInt("fpsCap", GameSettings.fpsCap);
            }

            if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.D))
            {
                fpsSlider.value += 1;
                keyTimeDelayer = keyTimeDelay;
                GameSettings.fpsCap = (int)fpsSlider.value;
                PlayerPrefs.SetInt("fpsCap", GameSettings.fpsCap);
            }
        }

        //vSyncCount
        if (pointedOptionMenu == 2 && GameSettings.vSync)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                vSyncSlider.value -= 1;
                keyTimeDelayer = keyTimeDelayFirst;
                GameSettings.vSyncCount = 5 - (int)vSyncSlider.value;
                PlayerPrefs.SetInt("vSyncCount", GameSettings.vSyncCount);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                vSyncSlider.value += 1;
                keyTimeDelayer = keyTimeDelayFirst;
                GameSettings.vSyncCount = 5 - (int)vSyncSlider.value;
                PlayerPrefs.SetInt("vSyncCount", GameSettings.vSyncCount);
            }


            if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.A))
            {
                vSyncSlider.value -= 1;
                keyTimeDelayer = keyTimeDelay;
                GameSettings.vSyncCount = 5 - (int)vSyncSlider.value;
                PlayerPrefs.SetInt("vSyncCount", GameSettings.vSyncCount);
            }

            if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.D))
            {
                vSyncSlider.value += 1;
                keyTimeDelayer = keyTimeDelay;
                GameSettings.vSyncCount = 5 - (int)vSyncSlider.value;
                PlayerPrefs.SetInt("vSyncCount", GameSettings.vSyncCount);
            }
        }

        //Graphic quality
        if (pointedOptionMenu == 4)
        {
            if (Input.GetKeyDown(KeyCode.A) && menuOptions[pointedOptionMenu].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.activeSelf)
            {
                RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiChangeOption);
                GameSettings.graphicsQuality -= 1;
                DisplayGraphicQuality();
                PlayerPrefs.SetInt("graphicsQuality", (int)GameSettings.graphicsQuality);
            }

            if (Input.GetKeyDown(KeyCode.D) && menuOptions[pointedOptionMenu].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.activeSelf)
            {
                RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiChangeOption);
                GameSettings.graphicsQuality += 1;
                DisplayGraphicQuality();
                PlayerPrefs.SetInt("graphicsQuality", (int)GameSettings.graphicsQuality);
            }
        }

        //vSync
        if (pointedOptionMenu == 3)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiSelectOption);
                GameSettings.vSync = !GameSettings.vSync;
                menuOptions[pointedOptionMenu].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.vSync);
                PlayerPrefs.SetInt("vSync", (GameSettings.vSync ? 1 : 0));
            }
        }

        //Fullscreen
        if (pointedOptionMenu == 5)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiSelectOption);
                GameSettings.fullscreen = !GameSettings.fullscreen;
                menuOptions[pointedOptionMenu].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.fullscreen);
                PlayerPrefs.SetInt("fullScreen", (GameSettings.fullscreen ? 1 : 0));
            }
        }

        //Mute music
        if (pointedOptionMenu == muteOptIndx)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiSelectOption);
                GameSettings.muteMusic = !GameSettings.muteMusic;
                menuOptions[pointedOptionMenu].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.muteMusic);
                PlayerPrefs.SetInt("muteMusic", (GameSettings.muteMusic ? 1 : 0));
            }
        }

        //Gesture hints
        if (pointedOptionMenu == 9)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiSelectOption);
                GameSettings.gestureHints = !GameSettings.gestureHints;
                menuOptions[pointedOptionMenu].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.gestureHints);
                PlayerPrefs.SetInt("gestureHints", (GameSettings.gestureHints ? 1 : 0));
            }
        }

        //Speech
        if (pointedOptionMenu == 8)
        {
            if (Input.GetKeyDown(KeyCode.Space) && speechChangeable)
            {
                RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiSelectOption);
                GameSettings.useSpeech = !GameSettings.useSpeech;
                menuOptions[pointedOptionMenu].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.useSpeech);
                PlayerPrefs.SetInt("useSpeach", (GameSettings.useSpeech ? 1 : 0));
            }
        }


    }
    public void OpenMenu(GameObject givenPointer, bool givenFromMainMenu = false)
    {
        fromMainMenu = givenFromMainMenu;
        volumeSlider.value = GameSettings.soundVolume * 100;
        brightnessSlider.value = GameSettings.brightness * (100 / maxBrightnessValue);
        fpsSlider.value = GameSettings.fpsCap;
        vSyncSlider.value = 5 - GameSettings.vSyncCount;
        DisplayFPSText();
        pointer = givenPointer;

        for (int i = 1; i <= 10; i++)
        {
            string text = i.ToString();
            menuOptions.Add(transform.Find("Menu").Find("ScrollRect").Find("Content").Find(text).gameObject);
        }

        scrollView = transform.Find("Menu").Find("ScrollRect").GetComponent<ScrollRect>();
        scrollView.verticalNormalizedPosition = 1f;


        DisplayMicrophone();
        //DisplayWebCam();


        DisplayGraphicQuality();

        DisplayFPSOrVSyncSlider();

        menuOptions[muteOptIndx].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.muteMusic);
        menuOptions[vsynOptIndx].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.vSync);
        menuOptions[fullOptIndx].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.fullscreen);
        menuOptions[speeOptIndx].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.useSpeech);
        menuOptions[gestOptIndx].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.gestureHints);

        if (fromMainMenu)
        {
            transform.parent.Find("Title").gameObject.SetActive(false);
            transform.Find("Info").Find("BlackoutBackground").gameObject.SetActive(true);
        }

        pointedOptionMenu = 0;
        PointOption(pointedOptionMenu);
        StartCoroutine(WaitOneFrameToPoint());
    }

    IEnumerator WaitOneFrameToPoint()
    {
        yield return 0;
        PointOption(pointedOptionMenu);
    }

    public void CloseMenu()
    {
        pointer.transform.SetParent(transform.parent.transform.Find("Menu"));
        pointer.SetActive(true);
        menuOptions.Clear();
        transform.parent.transform.Find("Menu").gameObject.SetActive(true);
        if (fromMainMenu) transform.parent.Find("Title").gameObject.SetActive(true);
        Destroy(gameObject);
    }

    void PointOption(int option)
    {
        for (int i = 0; i < menuOptions.Count; i++)
        {
            if (i != option)
            {
                //Sliders
                if (i == voluOptIndx || i == framOptIndx || i == brigOptIndx)
                {
                    menuOptions[i].transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    if (i == voluOptIndx)
                    {
                        menuOptions[i].transform.Find("Slider").Find("0").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                        menuOptions[i].transform.Find("Slider").Find("100").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    }
                    menuOptions[i].transform.Find("Slider").Find("Background").GetComponent<Image>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    menuOptions[i].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").Find("Value").GetComponent<TextMeshProUGUI>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                    menuOptions[i].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").GetComponent<Image>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                    menuOptions[i].transform.Find("Slider").Find("Fill Area").Find("Fill").GetComponent<Image>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                }
                //Choices
                else if (i == grapOptIndx || i == micrOptIndx) //here webcam
                {

                    menuOptions[i].transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    menuOptions[i].transform.Find("Desc").Find("DoublePointer").gameObject.SetActive(false);

                    menuOptions[i].transform.Find("Desc").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    menuOptions[i].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").Find("Arrow").GetComponent<RawImage>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    menuOptions[i].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").Find("Arrow").GetComponent<RawImage>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                }
                //Checkboxes
                else if (i == muteOptIndx || i == vsynOptIndx || i == fullOptIndx || i == speeOptIndx || i == gestOptIndx)
                {
                    menuOptions[i].GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    menuOptions[i].transform.Find("CheckBackground").GetComponent<RawImage>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    menuOptions[i].transform.Find("CheckBackground").Find("Checker").GetComponent<RawImage>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                }
            }
        }

        if (option < menuOptions.Count)
        {
            infoText.text = infoTexts[option];
            //Sliders
            if (option == voluOptIndx || option == framOptIndx || option == brigOptIndx)
            {
                menuOptions[option].transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
                pointer.transform.SetParent(menuOptions[option].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").Find("Value"));
                pointer.SetActive(true);

                if (option == voluOptIndx)
                {
                    menuOptions[option].transform.Find("Slider").Find("0").GetComponent<TextMeshProUGUI>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                    menuOptions[option].transform.Find("Slider").Find("100").GetComponent<TextMeshProUGUI>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                }
                menuOptions[option].transform.Find("Slider").Find("Background").GetComponent<Image>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                menuOptions[option].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").Find("Value").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
                menuOptions[option].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").GetComponent<Image>().color = new Color(1f, 1f, 1f);
                menuOptions[option].transform.Find("Slider").Find("Fill Area").Find("Fill").GetComponent<Image>().color = new Color(1f, 1f, 1f);
            }
            //Choices
            else if (option == grapOptIndx || option == micrOptIndx) //here webcam
            {
                menuOptions[option].transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);

                pointer.SetActive(false);
                menuOptions[option].transform.Find("Desc").Find("DoublePointer").gameObject.SetActive(true);

                menuOptions[option].transform.Find("Desc").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
                menuOptions[option].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").Find("Arrow").GetComponent<RawImage>().color = new Color(1f, 1f, 1f);
                menuOptions[option].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").Find("Arrow").GetComponent<RawImage>().color = new Color(1f, 1f, 1f);
            }
            //Checkboxes
            else if (option == muteOptIndx || option == vsynOptIndx || option == fullOptIndx 
                || option == speeOptIndx || option == gestOptIndx)
            {
                pointer.transform.SetParent(menuOptions[option].transform);
                pointer.SetActive(true);

                menuOptions[option].GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
                menuOptions[option].transform.Find("CheckBackground").GetComponent<RawImage>().color = new Color(1f, 1f, 1f);
                menuOptions[option].transform.Find("CheckBackground").Find("Checker").GetComponent<RawImage>().color = new Color(1f, 1f, 1f);
            }
        }
    }
    void DisplayGraphicQuality()
    {
        menuOptions[grapOptIndx].transform.Find("Desc").GetComponent<TextMeshProUGUI>().text = GameSettings.graphicsQuality.ToString();
        if (GameSettings.graphicsQuality == GameSettings.GraphicsQuality.Low)
        {
            menuOptions[grapOptIndx].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(false);
            menuOptions[grapOptIndx].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(true);
        }
        else if (GameSettings.graphicsQuality == GameSettings.GraphicsQuality.Medium)
        {
            menuOptions[grapOptIndx].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(true);
            menuOptions[grapOptIndx].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(true);
        }
        else if (GameSettings.graphicsQuality == GameSettings.GraphicsQuality.High)
        {
            menuOptions[grapOptIndx].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(true);
            menuOptions[grapOptIndx].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(false);
        }
    }

    void DisplayMicrophone()
    {
        string[] microphones = Microphone.devices;
        microphoneIndex = -1;
        for (int i = 0; i < microphones.Length; i++)
        {
            if (microphones[i] == GameSettings.microphoneName)
            {
                microphoneIndex = i;
                break;
            }
        }

        menuOptions[micrOptIndx].transform.Find("Desc").GetComponent<TextMeshProUGUI>().text = GameSettings.microphoneName;

        if (microphoneIndex > 0) menuOptions[micrOptIndx].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(true);
        else menuOptions[micrOptIndx].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(false);

        if (microphoneIndex < microphones.Length - 1) menuOptions[micrOptIndx].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(true);
        else menuOptions[micrOptIndx].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(false);

        if (microphoneIndex == -1)
        {
            menuOptions[micrOptIndx].transform.Find("Desc").GetComponent<TextMeshProUGUI>().text = "no Microphone detected";
            menuOptions[micrOptIndx].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(false);
            menuOptions[micrOptIndx].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(false);

            menuOptions[speeOptIndx].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(false);
            speechChangeable = false;
        }
        else speechChangeable = true;
    }
    /*
    void DisplayWebCam()
    {
        WebCamDevice[] cameras = WebCamTexture.devices;
        webCamIndex = -1;
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i].name == GameSettings.webCamName)
            {
                webCamIndex = i;
                break;
            }
        }

        menuOptions[8].transform.Find("Desc").GetComponent<TextMeshProUGUI>().text = GameSettings.webCamName;
        if (webCamIndex > 0) menuOptions[8].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(true);
        else menuOptions[8].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(false);

        if (webCamIndex < cameras.Length - 1) menuOptions[8].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(true);
        else menuOptions[8].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(false);

        if (webCamIndex == -1)
        {
            menuOptions[8].transform.Find("Desc").GetComponent<TextMeshProUGUI>().text = "no Camera detected";
            menuOptions[8].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(false);
            menuOptions[8].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(false);
        }

    }
    */

    void ChangeMicrophone()
    {
        string[] microphones = Microphone.devices;
        if (microphoneIndex >= 0 && microphoneIndex < microphones.Length) GameSettings.microphoneName = microphones[microphoneIndex];
        else
        {
            microphoneIndex = 0;
            GameSettings.microphoneName = microphones[microphoneIndex];
        }
    }

    void ChangeWebCam()
    {
        WebCamDevice[] cameras = WebCamTexture.devices;
        if (webCamIndex >= 0 && webCamIndex < cameras.Length) GameSettings.webCamName = cameras[webCamIndex].name;
        else
        {
            webCamIndex = 0;
            GameSettings.webCamName = cameras[webCamIndex].name;
        }
    }

    void DisplayFPSOrVSyncSlider()
    {
        if (GameSettings.vSync)
        {
            DisplayVSyncText();
            vSyncSlider.gameObject.name = "Slider";
            fpsSlider.gameObject.name = "X";
            vSyncSlider.gameObject.SetActive(true);
            fpsSlider.gameObject.SetActive(false);
        }
        else
        {
            DisplayFPSText();
            vSyncSlider.gameObject.name = "X";
            fpsSlider.gameObject.name = "Slider";
            vSyncSlider.gameObject.SetActive(false);
            fpsSlider.gameObject.gameObject.SetActive(true);
        }
    }

    void DisplayFPSText()
    {
        if (fpsSlider.value == 0) fpsValueText.text = "30";
        else if (fpsSlider.value == 1) fpsValueText.text = "60";
        else if (fpsSlider.value == 2) fpsValueText.text = "120";
        else if (fpsSlider.value == 3) fpsValueText.text = "144";
        else if (fpsSlider.value == 4) fpsValueText.text = "160";
        else if (fpsSlider.value == 5) fpsValueText.text = "165";
        else if (fpsSlider.value == 6) fpsValueText.text = "180";
        else if (fpsSlider.value == 7) fpsValueText.text = "200";
        else if (fpsSlider.value == 8) fpsValueText.text = "240";
        else if (fpsSlider.value == 9) fpsValueText.text = "360";
        else if (fpsSlider.value == 10) fpsValueText.text = "No cap";
        else fpsValueText.text = "Error";
    }

    void DisplayVSyncText()
    {
        if (vSyncSlider.value == 1) vSyncValueText.text = ((int)(Screen.currentResolution.refreshRateRatio.numerator / 4)).ToString();
        else if (vSyncSlider.value == 2) vSyncValueText.text = ((int)(Screen.currentResolution.refreshRateRatio.numerator / 3)).ToString();
        else if (vSyncSlider.value == 3) vSyncValueText.text = ((int)(Screen.currentResolution.refreshRateRatio.numerator / 2)).ToString();
        else if (vSyncSlider.value == 4) vSyncValueText.text = ((int)(Screen.currentResolution.refreshRateRatio.numerator / 1)).ToString();
        else vSyncValueText.text = "Error";
    }
}