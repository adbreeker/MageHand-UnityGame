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
            using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("points"))
            {
                Debug.Log("Loaded mediapipe");
                FindObjectOfType<FadeInFadeOut>().ChangeScene(ProgressSaving.GetSaveByName(ProgressSaving.saveName).gameStateSave.currentLvl);
            }
        }
        catch (FileNotFoundException)
        {
            return;
        }
        //Watch out!!! - it execudes code few more times before scene is changed (mind it if something wrong with loading game)
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
