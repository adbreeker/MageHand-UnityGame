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

    private AudioSource closeSound;
    private AudioSource changeSound;

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

    private int keyTimeDelayFirst = 20;
    private int keyTimeDelay = 10;
    private int keyTimeDelayer = 0;


    void Update()
    {
        KeysListener();
        PointOption(pointedOptionMenu, menuOptions);
        DisplayGesture(pointedOptionMenu);

        if (keyTimeDelayer > 0)
        {
            keyTimeDelayer--;
        }
    }

    void KeysListener()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            closeSound.Play();
            CloseMenu();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            changeSound.Play();
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
            changeSound.Play();
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

        if (keyTimeDelayer == 0 && Input.GetKey(KeyCode.S))
        {
            changeSound.Play();
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

        if (keyTimeDelayer == 0 && Input.GetKey(KeyCode.W))
        {
            changeSound.Play();
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

        closeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_Close);
        changeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_ChangeOption);

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
        Destroy(closeSound.gameObject, closeSound.clip.length);
        Destroy(changeSound.gameObject, changeSound.clip.length);
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
            //pointer.GetComponent<RectTransform>().sizeDelta = new Vector2(
            //    pointer.transform.parent.GetComponent<RectTransform>().sizeDelta.x + 102.5f, pointer.GetComponent<RectTransform>().sizeDelta.y);
            //pointer.transform.localPosition = new Vector3(0, 0, 0);
        }
    }
}