using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueDiary : MonoBehaviour
{
    //List of dialogues that the player has completed (dialogueName, (speaker, text)
    public Dictionary<string, List<List<string>>> dialogueDiary = new Dictionary<string, List<List<string>>>();

    [Header("Prefabs")]
    public GameObject diaryPrefab;
    public GameObject namePrefab;
    public GameObject linePrefab;


    [Header("Settings")]
    public bool ableToInteract = true;
    public bool diaryOpened = false;

    private GameObject instantiatedDiary;

    private List<TextMeshProUGUI> instantiatedNames;
    private GameObject instantiatedName;
    private GameObject pointer;
    private GameObject dialogueNamesScrollView;
    private GameObject dialogueNamesContentBox;
    private GameObject dialogeNamesBackground;
    private GameObject emptyTitle;

    private List<GameObject> instantiatedLines;
    private GameObject instantiatedLine;
    private GameObject dialogueLinesScrollView;
    private GameObject dialogueLinesContentBox;
    private GameObject dialogeLinesBackground;
    private GameObject dialogueLinesTitle;

    private bool atNamesList = false;
    private int pointedName;

    private int keyTimeDelayFirst = 20;
    private int keyTimeDelay = 5;
    private int keyTimeDelayer = 0;

    /*
    private void Start()
    {
        dialogueDiary.Add("TEST 2", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("TEST 3", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("TEST 4", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("TEST 5", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("TEST 6", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("TEST 7", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("TEST 8", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("TEST 9", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("TEST 10", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("TEST 11", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("TEST 12", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("TEST 13", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("TEST 14", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("TEST 15", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("TEST 16", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("TEST 17", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("TEST 18", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("TEST 19", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("TEST 20", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("TEST 21", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("TEST 22", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("TEST 23", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
        dialogueDiary.Add("LONGER", new List<List<string>> { new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" },
            new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" },
            new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" },
            new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" },
            new List<string> { null, "essasito tekscik taki o" }, new List<string> { "You", "essasito tekscik taki o" }, new List<string> { "Strange voice", "essasito tekscik taki o" } });
    }
    */

    void Update()
    {
        if (ableToInteract)
        {
            KeysListener();
        }

        if (keyTimeDelayer > 0)
        {
            keyTimeDelayer--;
        }
    }

    void KeysListener()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!diaryOpened) OpenDiary();
            else CloseDiary();
        }


        if (atNamesList && diaryOpened)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseDiary();
            }


            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (instantiatedNames.Count > 0) DisplayDialogue(instantiatedNames[pointedName].text);
            }


            if (Input.GetKeyDown(KeyCode.W))
            {
                if (pointedName > 0)
                {
                    pointedName--;
                    PointName(pointedName);
                    dialogueNamesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition += 1f / instantiatedNames.Count;
                    keyTimeDelayer = keyTimeDelayFirst;
                }
            }


            if (Input.GetKeyDown(KeyCode.S))
            {
                if (pointedName < instantiatedNames.Count - 1)
                {
                    pointedName++;
                    PointName(pointedName);
                    dialogueNamesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition += -1f / instantiatedNames.Count;
                    keyTimeDelayer = keyTimeDelayFirst;
                }
            }

            if (keyTimeDelayer == 0 && Input.GetKey(KeyCode.W))
            {
                if (pointedName > 0)
                {
                    pointedName--;
                    PointName(pointedName);
                    dialogueNamesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition += 1f / instantiatedNames.Count;
                    keyTimeDelayer = keyTimeDelay;
                }
            }


            if (keyTimeDelayer == 0 && Input.GetKey(KeyCode.S))
            {
                if (pointedName < instantiatedNames.Count - 1)
                {
                    pointedName++;
                    PointName(pointedName);
                    dialogueNamesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition += -1f / instantiatedNames.Count;
                    keyTimeDelayer = keyTimeDelay;
                }
            }
        }


        if (!atNamesList && diaryOpened)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DisplayNames();
            }

            if (Input.GetKey(KeyCode.W))
            {
                dialogueLinesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition += 0.015f;
            }   

            if (Input.GetKey(KeyCode.S))
            {
                dialogueLinesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition += -0.015f;
            }
        }
    }
    public void OpenDiary()
    {
        instantiatedDiary = Instantiate(diaryPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);

        //Disable other controls (close first, because it activates movement and enable other ui)
        PlayerParams.Controllers.inventory.CloseInventory();
        PlayerParams.Controllers.pauseMenu.CloseMenu();
        PlayerParams.Controllers.spellbook.CloseSpellbook();
        PlayerParams.Controllers.pauseMenu.ableToInteract = false;
        PlayerParams.Variables.uiActive = true;
        PlayerParams.Objects.hand.SetActive(false);

        pointer = instantiatedDiary.transform.Find("ScrollableDialogueNames").Find("Pointer").gameObject;
        dialogueNamesScrollView = instantiatedDiary.transform.Find("ScrollableDialogueNames").gameObject;
        dialogueNamesContentBox = instantiatedDiary.transform.Find("ScrollableDialogueNames").Find("Content").gameObject;
        dialogeNamesBackground = instantiatedDiary.transform.Find("Background").Find("DialogueNamesBackgroud").gameObject;
        emptyTitle = instantiatedDiary.transform.Find("Background").Find("DialogueNamesBackgroud").Find("Empty").gameObject;

        dialogueLinesScrollView = instantiatedDiary.transform.Find("ScrollableDialogue").gameObject;
        dialogueLinesContentBox = instantiatedDiary.transform.Find("ScrollableDialogue").Find("Content").gameObject;
        dialogeLinesBackground = instantiatedDiary.transform.Find("Background").Find("DialogueBackground").gameObject;
        dialogueLinesTitle = instantiatedDiary.transform.Find("Background").Find("DialogueBackground").Find("Title").gameObject;


        dialogueNamesScrollView.SetActive(true);
        dialogeNamesBackground.SetActive(true);

        instantiatedNames = new List<TextMeshProUGUI>();

        List<string> keys = new List<string>();
        foreach (string key in dialogueDiary.Keys)
        {
            keys.Add(key);
        }
        keys.Reverse();

        foreach (string key in keys)
        {
            instantiatedName = Instantiate(namePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, dialogueNamesContentBox.transform);
            instantiatedName.GetComponent<TextMeshProUGUI>().text = key;
            instantiatedNames.Add(instantiatedName.GetComponent<TextMeshProUGUI>());
        }

        if (instantiatedNames.Count > 0)
        {
            pointedName = 0;
            PointName(pointedName);
            pointer.SetActive(true);
        }
        else
        {
            emptyTitle.SetActive(true);
        }

        atNamesList = true;
        diaryOpened = true;
    }

    public void CloseDiary()
    {
        Destroy(instantiatedDiary);
        diaryOpened = false;

        //Enable other controls
        PlayerParams.Controllers.pauseMenu.ableToInteract = true;
        PlayerParams.Variables.uiActive = false;
        PlayerParams.Objects.hand.SetActive(true);
    }

    void PointName(int pointedName)
    {
        //Change color of text and make pointer child of chosen option
        for (int i = 0; i < instantiatedNames.Count; i++)
        {
            instantiatedNames[i].color = new Color(0.2666f, 0.2666f, 0.2666f);
        }

        if (pointedName < instantiatedNames.Count)
        {
            instantiatedNames[pointedName].color = new Color(1f, 1f, 1f);

            pointer.transform.SetParent(instantiatedNames[pointedName].transform);
        }
    }

    void DisplayDialogue(string name)
    {
        dialogueNamesScrollView.SetActive(false);
        dialogeNamesBackground.SetActive(false);

        dialogueLinesScrollView.SetActive(true);
        dialogeLinesBackground.SetActive(true);

        instantiatedLines = new List<GameObject>();
        foreach (List<string> list in dialogueDiary[name])
        {
            instantiatedLine = Instantiate(linePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, dialogueLinesContentBox.transform);
            if (list[0] != "")
            {
                instantiatedLine.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = list[0];
                if (list[0] == "You") instantiatedLine.transform.Find("Title").GetComponent<TextMeshProUGUI>().color = new Color(0.2070577f, 0.8989306f, 0.9339623f); //Blue
                else instantiatedLine.transform.Find("Title").GetComponent<TextMeshProUGUI>().color = new Color(0.9337825f, 0.9433962f, 0.4583482f); //Yellow
            }
            else Destroy(instantiatedLine.transform.Find("Title").gameObject);
            instantiatedLine.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = list[1];
            instantiatedLines.Add(instantiatedLine);
        }

        dialogueLinesTitle.GetComponent<TextMeshProUGUI>().text = name;

        atNamesList = false;
    }

    void DisplayNames()
    {
        dialogueNamesScrollView.SetActive(true);
        dialogeNamesBackground.SetActive(true);

        dialogueLinesScrollView.SetActive(false);
        dialogeLinesBackground.SetActive(false);

        foreach (GameObject line in instantiatedLines)
        {
            Destroy(line);
        }

        atNamesList = true;
    }
}