using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class LoadingScreen : MonoBehaviour
{
    public TextMeshProUGUI text;

    public float fontBig = 42f;
    public float fontSmall = 38f;

    private UDPReceive receive;
    void Start()
    {
        receive = FindObjectOfType<UDPReceive>();

        text.fontSize = fontSmall;
        StartCoroutine(Pulls());
    }

    private void Update()
    {
        //There we need to check if mediapipeProcess is loaded
        if(!String.IsNullOrWhiteSpace(receive.data)) SceneManager.LoadScene(ProgressSaving.GetSaveByName(ProgressSaving.saveName).gameStateSave.currentLvl);
    }

    IEnumerator Pulls()
    {
        while (text.fontSize < fontBig)
        {
            text.fontSize += 0.05f;
            yield return new WaitForSeconds(0);
        }

        while (text.fontSize > fontSmall)
        {
            text.fontSize -= 0.05f;
            yield return new WaitForSeconds(0);
        }
        StartCoroutine(Pulls());
    }
}
