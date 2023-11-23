using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    static Process mediapipeHandProcess;

    private SoundManager soundManager;
    private BackgroundMusic backgroundMusic;

    public enum GraphicsQuality
    {
        Low, Medium, High
    }

    public static float soundVolume = 0.3f;
    public static string microphoneName = null;
    public static string webCamName = null;
    public static int fpsCap = 1;
    private int checkerFpsCap;
    public static GraphicsQuality graphicsQuality = GraphicsQuality.High;
    private GraphicsQuality checkerGraphicsQuality;
    public static bool vSync = false;
    public static bool fullscreen = true;
    public static bool muteMusic = true;

    //always working settings for the game
    void Awake()
    {
        RunMediapipeExe();
        Application.quitting += CloseMediapipeExeOnQuit;

        //hide cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
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
        UpdateGraphicsQuality(true);
        checkerGraphicsQuality = graphicsQuality;

        //set vSync
        if (vSync) QualitySettings.vSyncCount = 0;
        else QualitySettings.vSyncCount = 1;

        //set fullscreen
        Screen.fullScreen = fullscreen;

        //set muteMusice
        backgroundMusic.muteBackgroundMusic = muteMusic;
    }

    private void Update()
    {
        //update soundVolume
        if (soundVolume != soundManager.GetVolume()) soundManager.ChangeVolume(soundVolume);
        
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
        if(vSync && QualitySettings.vSyncCount != 0) QualitySettings.vSyncCount = 0;
        else if(!vSync && QualitySettings.vSyncCount != 1) QualitySettings.vSyncCount = 1;

        //fullscreen
        if (Screen.fullScreen != fullscreen) Screen.fullScreen = fullscreen;

        //update muteMusic
        if (backgroundMusic.muteBackgroundMusic != muteMusic) backgroundMusic.muteBackgroundMusic = muteMusic;
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

    void UpdateGraphicsQuality(bool applyExpensiveChanges = false)
    {
        if (graphicsQuality == GraphicsQuality.Low) QualitySettings.SetQualityLevel(0, applyExpensiveChanges);
        else if (graphicsQuality == GraphicsQuality.Medium) QualitySettings.SetQualityLevel(1, applyExpensiveChanges);
        else if (graphicsQuality == GraphicsQuality.High) QualitySettings.SetQualityLevel(2, applyExpensiveChanges);
    }
}
