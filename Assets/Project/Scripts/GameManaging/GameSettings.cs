using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    //applying player setting, actually hardcoded values
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        //hide cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }
}
