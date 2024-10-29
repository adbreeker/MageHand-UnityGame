using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMODUnity;

public class GesturesMenu : MonoBehaviour
{
    private GameObject pointer;
    private int pointedOptionMenu;
    private TextMeshProUGUI description;
    private List<TextMeshProUGUI> menuOptions = new List<TextMeshProUGUI>();

    [Header("Click")]
    [TextArea(2, 5)]
    public string gestureDescription1;
    public GameObject hand1;

    [Header("Grab")]
    [TextArea(2, 5)]
    public string gestureDescription2;
    public GameObject hand2;

    [Header("Throw")]
    [TextArea(2, 5)]
    public string gestureDescription3;
    public GameObject hand3;

    [Header("Cast")]
    [TextArea(2, 5)]
    public string gestureDescription4;
    public GameObject hand4;

    [Header("Equip")]
    [TextArea(2, 5)]
    public string gestureDescription5;
    public GameObject hand5;

    [Header("Drink")]
    [TextArea(2, 5)]
    public string gestureDescription6;
    public GameObject hand6;

    private List<string> descriptions = new List<string>();
    private List<GameObject> hands = new List<GameObject>();

    private float keyTimeDelayFirst = 20f;
    private float keyTimeDelay = 10f;
    private float keyTimeDelayer = 0;


    void Update()
    {
        KeysListener();
        PointOption(pointedOptionMenu, menuOptions);
        DisplayGesture(pointedOptionMenu);

        if (keyTimeDelayer > 0) keyTimeDelayer -= 75 * Time.unscaledDeltaTime;
    }

    void KeysListener()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiClose);
            CloseMenu();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiChangeOption);
            if (pointedOptionMenu < menuOptions.Count - 1)
            {
                pointedOptionMenu++;
            }
            else
            {
                pointedOptionMenu = 0;
            }
            keyTimeDelayer = keyTimeDelayFirst;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiChangeOption);
            if (pointedOptionMenu > 0)
            {
                pointedOptionMenu--;
            }
            else
            {
                pointedOptionMenu = menuOptions.Count - 1;
            }
            keyTimeDelayer = keyTimeDelayFirst;
        }

        if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.S))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiChangeOption);
            if (pointedOptionMenu < menuOptions.Count - 1)
            {
                pointedOptionMenu++;
            }
            else
            {
                pointedOptionMenu = 0;
            }
            keyTimeDelayer = keyTimeDelay;
        }

        if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.W))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.NP_UiChangeOption);
            if (pointedOptionMenu > 0)
            {
                pointedOptionMenu--;
            }
            else
            {
                pointedOptionMenu = menuOptions.Count - 1;
            }
            keyTimeDelayer = keyTimeDelay;
        }
    }

    public void OpenMenu(GameObject givenPointer)
    {
        //transform.parent.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        //transform.parent.GetComponent<Canvas>().worldCamera = PlayerParams.Objects.playerCamera.transform.Find("UiCamera").GetComponent<Camera>();
        //transform.parent.GetComponent<Canvas>().planeDistance = 1.05f;

        descriptions.Add(gestureDescription1);
        descriptions.Add(gestureDescription2);
        descriptions.Add(gestureDescription3);
        descriptions.Add(gestureDescription4);
        descriptions.Add(gestureDescription5);
        descriptions.Add(gestureDescription6);

        hands.Add(hand1);
        hands.Add(hand2);
        hands.Add(hand3);
        hands.Add(hand4);
        hands.Add(hand5);
        hands.Add(hand6);

        pointer = givenPointer;

        for (int i = 1; i < 7; i++)
        {
            string text = i.ToString();
            menuOptions.Add(transform.Find("Menu").Find("Options").Find(text).GetComponent<TextMeshProUGUI>());
        }

        description = transform.Find("GestureDisplay").Find("Description").GetComponent<TextMeshProUGUI>();

        pointedOptionMenu = 0;
        PointOption(pointedOptionMenu, menuOptions);
    }

    public void CloseMenu()
    {
        transform.parent.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        pointer.transform.SetParent(transform.parent.transform.Find("Menu"));
        menuOptions.Clear();
        transform.parent.transform.Find("Menu").gameObject.SetActive(true);
        Destroy(gameObject);
    }

    void DisplayGesture(int option)
    {
        for(int i = 0; i < hands.Count; i++)
        {
            if(i != option) hands[i].SetActive(false);
            else hands[i].SetActive(true);
        }
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
        }
    }
}