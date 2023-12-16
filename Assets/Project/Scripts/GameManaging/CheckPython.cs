using System.Diagnostics;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPython : MonoBehaviour
{
    public bool changeScene = true;
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "Opening") CheckIfPythonWorks();
    }

    void CheckIfPythonWorks()
    {
        foreach (Process current in Process.GetProcesses())
        {
            if ((current.Id == GameSettings.mediapipeHandProcess.Id) && current.HasExited)
            {
                {
                    UnityEngine.Debug.Log("Python process has exited - crashed");
                    if(changeScene) FindObjectOfType<FadeInFadeOut>().ChangeScene("Python_Crashed");
                }
            }
        }


        /*
        if (GameSettings.mediapipeHandProcess.HasExited)
        {
            UnityEngine.Debug.Log("Python process has exited - crashed");
            //FindObjectOfType<FadeInFadeOut>().ChangeScene("Python_Crashed");
        }
        */


        /*
        Debug.Log("checking");
        try
        {
            MemoryMappedFile mmf_gesture = MemoryMappedFile.OpenExisting("gestures");
            MemoryMappedViewStream stream_gesture = mmf_gesture.CreateViewStream();
            BinaryReader reader_gesture = new BinaryReader(stream_gesture);
            byte[] frameGesture = reader_gesture.ReadBytes(12);
            string gesture = System.Text.Encoding.UTF8.GetString(frameGesture, 0, 12);
            if (gesture[0] == '\0')
            {
                Debug.Log("file exists: Python crashed or not loaded yet");
                FindObjectOfType<FadeInFadeOut>().ChangeScene("Python_Crashed");
            }
        }
        catch
        {
            Debug.Log("file doesn't exist: Python crashed or not loaded yet");
            FindObjectOfType<FadeInFadeOut>().ChangeScene("Python_Crashed");
        }
        */
    }
}
