using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
            CloseMenu();
        }
    }

    public void CloseMenu()
    {
        transform.parent.transform.Find("Menu").gameObject.SetActive(true);
        Destroy(gameObject);
    }
}
