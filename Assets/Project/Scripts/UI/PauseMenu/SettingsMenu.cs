using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public RawImage webcamVideoDisplay;
    public RectTransform webcamVideoDisplayFrame;
    public Slider volumeSlider;
    public TextMeshProUGUI volumeValueText;
    public Slider fpsSlider;
    public TextMeshProUGUI fpsValueText;

    private WebCamTexture tex;

    private GameObject pointer;
    private int pointedOptionMenu;
    private List<GameObject> menuOptions = new List<GameObject>();

    private AudioSource closeSound;
    private AudioSource changeSound;
    private AudioSource selectSound;

    private int keyTimeDelayFirst = 20;
    private int keyTimeDelay = 10;
    private int keyTimeDelayer = 0;

    private int microphoneIndex = 0;
    private int webCamIndex = 0;
    void Update()
    {
        KeysListener();
        PointOption(pointedOptionMenu, menuOptions);

        if (keyTimeDelayer > 0) keyTimeDelayer--;


        volumeValueText.text = volumeSlider.value.ToString();
        DisplayFPSText();
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

            if (keyTimeDelayer == 0 && Input.GetKey(KeyCode.A))
            {
                volumeSlider.value -= 1;
                GameSettings.soundVolume = volumeSlider.value / 100;
            }

            if (keyTimeDelayer == 0 && Input.GetKey(KeyCode.D))
            {
                volumeSlider.value += 1;
                GameSettings.soundVolume = volumeSlider.value / 100;
            }
        }

        //Microphone
        if (pointedOptionMenu == 1)
        {
            if (Input.GetKeyDown(KeyCode.A) && menuOptions[1].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.activeSelf)
            {
                changeSound.Play();
                microphoneIndex += -1;
                ChangeMicrophone();
                DisplayMicrophone();
            }

            if (Input.GetKeyDown(KeyCode.D) && menuOptions[1].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.activeSelf)
            {
                changeSound.Play();
                microphoneIndex += 1;
                ChangeMicrophone();
                DisplayMicrophone();
            }
        }

        //Webcam
        if (pointedOptionMenu == 2)
        {
            if (Input.GetKeyDown(KeyCode.A) && menuOptions[2].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.activeSelf)
            {
                changeSound.Play();
                webCamIndex += -1;
                ChangeWebCam();
                DisplayWebCam();
            }

            if (Input.GetKeyDown(KeyCode.D) && menuOptions[2].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.activeSelf)
            {
                changeSound.Play();
                webCamIndex += 1;
                ChangeWebCam();
                DisplayWebCam();
            }
        }

        //FPS cap
        if (pointedOptionMenu == 3)
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


            if (keyTimeDelayer == 0 && Input.GetKey(KeyCode.A))
            {
                fpsSlider.value -= 1;
                keyTimeDelayer = keyTimeDelay;
                GameSettings.fpsCap = (int)fpsSlider.value;
            }

            if (keyTimeDelayer == 0 && Input.GetKey(KeyCode.D))
            {
                fpsSlider.value += 1;
                keyTimeDelayer = keyTimeDelay;
                GameSettings.fpsCap = (int)fpsSlider.value;
            }
        }

        //Graphic quality
        if (pointedOptionMenu == 4)
        {
            if (Input.GetKeyDown(KeyCode.A) && menuOptions[4].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.activeSelf)
            {
                changeSound.Play();
                GameSettings.graphicsQuality -= 1;
                DisplayGraphicQuality();
            }

            if (Input.GetKeyDown(KeyCode.D) && menuOptions[4].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.activeSelf)
            {
                changeSound.Play();
                GameSettings.graphicsQuality += 1;
                DisplayGraphicQuality();
            }
        }

        //vSync
        if (pointedOptionMenu == 5)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                selectSound.Play();
                GameSettings.vSync = !GameSettings.vSync;
                menuOptions[5].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.vSync);
            }
        }

        //Fullscreen
        if (pointedOptionMenu == 6)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                selectSound.Play();
                GameSettings.fullscreen = !GameSettings.fullscreen;
                menuOptions[6].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.fullscreen);
            }
        }

        //Mute music
        if (pointedOptionMenu == 7)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                selectSound.Play();
                GameSettings.muteMusic = !GameSettings.muteMusic;
                menuOptions[7].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.muteMusic);
            }
        }
    }
    public void OpenMenu(GameObject givenPointer)
    {
        volumeSlider.value = GameSettings.soundVolume * 100;
        fpsSlider.value = GameSettings.fpsCap;
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

        menuOptions[5].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.vSync);
        menuOptions[6].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.fullscreen);
        menuOptions[7].transform.Find("CheckBackground").Find("Checker").gameObject.SetActive(GameSettings.muteMusic);

        pointedOptionMenu = 0;
        PointOption(pointedOptionMenu, menuOptions);
    }

    public void CloseMenu()
    {
        if (tex != null) tex.Stop();

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
        webcamVideoDisplayFrame.gameObject.SetActive(false);
        for (int i = 0; i < allOptions.Count; i++)
        {
            if (i != option)
            {
                if (i == 0 || i == 3)
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
                else if (i == 1 || i == 2 || i == 4)
                {
   
                    allOptions[i].transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    allOptions[i].transform.Find("Desc").Find("DoublePointer").gameObject.SetActive(false);

                    allOptions[i].transform.Find("Desc").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    allOptions[i].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").Find("Arrow").GetComponent<RawImage>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    allOptions[i].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").Find("Arrow").GetComponent<RawImage>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                }
                else if (i == 5 || i == 6 || i == 7)
                {
                    allOptions[i].GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    allOptions[i].transform.Find("CheckBackground").GetComponent<RawImage>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    allOptions[i].transform.Find("CheckBackground").Find("Checker").GetComponent<RawImage>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                }
            }
        }

        if (option < allOptions.Count)
        {
            if (option == 0 || option == 3)
            {
                allOptions[option].transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
                pointer.transform.SetParent(allOptions[option].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").Find("Value"));
                pointer.SetActive(true);

                if (option == 1)
                {
                    allOptions[option].transform.Find("Slider").Find("0").GetComponent<TextMeshProUGUI>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                    allOptions[option].transform.Find("Slider").Find("100").GetComponent<TextMeshProUGUI>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                }
                allOptions[option].transform.Find("Slider").Find("Background").GetComponent<Image>().color = new Color(0.4625f, 0.4625f, 0.4625f);
                allOptions[option].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").Find("Value").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
                allOptions[option].transform.Find("Slider").Find("Handle Slide Area").Find("Handle").GetComponent<Image>().color = new Color(1f, 1f, 1f);
                allOptions[option].transform.Find("Slider").Find("Fill Area").Find("Fill").GetComponent<Image>().color = new Color(1f, 1f, 1f);
            }
            else if (option == 1 || option == 2 || option == 4)
            {
                allOptions[option].transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
                if (option == 2) webcamVideoDisplayFrame.gameObject.SetActive(true);

                pointer.SetActive(false);
                allOptions[option].transform.Find("Desc").Find("DoublePointer").gameObject.SetActive(true);

                allOptions[option].transform.Find("Desc").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
                allOptions[option].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").Find("Arrow").GetComponent<RawImage>().color = new Color(1f, 1f, 1f);
                allOptions[option].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").Find("Arrow").GetComponent<RawImage>().color = new Color(1f, 1f, 1f);
            }
            else if (option == 5 || option == 6 || option == 7)
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
        menuOptions[1].transform.Find("Desc").GetComponent<TextMeshProUGUI>().text = GameSettings.microphoneName;
        if (microphoneIndex > 0) menuOptions[1].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(true);
        else menuOptions[1].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(false);

        if (microphoneIndex < microphones.Length - 1) menuOptions[1].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(true);
        else menuOptions[1].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(false);
    }
    void DisplayWebCam()
    {
        WebCamDevice[] cameras = WebCamTexture.devices;
        menuOptions[2].transform.Find("Desc").GetComponent<TextMeshProUGUI>().text = GameSettings.webCamName;
        if (webCamIndex > 0) menuOptions[2].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(true);
        else menuOptions[2].transform.Find("Desc").Find("DoublePointer").Find("LeftArrow").gameObject.SetActive(false);

        if (webCamIndex < cameras.Length - 1) menuOptions[2].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(true);
        else menuOptions[2].transform.Find("Desc").Find("DoublePointer").Find("RightArrow").gameObject.SetActive(false);

        if(tex != null) tex.Stop();
        
        tex = new WebCamTexture(GameSettings.webCamName);
        webcamVideoDisplay.texture = tex;
        tex.Play();

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
}