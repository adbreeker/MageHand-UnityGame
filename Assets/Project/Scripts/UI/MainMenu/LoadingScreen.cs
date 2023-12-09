using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.IO.MemoryMappedFiles;
using System.IO;

public class LoadingScreen : MonoBehaviour
{
    public TextMeshProUGUI text;

    public float fontBig = 42f;
    public float fontSmall = 38f;

    void Start()
    {
        text.fontSize = fontSmall;
        StartCoroutine(Pulls());
    }

    private void Update()
    {
        //There we need to check if mediapipeProcess is loaded
        try
        {
            MemoryMappedFile mmf_gesture = MemoryMappedFile.OpenExisting("gestures");
            MemoryMappedViewStream stream_gesture = mmf_gesture.CreateViewStream();
            BinaryReader reader_gesture = new BinaryReader(stream_gesture);
            byte[] frameGesture = reader_gesture.ReadBytes(12);
            string gesture = System.Text.Encoding.UTF8.GetString(frameGesture, 0, 12);
            if (gesture[0] == '\0')
            {
                return;
            }
        }
        catch
        {
            return;
        }

        //Watch out!!! - it execudes code few more times before scene is changed (mind it if something wrong with loading game)

        Debug.Log("Loaded mediapipe");
        FindObjectOfType<FadeInFadeOut>().ChangeScene(ProgressSaving.GetSaveByName(ProgressSaving.saveName).gameStateSave.currentLvl);
    }

    IEnumerator Pulls()
    {
        while (text.fontSize < fontBig)
        {
            text.fontSize += 0.05f;
            yield return new WaitForSeconds(0);
        }
        yield return new WaitForSeconds(0);

        while (text.fontSize > fontSmall)
        {
            text.fontSize -= 0.05f;
            yield return new WaitForSeconds(0);
        }
        StartCoroutine(Pulls());
    }
}
