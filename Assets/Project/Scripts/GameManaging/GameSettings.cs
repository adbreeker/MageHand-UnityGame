using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static Process mediapipeHandProcess;
    public static Process whisperProcess;

    private SoundManager soundManager;

    public enum GraphicsQuality
    {
        Low, Medium, High
    }

    public static float soundVolume = 0.3f;

    public static string microphoneName = null;

    public static string webCamName = null;

    // 0 - 30; 1 - 60; 2 - 120; 3 - 144; 4 - 160; 5 - 165; 6 - 180; 7 - 200; 8 - 240; 9 - 360; 10 - unlimited
    public static int fpsCap = 10;
    private int checkerFpsCap;

    public static GraphicsQuality graphicsQuality = GraphicsQuality.High;
    private GraphicsQuality checkerGraphicsQuality;

    public static bool vSync = false;
    public static int vSyncCount = 1;

    public static bool fullscreen = true;
    private float lastWidth;
    private float lastHeight;

    public static bool muteMusic = false;

    public static bool useSpeech = true;

    public static bool gestureHints = true;

    //always working settings for the game
    void Awake()
    {
        RunMediapipeExe();
        Application.quitting += CloseMediapipeExeOnQuit;
        RunWhisperExe();
        Application.quitting += CloseWhisperExeOnQuit;

        LoadGameSettings();
    }

    //applying player setting from saves (now it's hardcoded)
    private void Start()
    {
        soundManager = GameParams.Managers.soundManager;

        //set volume
        soundManager.ChangeVolume(soundVolume);

        //set microphone
        UpdateMicrophone();

        //set webcam
        UpdateWebCam();

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
    }

    private void Update()
    {
        //update soundVolume
        if (soundVolume != soundManager.GetVolume()) soundManager.ChangeVolume(soundVolume, fromPauseMenu: true);

        //update webcam and microphone
        UpdateMicrophone();
        UpdateWebCam();

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

        //useSpeech
        if (microphoneName == null) useSpeech = false;

        //resizing resolution while in window mode to stay in 16/9
        if (!Screen.fullScreen)
        {
            if (lastWidth != Screen.width) Screen.SetResolution(Screen.width, (int)(Screen.width * (9f / 16f)), false);
            else if (lastHeight != Screen.height) Screen.SetResolution((int)(Screen.height * (16f / 9f)), Screen.height, false);

            lastWidth = Screen.width;
            lastHeight = Screen.height;
        }
    }

    void LoadGameSettings()
    {
        soundVolume = PlayerPrefs.GetFloat("soundVolume", 0.3f);
        microphoneName = PlayerPrefs.GetString("microphoneName", null);
        webCamName = PlayerPrefs.GetString("webCamName", null);
        fpsCap = PlayerPrefs.GetInt("fpsCap", 10);
        graphicsQuality = (GraphicsQuality)PlayerPrefs.GetInt("graphicsQuality", 2);
        vSync = PlayerPrefs.GetInt("vSync", 0) != 0;
        vSyncCount = PlayerPrefs.GetInt("vSyncCount", 1);
        fullscreen = PlayerPrefs.GetInt("fullScreen", 1) != 0;
        muteMusic = PlayerPrefs.GetInt("muteMusic", 0) != 0;
        useSpeech = PlayerPrefs.GetInt("useSpeach", 1) != 0;
        gestureHints = PlayerPrefs.GetInt("gestureHints", 1) != 0;
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

    void RunWhisperExe()
    {
        if (whisperProcess == null)
        {
            UnityEngine.Debug.Log("Running whisper");
            string handPath = Path.Combine(Application.streamingAssetsPath, "Whisper", "stream.exe");

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = handPath;
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            whisperProcess = new Process();
            whisperProcess.StartInfo = startInfo;
            whisperProcess.Start();
        }
    }

    void CloseWhisperExeOnQuit()
    {
        if (whisperProcess != null && !whisperProcess.HasExited)
        {
            UnityEngine.Debug.Log("Killing whisper");
            whisperProcess.Kill();
            whisperProcess.Dispose();
        }
    }

    //change microphone to chosen from settings if it is able to find it
    void UpdateMicrophone()
    {
        if (Microphone.devices.Length > 0)
        {
            if (microphoneName == null) microphoneName = Microphone.devices[0];
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

                if (!found) microphoneName = Microphone.devices[0];
            }
        }
        else microphoneName = null;
    }

    //change webcam to chosen from settings if it is able to find it
    void UpdateWebCam()
    {
        if(WebCamTexture.devices.Length > 0)
        {
            if (webCamName == null) webCamName = WebCamTexture.devices[0].name;
            else
            {
                bool found = false;
                foreach (WebCamDevice webCamDevice in WebCamTexture.devices)
                {
                    if (webCamDevice.name == webCamName)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found) webCamName = WebCamTexture.devices[0].name;
            }
        }
        else webCamName = null;
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
