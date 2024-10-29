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
    [Space]
    [SerializeField] private RawImage webcamVideoDisplay;
    [SerializeField] private RectTransform webcamVideoDisplayFrame;

    private bool isChosenOptionMenu = true;

    private void Start()
    {
        StartCoroutine(RunMediapipe());
    }

    private void Update()
    {
        ManageInputs();
        ShowCamera();
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



    private void ShowCamera()
    {
        try
        {
            using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("magehand_video_unity"))
            {
                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {
                    BinaryReader reader = new BinaryReader(stream);
                    byte[] frameBuffer = reader.ReadBytes(480 * 640 * 3);

                    int width = 640;
                    int height = 480;

                    Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
                    texture.LoadRawTextureData(frameBuffer);
                    texture.Apply();

                    float scale = (float)texture.height / (float)texture.width;
                    webcamVideoDisplayFrame.sizeDelta = new Vector2(webcamVideoDisplayFrame.sizeDelta.x, webcamVideoDisplayFrame.sizeDelta.x * scale);

                    webcamVideoDisplay.gameObject.SetActive(true);

                    webcamVideoDisplay.texture = texture;

                }
            }
        }
        catch (FileNotFoundException)
        {
            webcamVideoDisplay.gameObject.SetActive(false);
        }
    }
}
