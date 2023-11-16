using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    static Process mediapipeHandProcess;

    private SoundManager soundManager;

    public static float soundVolume = 0.3f;
    public static string microphoneName = null;
    public static string webCamName = null;

    //applying player setting, actually hardcoded values
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        RunMediapipeExe();
        Application.quitting += CloseMediapipeExeOnQuit;
    }

    private void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();


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


        //hide cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        if (soundVolume != soundManager.GetVolume()) soundManager.ChangeVolume(soundVolume);
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
}
