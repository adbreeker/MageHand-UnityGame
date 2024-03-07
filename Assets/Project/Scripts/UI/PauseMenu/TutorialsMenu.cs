using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialsMenu : MonoBehaviour
{
    private GameObject pointer;
    private ScrollRect scrollView;
    private int pointedOptionMenu;
    private List<TextMeshProUGUI> menuOptions = new List<TextMeshProUGUI>();

    private AudioSource closeSound;
    private AudioSource changeSound;

    private float keyTimeDelayFirst = 20f;
    private float keyTimeDelay = 10f;
    private float keyTimeDelayer = 0;

    public List<GameObject> tutorialPrefabs = new List<GameObject>();
    private GameObject instantiatedTutorialPrefab;
    private Camera uiCamera;
    void Update()
    {
        KeysListener();

        if (keyTimeDelayer > 0) keyTimeDelayer -= 75 * Time.unscaledDeltaTime;
    }

    void KeysListener()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            closeSound.Play();
            CloseMenu();
        }


        //W
        if (Input.GetKeyDown(KeyCode.W))
        {
            changeSound.Play();
            if (pointedOptionMenu > 0)
            {
                pointedOptionMenu--;
                scrollView.verticalNormalizedPosition += 1f / (menuOptions.Count - 1);
            }
            else
            {
                pointedOptionMenu = menuOptions.Count - 1;
                scrollView.verticalNormalizedPosition = 0f;
            }
            keyTimeDelayer = keyTimeDelayFirst;
            PointOption(pointedOptionMenu);
        }
        //S
        if (Input.GetKeyDown(KeyCode.S))
        {
            changeSound.Play();
            if (pointedOptionMenu < menuOptions.Count - 1)
            {
                pointedOptionMenu++;
                scrollView.verticalNormalizedPosition += -1f / (menuOptions.Count - 1);
            }
            else
            {
                pointedOptionMenu = 0;
                scrollView.verticalNormalizedPosition = 1f;
            }
            keyTimeDelayer = keyTimeDelayFirst;
            PointOption(pointedOptionMenu);
        }
        //W hold
        if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.W))
        {
            changeSound.Play();
            if (pointedOptionMenu > 0)
            {
                pointedOptionMenu--;
                scrollView.verticalNormalizedPosition += 1f / (menuOptions.Count - 1);
            }
            else
            {
                pointedOptionMenu = menuOptions.Count - 1;
                scrollView.verticalNormalizedPosition = 0f;
            }
            keyTimeDelayer = keyTimeDelay;
            PointOption(pointedOptionMenu);
        }
        //S hold
        if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.S))
        {
            changeSound.Play();
            if (pointedOptionMenu < menuOptions.Count - 1)
            {
                pointedOptionMenu++;
                scrollView.verticalNormalizedPosition += -1f / (menuOptions.Count - 1);
            }
            else
            {
                pointedOptionMenu = 0;
                scrollView.verticalNormalizedPosition = 1f;
            }
            keyTimeDelayer = keyTimeDelay;
            PointOption(pointedOptionMenu);
        }
    }

    public void OpenMenu(GameObject givenPointer)
    {
        transform.parent.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        transform.parent.GetComponent<Canvas>().worldCamera = PlayerParams.Objects.playerCamera.transform.Find("UiCamera").GetComponent<Camera>();
        transform.parent.GetComponent<Canvas>().planeDistance = 1.05f;

        pointer = givenPointer;

        closeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_Close);
        changeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_ChangeOption);

        for (int i = 1; i < 14; i++)
        {
            string text = i.ToString();
            menuOptions.Add(transform.Find("Menu").Find("ScrollRect").Find("Content").Find(text).GetComponent<TextMeshProUGUI>());
        }

        scrollView = transform.Find("Menu").Find("ScrollRect").GetComponent<ScrollRect>();
        scrollView.verticalNormalizedPosition = 1f;
        uiCamera = PlayerParams.Objects.uiCamera;

        pointedOptionMenu = 0;
        PointOption(pointedOptionMenu);
        StartCoroutine(WaitOneFrameToPoint());
    }

    IEnumerator WaitOneFrameToPoint()
    {
        yield return 0;
        PointOption(pointedOptionMenu);
    }

    public void CloseMenu()
    {
        transform.parent.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        pointer.transform.SetParent(transform.parent.transform.Find("Menu"));
        menuOptions.Clear();
        transform.parent.transform.Find("Menu").gameObject.SetActive(true);
        Destroy(closeSound.gameObject, closeSound.clip.length);
        Destroy(changeSound.gameObject, changeSound.clip.length);
        Destroy(instantiatedTutorialPrefab);
        Destroy(gameObject);
    }

    void PointOption(int option)
    {
        Destroy(instantiatedTutorialPrefab);
        instantiatedTutorialPrefab = Instantiate(tutorialPrefabs[option], new Vector3(0, 0, 0), Quaternion.identity);
        Canvas prefabCanvas = instantiatedTutorialPrefab.GetComponent<Canvas>();
        RectTransform prefabRectTransform = instantiatedTutorialPrefab.transform.Find("Panel").GetComponent<RectTransform>();
        prefabCanvas.worldCamera = uiCamera;
        prefabCanvas.planeDistance = 1.05f;
        prefabRectTransform.anchorMin = new Vector2(0, 0.5f);
        prefabRectTransform.anchorMax = new Vector2(0, 0.5f);
        prefabRectTransform.anchoredPosition = new Vector3(600, 0, 0);
        prefabRectTransform.localScale = new Vector3(0.85f, 0.85f, 1);
        instantiatedTutorialPrefab.transform.Find("BlackoutBackground").gameObject.SetActive(false);
        instantiatedTutorialPrefab.transform.Find("Panel").Find("OptionText").gameObject.SetActive(false);
        instantiatedTutorialPrefab.transform.Find("Panel").Find("Background").GetComponent<RectTransform>().offsetMin = new Vector2(0, 75f);

        for (int i = 0; i < menuOptions.Count; i++)
        {
            menuOptions[i].color = new Color(0.2666f, 0.2666f, 0.2666f);
        }

        if (option < menuOptions.Count)
        {
            menuOptions[option].color = new Color(1f, 1f, 1f);

            pointer.transform.SetParent(menuOptions[option].transform);
        }
    }
}