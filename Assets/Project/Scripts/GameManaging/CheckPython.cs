using UnityEngine;

public class CheckPython : MonoBehaviour
{
    [SerializeField] private bool dontChangeSceneInEditor = true;

    private bool changeScene = true;

    void Update()
    {
        CheckIfPythonWorks();
    }

    void CheckIfPythonWorks()
    {
        if (GameSettings.mediapipeHandProcess.HasExited)
        {
            if (changeScene)
            {
                UnityEngine.Debug.LogError("Python process has exited, crashed - in build version there will be scene changing now to the Python_Crashed");
                changeScene = false;
#if UNITY_EDITOR
                if (dontChangeSceneInEditor) return;
#endif
                GameParams.Managers.fadeInOutManager.ChangeScene("Python_Crashed");
            }
        }


        //for some reason i had this foreach instead of just a check on GameSettings.mediapipeHandProcess
        //before this foreach there was just check and i don't know why i gave it up

        /*
        foreach (Process current in Process.GetProcesses())
        {
            if ((current.Id == GameSettings.mediapipeHandProcess.Id) && current.HasExited)
            {
                UnityEngine.Debug.Log("Python process has exited - crashed");
                if(changeScene)
                {
                    UnityEngine.Debug.LogError("Python process has exited, crashed - in build version there will be scene changing now to the Python_Crashed");
                    changeScene = false;
#if UNITY_EDITOR
                    return;
#endif
                    GameParams.Managers.fadeInOutManager.ChangeScene("Python_Crashed");
                }
            }
        }
        */
    }
}
