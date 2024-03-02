using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsMenu : MonoBehaviour
{
    [Header("Webcam display")]
    public RawImage webcamVideoDisplay;
    public RectTransform webcamVideoDisplayFrame;
    [Header("Volume slider")]
    public Slider volumeSlider;
    public TextMeshProUGUI volumeValueText;
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

    private AudioSource closeSound;
    private AudioSource changeSound;
    private AudioSource selectSound;

    private ScrollRect scrollView;

    private bool speechChangeable = true;

    private float keyTimeDelayFirst = 20f;
    private float keyTimeDelay = 10f;
    private float keyTimeDelayer = 0;

    private int microphoneIndex = 0;
    private int webCamIndex = 0;

    private bool fromMainMenu;
    void Update()
    {
        DisplayMicrophone();
        //DisplayWebCam();

        KeysListener();
        //PointOption(pointedOptionMenu);

        if (keyTimeDelayer > 0) keyTimeDelayer -= 75 * Time.unscaledDeltaTime;
        
        volumeValueText.text = volumeSlider.value.ToString();
        DisplayFPSOrVSyncSlider();

        if(!fromMainMenu) ShowCamera();
    }

    void KeysListener()
    {
        //General controls
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            closeSound.Play();
            CloseMenu();
        }

        //W
        if (Input.GetKeyDown(KeyCode.W))
        {
            changeSound.Play();
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
            changeSound.Play();
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
            changeSound.Play();
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
            changeSound.Play();
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
        if (pointedOptionMenu == 0)
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

        //Microphone
        if (pointedOptionMenu == 6)
        {
            if (Input.GetKeyDown(KeyCode.A) && menuOptions[pointedOptionMenu].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.activeSelf)
            {
                changeSound.Play();
                microphoneIndex += -1;
                ChangeMicrophone();
                PlayerPrefs.SetString("microphoneName", GameSettings.microphoneName);
            }

            if (Input.GetKeyDown(KeyCode.D) && menuOptions[pointedOptionMenu].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.activeSelf)
            {
                changeSound.Play();
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
                changeSound.Play();
                GameSettings.graphicsQuality -= 1;
                DisplayGraphicQuality();
                PlayerPrefs.SetInt("graphicsQuality", (int)GameSettings.graphicsQuality);
            }

            if (Input.GetKeyDown(KeyCode.D) && menuOptions[pointedOptionMenu].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.activeSelf)
            {
                changeSound.Play();
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
                selectSound.Play();
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
                selectSound.Play();
                GameSettings.fullscreen = !GameSettings.fullscreen;
                menuOptions[pointedOptionMenu].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.fullscreen);
                PlayerPrefs.SetInt("fullScreen", (GameSettings.fullscreen ? 1 : 0));
            }
        }

        //Mute music
        if (pointedOptionMenu == 1)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                selectSound.Play();
                GameSettings.muteMusic = !GameSettings.muteMusic;
                menuOptions[pointedOptionMenu].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.muteMusic);
                PlayerPrefs.SetInt("muteMusic", (GameSettings.muteMusic ? 1 : 0));
            }
        }

        //Gesture hints
        if (pointedOptionMenu == 8)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                selectSound.Play();
                GameSettings.gestureHints = !GameSettings.gestureHints;
                menuOptions[pointedOptionMenu].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.gestureHints);
                PlayerPrefs.SetInt("gestureHints", (GameSettings.gestureHints ? 1 : 0));
            }
        }

        //Speech
        if (pointedOptionMenu == 7)
        {
            if (Input.GetKeyDown(KeyCode.Space) && speechChangeable)
            {
                selectSound.Play();
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
        fpsSlider.value = GameSettings.fpsCap;
        vSyncSlider.value = 5 - GameSettings.vSyncCount;
        DisplayFPSText();
        pointer = givenPointer;

        closeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_Close);
        changeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_ChangeOption);
        selectSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_SelectOption);

        for (int i = 1; i < 10; i++)
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

        menuOptions[1].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.muteMusic);
        menuOptions[3].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.vSync);
        menuOptions[5].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.fullscreen);
        menuOptions[7].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.useSpeech);
        menuOptions[8].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.gestureHints);

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
        Destroy(closeSound.gameObject, closeSound.clip.length);
        Destroy(changeSound.gameObject, changeSound.clip.length);
        Destroy(selectSound.gameObject, selectSound.clip.length);
        Destroy(gameObject);
    }

    void PointOption(int option)
    {
        for (int i = 0; i < menuOptions.Count; i++)
        {
            if (i != option)
            {
                if (i == 0 || i == 2)
                {
                    menuOptions[i].transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    if (i == 0)
                    {
                        menuOptions[i].transform.Find("Slider").Find("0").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                        menuOptions[i].transform.Find("Slider").Find("100").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    }
                    menuOptions[i].transform.Find("Slider").Find("Background").GetComponent<Image>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    menuOptions[i].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").Find("Value").GetComponent<TextMeshProUGUI>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                    menuOptions[i].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").GetComponent<Image>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                    menuOptions[i].transform.Find("Slider").Find("Fill Area").Find("Fill").GetComponent<Image>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                }
                else if (i == 4 || i == 6) //here webcam
                {

                    menuOptions[i].transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    menuOptions[i].transform.Find("Desc").Find("DoublePointer").gameObject.SetActive(false);

                    menuOptions[i].transform.Find("Desc").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    menuOptions[i].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").Find("Arrow").GetComponent<RawImage>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    menuOptions[i].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").Find("Arrow").GetComponent<RawImage>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                }
                else if (i == 1 || i == 3 || i == 5 || i == 7 || option == 8)
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
            if (option == 0 || option == 2)
            {
                menuOptions[option].transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
                pointer.transform.SetParent(menuOptions[option].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").Find("Value"));
                pointer.SetActive(true);

                if (option == 0)
                {
                    menuOptions[option].transform.Find("Slider").Find("0").GetComponent<TextMeshProUGUI>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                    menuOptions[option].transform.Find("Slider").Find("100").GetComponent<TextMeshProUGUI>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                }
                menuOptions[option].transform.Find("Slider").Find("Background").GetComponent<Image>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                menuOptions[option].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").Find("Value").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
                menuOptions[option].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").GetComponent<Image>().color = new Color(1f, 1f, 1f);
                menuOptions[option].transform.Find("Slider").Find("Fill Area").Find("Fill").GetComponent<Image>().color = new Color(1f, 1f, 1f);
            }
            else if (option == 4 || option == 6) //here webcam
            {
                menuOptions[option].transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);

                pointer.SetActive(false);
                menuOptions[option].transform.Find("Desc").Find("DoublePointer").gameObject.SetActive(true);

                menuOptions[option].transform.Find("Desc").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
                menuOptions[option].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").Find("Arrow").GetComponent<RawImage>().color = new Color(1f, 1f, 1f);
                menuOptions[option].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").Find("Arrow").GetComponent<RawImage>().color = new Color(1f, 1f, 1f);
            }
            else if (option == 1 || option == 3 || option == 5 || option == 7 || option == 8)
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
        menuOptions[4].transform.Find("Desc").GetComponent<TextMeshProUGUI>().text = GameSettings.graphicsQuality.ToString();
        if (GameSettings.graphicsQuality == GameSettings.GraphicsQuality.Low)
        {
            menuOptions[4].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(false);
            menuOptions[4].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(true);
        }
        else if (GameSettings.graphicsQuality == GameSettings.GraphicsQuality.Medium)
        {
            menuOptions[4].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(true);
            menuOptions[4].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(true);
        }
        else if (GameSettings.graphicsQuality == GameSettings.GraphicsQuality.High)
        {
            menuOptions[4].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(true);
            menuOptions[4].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(false);
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

        menuOptions[6].transform.Find("Desc").GetComponent<TextMeshProUGUI>().text = GameSettings.microphoneName;

        if (microphoneIndex > 0) menuOptions[6].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(true);
        else menuOptions[6].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(false);

        if (microphoneIndex < microphones.Length - 1) menuOptions[6].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(true);
        else menuOptions[6].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(false);

        if (microphoneIndex == -1)
        {
            menuOptions[6].transform.Find("Desc").GetComponent<TextMeshProUGUI>().text = "no Microphone detected";
            menuOptions[6].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(false);
            menuOptions[6].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(false);

            menuOptions[7].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(false);
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
        if (vSyncSlider.value == 1) vSyncValueText.text = ((int)(Screen.currentResolution.refreshRate / 4)).ToString();
        else if (vSyncSlider.value == 2) vSyncValueText.text = ((int)(Screen.currentResolution.refreshRate / 3)).ToString();
        else if (vSyncSlider.value == 3) vSyncValueText.text = ((int)(Screen.currentResolution.refreshRate / 2)).ToString();
        else if (vSyncSlider.value == 4) vSyncValueText.text = ((int)(Screen.currentResolution.refreshRate / 1)).ToString();
        else vSyncValueText.text = "Error";
    }

    void ShowCamera()
    {
        webcamVideoDisplayFrame.gameObject.SetActive(true);

        try
        {
            using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("magehand_video_unity"))
            {
                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {
                    BinaryReader reader = new BinaryReader(stream);
                    byte[] frameBuffer = reader.ReadBytes(480 * 640 * 3);

                    int width = 640;
                    int height = 480;

                    Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
                    texture.LoadRawTextureData(frameBuffer);
                    texture.Apply();

                    float scale = (float)texture.height / (float)texture.width;
                    webcamVideoDisplayFrame.sizeDelta = new Vector2(webcamVideoDisplayFrame.sizeDelta.x, webcamVideoDisplayFrame.sizeDelta.x * scale);

                    webcamVideoDisplay.gameObject.SetActive(true);

                    webcamVideoDisplay.texture = texture;

                }
            }
        }
        catch (FileNotFoundException)
        {
            webcamVideoDisplay.gameObject.SetActive(false);
            Debug.Log("video_unity doesn't exist");
        }
    }
}