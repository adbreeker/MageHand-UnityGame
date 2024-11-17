using FMODUnity;
using System.Collections;
using System.IO.MemoryMappedFiles;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PythonCrashed : MonoBehaviour
{
    [SerializeField] private Transform pointer;
    [SerializeField] private Transform optionMenu;
    [SerializeField] private Transform optionQuit;

    private bool isChosenOptionMenu = true;
    private bool isSceneChanging = false;

    private void Start()
    {
        StartCoroutine(RunMediapipe());
    }

    private void Update()
    {
        if (!isSceneChanging) ManageInputs();
    }

    private void ManageInputs()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.W))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiChangeOption);
            isChosenOptionMenu = !isChosenOptionMenu;

            if (isChosenOptionMenu)
                pointer.SetParent(optionMenu);
            else
                pointer.SetParent(optionQuit);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiSelectOption);
            isSceneChanging = true;
            if (isChosenOptionMenu)
                GameParams.Managers.fadeInOutManager.ChangeScene("Menu");
            else
                GameParams.Managers.fadeInOutManager.CloseGame();
        }
    }

    private IEnumerator RunMediapipe()
    {
        if (GameSettings.mediapipeHandProcess.HasExited)
        {
            GameSettings.mediapipeHandProcess = null;
            GameParams.Managers.gameSettings.RunMediapipeExe();
        }

        yield return new WaitForSeconds(3f);
        StartCoroutine(RunMediapipe());
    }
}
