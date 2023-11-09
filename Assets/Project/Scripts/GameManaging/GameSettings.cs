using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    static Process mediapipeHandProcess;

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
        //hide cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
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
