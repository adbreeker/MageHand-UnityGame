using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private WebCamTexture tex;

    private GameObject pointer;
    private int pointedOptionMenu;
    private List<GameObject> menuOptions = new List<GameObject>();

    private AudioSource closeSound;
    private AudioSource changeSound;
    private AudioSource selectSound;

    private float keyTimeDelayFirst = 20f;
    private float keyTimeDelay = 10f;
    private float keyTimeDelayer = 0;

    private int microphoneIndex = 0;
    private int webCamIndex = 0;
    void Update()
    {
        KeysListener();
        PointOption(pointedOptionMenu, menuOptions);

        if (keyTimeDelayer > 0) keyTimeDelayer -= 75 * Time.unscaledDeltaTime;
        
        volumeValueText.text = volumeSlider.value.ToString();
        DisplayFPSOrVSyncSlider();
    }

    void KeysListener()
    {
        //General controls
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

        //Volume
        if (pointedOptionMenu == 0)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                volumeSlider.value -= 1;
                keyTimeDelayer = keyTimeDelayFirst;
                GameSettings.soundVolume = volumeSlider.value / 100;
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                volumeSlider.value += 1;
                keyTimeDelayer = keyTimeDelayFirst;
                GameSettings.soundVolume = volumeSlider.value / 100;
            }

            if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.A))
            {
                volumeSlider.value -= 1;
                keyTimeDelayer = 1f;
                GameSettings.soundVolume = volumeSlider.value / 100;
            }

            if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.D))
            {
                volumeSlider.value += 1;
                keyTimeDelayer = 1f;
                GameSettings.soundVolume = volumeSlider.value / 100;
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
                DisplayMicrophone();
            }

            if (Input.GetKeyDown(KeyCode.D) && menuOptions[pointedOptionMenu].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.activeSelf)
            {
                changeSound.Play();
                microphoneIndex += 1;
                ChangeMicrophone();
                DisplayMicrophone();
            }
        }

        //Webcam
        if (pointedOptionMenu == 7)
        {
            if (Input.GetKeyDown(KeyCode.A) && menuOptions[pointedOptionMenu].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.activeSelf)
            {
                changeSound.Play();
                webCamIndex += -1;
                ChangeWebCam();
                DisplayWebCam();
            }

            if (Input.GetKeyDown(KeyCode.D) && menuOptions[pointedOptionMenu].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.activeSelf)
            {
                changeSound.Play();
                webCamIndex += 1;
                ChangeWebCam();
                DisplayWebCam();
            }
        }

        //FPS cap
        if (pointedOptionMenu == 2 && !GameSettings.vSync)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                fpsSlider.value -= 1;
                keyTimeDelayer = keyTimeDelayFirst;
                GameSettings.fpsCap = (int)fpsSlider.value;
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                fpsSlider.value += 1;
                keyTimeDelayer = keyTimeDelayFirst;
                GameSettings.fpsCap = (int)fpsSlider.value;
            }


            if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.A))
            {
                fpsSlider.value -= 1;
                keyTimeDelayer = keyTimeDelay;
                GameSettings.fpsCap = (int)fpsSlider.value;
            }

            if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.D))
            {
                fpsSlider.value += 1;
                keyTimeDelayer = keyTimeDelay;
                GameSettings.fpsCap = (int)fpsSlider.value;
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
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                vSyncSlider.value += 1;
                keyTimeDelayer = keyTimeDelayFirst;
                GameSettings.vSyncCount = 5 - (int)vSyncSlider.value;
            }


            if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.A))
            {
                vSyncSlider.value -= 1;
                keyTimeDelayer = keyTimeDelay;
                GameSettings.vSyncCount = 5 - (int)vSyncSlider.value;
            }

            if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.D))
            {
                vSyncSlider.value += 1;
                keyTimeDelayer = keyTimeDelay;
                GameSettings.vSyncCount = 5 - (int)vSyncSlider.value;
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
            }

            if (Input.GetKeyDown(KeyCode.D) && menuOptions[pointedOptionMenu].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.activeSelf)
            {
                changeSound.Play();
                GameSettings.graphicsQuality += 1;
                DisplayGraphicQuality();
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
            }
        }
    }
    public void OpenMenu(GameObject givenPointer)
    {
        volumeSlider.value = GameSettings.soundVolume * 100;
        fpsSlider.value = GameSettings.fpsCap;
        vSyncSlider.value = 5 - GameSettings.vSyncCount;
        DisplayFPSText();
        pointer = givenPointer;

        closeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_Close);
        changeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_ChangeOption);
        selectSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_SelectOption);

        for (int i = 1; i < 9; i++)
        {
            string text = i.ToString();
            menuOptions.Add(transform.Find("Menu").Find("Options").Find(text).gameObject);
        }

        string[] microphones = Microphone.devices;
        for (int i = 0; i < microphones.Length; i++)
        {
            if (microphones[i] == GameSettings.microphoneName)
            {
                microphoneIndex = i;
                break;
            }
        }
        DisplayMicrophone();

        WebCamDevice[] cameras = WebCamTexture.devices;
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i].name == GameSettings.webCamName)
            {
                webCamIndex = i;
                break;
            }
        }
        DisplayWebCam();

        DisplayGraphicQuality();

        DisplayFPSOrVSyncSlider();

        menuOptions[1].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.muteMusic);
        menuOptions[3].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.vSync);
        menuOptions[5].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.fullscreen);

        pointedOptionMenu = 0;
        PointOption(pointedOptionMenu, menuOptions);
    }

    public void CloseMenu()
    {
        if (tex != null) tex.Stop();

        pointer.transform.SetParent(transform.parent.transform.Find("Menu"));
        pointer.SetActive(true);
        menuOptions.Clear();
        transform.parent.transform.Find("Menu").gameObject.SetActive(true);
        Destroy(closeSound.gameObject, closeSound.clip.length);
        Destroy(changeSound.gameObject, changeSound.clip.length);
        Destroy(selectSound.gameObject, selectSound.clip.length);
        Destroy(gameObject);
    }

    void PointOption(int option, List<GameObject> allOptions)
    {
        webcamVideoDisplayFrame.gameObject.SetActive(false);
        for (int i = 0; i < allOptions.Count; i++)
        {
            if (i != option)
            {
                if (i == 0 || i == 2)
                {
                    allOptions[i].transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    if (i == 0)
                    {
                        allOptions[i].transform.Find("Slider").Find("0").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                        allOptions[i].transform.Find("Slider").Find("100").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    }
                    allOptions[i].transform.Find("Slider").Find("Background").GetComponent<Image>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    allOptions[i].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").Find("Value").GetComponent<TextMeshProUGUI>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                    allOptions[i].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").GetComponent<Image>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                    allOptions[i].transform.Find("Slider").Find("Fill Area").Find("Fill").GetComponent<Image>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                }
                else if (i == 4 || i == 6 || i == 7)
                {
   
                    allOptions[i].transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    allOptions[i].transform.Find("Desc").Find("DoublePointer").gameObject.SetActive(false);

                    allOptions[i].transform.Find("Desc").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    allOptions[i].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").Find("Arrow").GetComponent<RawImage>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    allOptions[i].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").Find("Arrow").GetComponent<RawImage>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                }
                else if (i == 1 || i == 3 || i == 5)
                {
                    allOptions[i].GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    allOptions[i].transform.Find("CheckBackground").GetComponent<RawImage>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    allOptions[i].transform.Find("CheckBackground").Find("Checker").GetComponent<RawImage>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                }
            }
        }

        if (option < allOptions.Count)
        {
            if (option == 0 || option == 2)
            {
                allOptions[option].transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
                pointer.transform.SetParent(allOptions[option].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").Find("Value"));
                pointer.SetActive(true);

                if (option == 0)
                {
                    allOptions[option].transform.Find("Slider").Find("0").GetComponent<TextMeshProUGUI>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                    allOptions[option].transform.Find("Slider").Find("100").GetComponent<TextMeshProUGUI>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                }
                allOptions[option].transform.Find("Slider").Find("Background").GetComponent<Image>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                allOptions[option].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").Find("Value").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
                allOptions[option].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").GetComponent<Image>().color = new Color(1f, 1f, 1f);
                allOptions[option].transform.Find("Slider").Find("Fill Area").Find("Fill").GetComponent<Image>().color = new Color(1f, 1f, 1f);
            }
            else if (option == 4 || option == 6 || option == 7)
            {
                allOptions[option].transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
                if (option == 7) webcamVideoDisplayFrame.gameObject.SetActive(true);

                pointer.SetActive(false);
                allOptions[option].transform.Find("Desc").Find("DoublePointer").gameObject.SetActive(true);

                allOptions[option].transform.Find("Desc").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
                allOptions[option].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").Find("Arrow").GetComponent<RawImage>().color = new Color(1f, 1f, 1f);
                allOptions[option].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").Find("Arrow").GetComponent<RawImage>().color = new Color(1f, 1f, 1f);
            }
            else if (option == 1 || option == 3 || option == 5)
            {
                pointer.transform.SetParent(allOptions[option].transform);
                pointer.SetActive(true);

                allOptions[option].GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
                allOptions[option].transform.Find("CheckBackground").GetComponent<RawImage>().color = new Color(1f, 1f, 1f);
                allOptions[option].transform.Find("CheckBackground").Find("Checker").GetComponent<RawImage>().color = new Color(1f, 1f, 1f);
            }
        }
    }
    void DisplayMicrophone()
    {
        string[] microphones = Microphone.devices;
        menuOptions[6].transform.Find("Desc").GetComponent<TextMeshProUGUI>().text = GameSettings.microphoneName;
        if (microphoneIndex > 0) menuOptions[6].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(true);
        else menuOptions[6].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(false);

        if (microphoneIndex < microphones.Length - 1) menuOptions[6].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(true);
        else menuOptions[6].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(false);
    }
    void DisplayWebCam()
    {
        WebCamDevice[] cameras = WebCamTexture.devices;
        menuOptions[7].transform.Find("Desc").GetComponent<TextMeshProUGUI>().text = GameSettings.webCamName;
        if (webCamIndex > 0) menuOptions[7].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(true);
        else menuOptions[7].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(false);

        if (webCamIndex < cameras.Length - 1) menuOptions[7].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(true);
        else menuOptions[7].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(false);

        if(tex != null) tex.Stop();
        
        tex = new WebCamTexture(GameSettings.webCamName);
        webcamVideoDisplay.texture = tex;
        //tex.Play();

        float scale = (float)tex.height / (float)tex.width;
        webcamVideoDisplayFrame.sizeDelta = new Vector2(webcamVideoDisplayFrame.sizeDelta.x, webcamVideoDisplayFrame.sizeDelta.x * scale);
    }

    void DisplayGraphicQuality()
    {
        menuOptions[4].transform.Find("Desc").GetComponent<TextMeshProUGUI>().text = GameSettings.graphicsQuality.ToString();
        if(GameSettings.graphicsQuality == GameSettings.GraphicsQuality.Low)
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
}