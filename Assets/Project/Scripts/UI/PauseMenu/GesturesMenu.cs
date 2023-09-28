using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GesturesMenu : MonoBehaviour
{
    private GameObject pointer;
    private int pointedOptionMenu;
    private List<TextMeshProUGUI> menuOptions = new List<TextMeshProUGUI>();

    void Update()
    {
        KeysListener();
        PointOption(pointedOptionMenu, menuOptions);
    }

    void KeysListener()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMenu();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (pointedOptionMenu < menuOptions.Count - 1)
            {
                pointedOptionMenu++;
            }
            else
            {
                pointedOptionMenu = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (pointedOptionMenu > 0)
            {
                pointedOptionMenu--;
            }
            else
            {
                pointedOptionMenu = menuOptions.Count - 1;
            }
        }
    }

    public void OpenMenu(GameObject givenPointer)
    {
        pointer = givenPointer;

        for (int i = 1; i < 7; i++)
        {
            string text = i.ToString();
            menuOptions.Add(transform.Find("Menu").Find("Options").Find(text).GetComponent<TextMeshProUGUI>());
        }

        pointedOptionMenu = 0;
        PointOption(pointedOptionMenu, menuOptions);
    }

    public void CloseMenu()
    {
        pointer.transform.SetParent(transform.parent.transform.Find("Menu").Find("Options").Find("4"));
        pointer.transform.localPosition = new Vector3(0, 0, 0);
        menuOptions.Clear();
        transform.parent.transform.Find("Menu").gameObject.SetActive(true);
        Destroy(gameObject);
    }

    void PointOption(int option, List<TextMeshProUGUI> allOptions)
    {
        for (int i = 0; i < allOptions.Count; i++)
        {
            allOptions[i].color = new Color(0.2666f, 0.2666f, 0.2666f);
        }

        if (option < allOptions.Count)
        {
            allOptions[option].color = new Color(1f, 1f, 1f);

            pointer.transform.SetParent(allOptions[option].transform);
            pointer.GetComponent<RectTransform>().sizeDelta = new Vector2(
                pointer.transform.parent.GetComponent<RectTransform>().sizeDelta.x + 102.5f, pointer.GetComponent<RectTransform>().sizeDelta.y);
            pointer.transform.localPosition = new Vector3(0, 0, 0);
        }
    }
}