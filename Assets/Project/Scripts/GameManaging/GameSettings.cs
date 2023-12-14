using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static Process mediapipeHandProcess;
    private SoundManager soundManager;
    private BackgroundMusic backgroundMusic;

    public enum GraphicsQuality
    {
        Low, Medium, High
    }

    public static float soundVolume = 0.3f;

    public static string microphoneName = null;

    public static string webCamName = null;

    // 0 - 30; 1 - 60; 2 - 120; 3 - 144; 4 - 160; 5 - 165; 6 - 180; 7 - 200; 8 - 240; 9 - 360; 10 - unlimited
    public static int fpsCap = 1;
    private int checkerFpsCap;

    public static GraphicsQuality graphicsQuality = GraphicsQuality.High;
    private GraphicsQuality checkerGraphicsQuality;

    public static bool vSync = false;
    public static int vSyncCount = 1;

    public static bool fullscreen = true;
    private float lastWidth;
    private float lastHeight;

    public static bool muteMusic = false;

    //always working settings for the game
    void Awake()
    {
        RunMediapipeExe();
        Application.quitting += CloseMediapipeExeOnQuit;
    }

    //applying player setting from saves (now it's hardcoded)
    private void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        backgroundMusic = FindObjectOfType<BackgroundMusic>();

        //set volume
        soundManager.ChangeVolume(soundVolume);

        //set microphone
        ChangeMicrophoneOnStartIfAble();

        //set webcam
        ChangeWebcamOnStartIfAble();

        //set fpsCap
        UpdateFPSCap();
        checkerFpsCap = fpsCap;

        //set graphicsQuality
        UpdateGraphicsQuality();
        checkerGraphicsQuality = graphicsQuality;

        //set vSync
        if (vSync) QualitySettings.vSyncCount = vSyncCount;
        else QualitySettings.vSyncCount = 0;

        //set fullscreen
        Screen.fullScreen = fullscreen;
        Cursor.visible = !fullscreen;
        if(fullscreen) Cursor.lockState = CursorLockMode.Confined;
        else Cursor.lockState = CursorLockMode.None;

        //set muteMusice
        backgroundMusic.muteBackgroundMusic = muteMusic;
    }

    private void Update()
    {
        //update soundVolume
        if (soundVolume != soundManager.GetVolume()) soundManager.ChangeVolume(soundVolume, fromPauseMenu: true);
        
        //update fpsCap
        if (checkerFpsCap != fpsCap)
        {
            UpdateFPSCap();
            checkerFpsCap = fpsCap;
        }

        //update graphicsQuality
        if(checkerGraphicsQuality != graphicsQuality)
        {
            UpdateGraphicsQuality();
            checkerGraphicsQuality = graphicsQuality;
        }    

        //update vSync
        if(vSync && QualitySettings.vSyncCount != vSyncCount) QualitySettings.vSyncCount = vSyncCount;
        else if(!vSync && QualitySettings.vSyncCount != 0) QualitySettings.vSyncCount = 0;

        //fullscreen
        if (Screen.fullScreen != fullscreen)
        {
            Screen.fullScreen = fullscreen;
            Cursor.visible = !fullscreen;
            if (fullscreen)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
            }
            else Cursor.lockState = CursorLockMode.None;
        }

        //update muteMusic
        if (backgroundMusic.muteBackgroundMusic != muteMusic) backgroundMusic.muteBackgroundMusic = muteMusic;


        //resizing resolution while in window mode to stay in 16/9
        if (!Screen.fullScreen)
        {
            if (lastWidth != Screen.width) Screen.SetResolution(Screen.width, (int)(Screen.width * (9f / 16f)), false);
            else if (lastHeight != Screen.height) Screen.SetResolution((int)(Screen.height * (16f / 9f)), Screen.height, false);

            lastWidth = Screen.width;
            lastHeight = Screen.height;
        }
    }

    void RunMediapipeExe()
    {
        if(mediapipeHandProcess == null)
        {
            UnityEngine.Debug.Log("Running mediapipe");
            string handPath = Path.Combine(Application.streamingAssetsPath, "Mediapipe", "hand.exe");

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = handPath;
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            mediapipeHandProcess = new Process();
            mediapipeHandProcess.StartInfo = startInfo;
            mediapipeHandProcess.Start();
        }
    }

    void CloseMediapipeExeOnQuit()
    {
        if (mediapipeHandProcess != null && !mediapipeHandProcess.HasExited) 
        {
            UnityEngine.Debug.Log("Killing mediapipe");
            mediapipeHandProcess.Kill();
            mediapipeHandProcess.Dispose();
        }
    }

    //change microphone to chosen from settings if it is able to find it
    void ChangeMicrophoneOnStartIfAble()
    {
        if (microphoneName == null && Microphone.devices.Length > 0) microphoneName = Microphone.devices[0];
        else
        {
            bool found = false;
            foreach (string name in Microphone.devices)
            {
                if (name == microphoneName)
                {
                    found = true;
                    break;
                }
            }
            if (!found && Microphone.devices.Length > 0) microphoneName = Microphone.devices[0];
        }
    }

    //change webcam to chosen from settings if it is able to find it
    void ChangeWebcamOnStartIfAble()
    {
        if (webCamName == null && WebCamTexture.devices.Length > 0) webCamName = WebCamTexture.devices[0].name;
        else
        {
            bool found = false;
            foreach (WebCamDevice webCamDevice in WebCamTexture.devices)
            {
                if (webCamDevice.name == microphoneName)
                {
                    found = true;
                    break;
                }
            }
            if (!found && WebCamTexture.devices.Length > 0) webCamName = WebCamTexture.devices[0].name;
        }
    }

    void UpdateFPSCap()
    {
        if (fpsCap == 0) Application.targetFrameRate = 30;
        else if (fpsCap == 1) Application.targetFrameRate = 60;
        else if (fpsCap == 2) Application.targetFrameRate = 120;
        else if (fpsCap == 3) Application.targetFrameRate = 144;
        else if (fpsCap == 4) Application.targetFrameRate = 160;
        else if (fpsCap == 5) Application.targetFrameRate = 165;
        else if (fpsCap == 6) Application.targetFrameRate = 180;
        else if (fpsCap == 7) Application.targetFrameRate = 200;
        else if (fpsCap == 8) Application.targetFrameRate = 240;
        else if (fpsCap == 9) Application.targetFrameRate = 360;
        else if (fpsCap == 10) Application.targetFrameRate = -1;
        else Application.targetFrameRate = -1;
    }

    void UpdateGraphicsQuality()
    {
        if (graphicsQuality == GraphicsQuality.Low) QualitySettings.SetQualityLevel(0);
        else if (graphicsQuality == GraphicsQuality.Medium) QualitySettings.SetQualityLevel(1);
        else if (graphicsQuality == GraphicsQuality.High) QualitySettings.SetQualityLevel(2);
    }
}
