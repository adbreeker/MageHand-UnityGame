using UnityEngine;

public class CheckPython : MonoBehaviour
{
    void Update()
    {
        if(FindObjectOfType<MoveHandPoints>() != null) CheckIfPythonWorks();
    }

    void CheckIfPythonWorks()
    {
        if (GameSettings.mediapipeHandProcess.HasExited)
        {
            Debug.Log("Python process has exited - crashed");
            FindObjectOfType<FadeInFadeOut>().ChangeScene("Python_Crashed");
        }

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
