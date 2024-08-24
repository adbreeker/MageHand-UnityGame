using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FMODUnity;

public class ControlsMenu : MonoBehaviour
{
    void Update()
    {
        KeysListener();
    }

    void KeysListener()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiClose);
            CloseMenu();
        }
    }

    public void CloseMenu()
    {
        transform.parent.transform.Find("Menu").gameObject.SetActive(true);
        Destroy(gameObject);
    }
}
