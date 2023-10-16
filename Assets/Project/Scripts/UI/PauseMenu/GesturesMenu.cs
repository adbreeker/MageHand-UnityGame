using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GesturesMenu : MonoBehaviour
{
    private GameObject pointer;
    private int pointedOptionMenu;
    private TextMeshProUGUI description;
    private RawImage picture;
    private List<TextMeshProUGUI> menuOptions = new List<TextMeshProUGUI>();

    [Header("Click")]
    public string gestureDescription1;
    public Texture gesturePicture1;

    [Header("Grab")]
    public string gestureDescription2;
    public Texture gesturePicture2;

    [Header("Throw")]
    public string gestureDescription3;
    public Texture gesturePicture3;

    [Header("Cast")]
    public string gestureDescription4;
    public Texture gesturePicture4;

    [Header("Equip")]
    public string gestureDescription5;
    public Texture gesturePicture5;

    [Header("Drink")]
    public string gestureDescription6;
    public Texture gesturePicture6;

    private List<string> descriptions = new List<string>();
    private List<Texture> pictures = new List<Texture>();

    void Update()
    {
        KeysListener();
        PointOption(pointedOptionMenu, menuOptions);
        DisplayGesture(pointedOptionMenu);
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
        descriptions.Add(gestureDescription1);
        descriptions.Add(gestureDescription2);
        descriptions.Add(gestureDescription3);
        descriptions.Add(gestureDescription4);
        descriptions.Add(gestureDescription5);
        descriptions.Add(gestureDescription6);

        pictures.Add(gesturePicture1);
        pictures.Add(gesturePicture2);
        pictures.Add(gesturePicture3);
        pictures.Add(gesturePicture4);
        pictures.Add(gesturePicture5);
        pictures.Add(gesturePicture6);

        pointer = givenPointer;

        for (int i = 1; i < 7; i++)
        {
            string text = i.ToString();
            menuOptions.Add(transform.Find("Menu").Find("Options").Find(text).GetComponent<TextMeshProUGUI>());
        }

        picture = transform.Find("GestureDisplay").Find("Picture").GetComponent<RawImage>();
        description = transform.Find("GestureDisplay").Find("Description").GetComponent<TextMeshProUGUI>();

        pointedOptionMenu = 0;
        PointOption(pointedOptionMenu, menuOptions);
    }

    public void CloseMenu()
    {
        pointer.transform.SetParent(transform.parent.transform.Find("Menu"));
        menuOptions.Clear();
        transform.parent.transform.Find("Menu").gameObject.SetActive(true);
        Destroy(gameObject);
    }

    void DisplayGesture(int option)
    {
        picture.texture = pictures[option];
        description.text = descriptions[option];
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