using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class Journal : MonoBehaviour
{
    //List of dialogues that the player has completed (dialogueName, (speaker, text)
    public Dictionary<string, List<List<string>>> dialoguesJournal = new Dictionary<string, List<List<string>>>();

    //List of notes that the player has picked up (name, content)
    public Dictionary<string, string> notesJournal = new Dictionary<string, string>();

    [Header("Prefabs")]
    public GameObject journalPrefab;
    public GameObject namePrefab;
    public GameObject linePrefab;
    public GameObject notePrefab;


    [Header("Settings")]
    public bool ableToInteract = true;
    public bool journalOpened = false;

    private AudioSource closeSound;
    private AudioSource openSound;
    private AudioSource changeSound;
    private AudioSource selectSound;

    private GameObject instantiatedJournal;
    private GameObject pointer;
    private GameObject namesBackground;

    private List<TextMeshProUGUI> instantiatedDialoguesNames;
    private GameObject instantiatedDialogueName;
    private GameObject dialoguesNamesScrollView;
    private GameObject dialoguesNamesContentBox;
    private GameObject emptyDialoguesTitle;

    private List<TextMeshProUGUI> instantiatedNotesNames;
    private GameObject instantiatedNoteName;
    private GameObject notesNamesScrollView;
    private GameObject notesNamesContentBox;
    private GameObject emptyNotesTitle;

    private GameObject instantiatedNote;

    private List<GameObject> instantiatedLines;
    private GameObject instantiatedLine;
    private GameObject dialogueLinesScrollView;
    private GameObject dialogueLinesContentBox;
    private GameObject dialogeLinesBackground;
    private GameObject dialogueLinesTitle;

    private bool atNamesList = false;
    public bool atDialoguesNamesList = false;
    public bool atNotesNamesList = false;
    private int pointedDialogueName;
    private int pointedNoteName;

    private float keyTimeDelayFirst = 20f;
    private float keyTimeDelay = 10f;
    private float keyTimeDelayer = 0;
    private float dialogueScrollSpeed;

    /*
    private void Start()
    {
        dialoguesJournal.Add("1", new List<List<string>> { new List<string> { null, "tekscik taki o" }});
        dialoguesJournal.Add("2", new List<List<string>> { new List<string> { null, "tekscik taki o" } });
        dialoguesJournal.Add("3", new List<List<string>> { new List<string> { null, "tekscik taki o" } });
        dialoguesJournal.Add("4", new List<List<string>> { new List<string> { null, "tekscik taki o" } });
        dialoguesJournal.Add("5", new List<List<string>> { new List<string> { null, "tekscik taki o" } });
        dialoguesJournal.Add("6", new List<List<string>> { new List<string> { null, "tekscik taki o" } });
        dialoguesJournal.Add("7", new List<List<string>> { new List<string> { null, "tekscik taki o" } });
        dialoguesJournal.Add("8", new List<List<string>> { new List<string> { null, "tekscik taki o" } });
        dialoguesJournal.Add("9", new List<List<string>> { new List<string> { null, "tekscik taki o" } });
        dialoguesJournal.Add("10", new List<List<string>> { new List<string> { null, "tekscik taki o" } });
        dialoguesJournal.Add("11", new List<List<string>> { new List<string> { null, "tekscik taki o" } });
        dialoguesJournal.Add("12", new List<List<string>> { new List<string> { null, "tekscik taki o" } });
        dialoguesJournal.Add("13", new List<List<string>> { new List<string> { null, "tekscik taki o" } });
        dialoguesJournal.Add("14", new List<List<string>> { new List<string> { null, "tekscik taki o" } });
        dialoguesJournal.Add("15", new List<List<string>> { new List<string> { null, "tekscik taki o" } });
        dialoguesJournal.Add("16", new List<List<string>> { new List<string> { null, "tekscik taki o" } });
        dialoguesJournal.Add("17", new List<List<string>> { new List<string> { null, "tekscik taki o" } });
        dialoguesJournal.Add("18", new List<List<string>> { new List<string> { null, "tekscik taki o" } });
        dialoguesJournal.Add("KRUCIAK", new List<List<string>> { new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" } });
        dialoguesJournal.Add("ŒREDNIAK", new List<List<string>> { new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" } });
        dialoguesJournal.Add("LONGER", new List<List<string>> { new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" } });
        dialoguesJournal.Add("TURBO LONGER", new List<List<string>> { new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" },
            new List<string> { null, "tekscik taki o" }, new List<string> { "You", "tekscik taki o" }, new List<string> { "Strange voice", "tekscik taki o" } });

        notesJournal.Add( "Name1", "text of note" );
        notesJournal.Add( "Name2", "text of note" );
        notesJournal.Add( "Name3", "text of note" );
        notesJournal.Add( "Name4", "text of note" );
        notesJournal.Add( "Name5", "text of note" );
        notesJournal.Add( "Name6", "text of note" );
        notesJournal.Add( "Name7", "text of note" );
        notesJournal.Add( "Name8", "text of note" );
        notesJournal.Add( "Name9", "text of note" );
        notesJournal.Add( "Name10", "text of note" );
        notesJournal.Add( "Name11", "text of note" );
        notesJournal.Add( "Name12", "text of note" );
        notesJournal.Add( "Name13", "text of note" );
        notesJournal.Add( "Name14", "text of note" );
        notesJournal.Add( "Name15", "text of note" );
        notesJournal.Add( "Name16", "text of note" );
        notesJournal.Add( "Name17", "text of note" );
        notesJournal.Add( "Name18", "text of note" );
    }
    */

    void Update()
    {
        if (ableToInteract)
        {
            KeysListener();
        }

        if (keyTimeDelayer > 0) keyTimeDelayer -= 75 * Time.unscaledDeltaTime;
    }

    void KeysListener()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (!journalOpened)
            {
                OpenJournal();
                openSound.Play();
            }
            else 
            {
                closeSound.Play();
                CloseJournal();
            }
        }


        if (atNamesList && journalOpened)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                closeSound.Play();
                CloseJournal();
            }


            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(atDialoguesNamesList && instantiatedDialoguesNames.Count > 0)
                {
                    selectSound.Play();
                    DisplayDialogue(instantiatedDialoguesNames[pointedDialogueName].text);
                }
                if(atNotesNamesList && instantiatedNotesNames.Count > 0)
                {
                    DisplayNote(instantiatedNotesNames[pointedNoteName].text);
                }
            }


            if (Input.GetKeyDown(KeyCode.W))
            {
                if (atDialoguesNamesList)
                {
                    changeSound.Play();
                    if(pointedDialogueName > 0)
                    {
                        pointedDialogueName--;
                        dialoguesNamesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition += 1f / (instantiatedDialoguesNames.Count - 1);
                    } 
                    else
                    {
                        pointedDialogueName = instantiatedDialoguesNames.Count - 1;
                        dialoguesNamesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
                    }
                    PointName(pointedDialogueName);
                    keyTimeDelayer = keyTimeDelayFirst;
                }
                if (atNotesNamesList)
                {
                    changeSound.Play();
                    if (pointedNoteName > 0)
                    {
                        pointedNoteName--;
                        notesNamesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition += 1f / (instantiatedNotesNames.Count - 1);
                    }
                    else
                    {
                        pointedNoteName = instantiatedNotesNames.Count - 1;
                        notesNamesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
                    }
                    PointName(pointedNoteName);
                    keyTimeDelayer = keyTimeDelayFirst;
                }
            }


            if (Input.GetKeyDown(KeyCode.S))
            {
                if (atDialoguesNamesList)
                {
                    changeSound.Play();
                    if(pointedDialogueName < instantiatedDialoguesNames.Count - 1)
                    {
                        pointedDialogueName++;
                        dialoguesNamesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition += -1f / (instantiatedDialoguesNames.Count - 1);
                    }
                    else
                    {
                        pointedDialogueName = 0;
                        dialoguesNamesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
                    }
                    PointName(pointedDialogueName);
                    keyTimeDelayer = keyTimeDelayFirst;
                }
                if (atNotesNamesList)
                {
                    changeSound.Play();
                    if(pointedNoteName < instantiatedNotesNames.Count - 1)
                    {
                        pointedNoteName++;
                        notesNamesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition += -1f / (instantiatedNotesNames.Count - 1);
                    }
                    else
                    {
                        pointedNoteName = 0;
                        notesNamesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
                    }
                    PointName(pointedNoteName);
                    keyTimeDelayer = keyTimeDelayFirst;
                }
            }

            if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.W))
            {
                if (atDialoguesNamesList)
                {
                    changeSound.Play();
                    if (pointedDialogueName > 0)
                    {
                        pointedDialogueName--;
                        dialoguesNamesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition += 1f / (instantiatedDialoguesNames.Count - 1);
                    }
                    else
                    {
                        pointedDialogueName = instantiatedDialoguesNames.Count - 1;
                        dialoguesNamesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
                    }
                    PointName(pointedDialogueName);
                    keyTimeDelayer = keyTimeDelay;
                }
                if (atNotesNamesList)
                {
                    changeSound.Play();
                    if (pointedNoteName > 0)
                    {
                        pointedNoteName--;
                        notesNamesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition += 1f / (instantiatedNotesNames.Count - 1);
                    }
                    else
                    {
                        pointedNoteName = instantiatedNotesNames.Count - 1;
                        notesNamesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
                    }
                    PointName(pointedNoteName);
                    keyTimeDelayer = keyTimeDelay;
                }
            }


            if (keyTimeDelayer <= 0 && Input.GetKey(KeyCode.S))
            {
                if (atDialoguesNamesList)
                {
                    changeSound.Play();
                    if (pointedDialogueName < instantiatedDialoguesNames.Count - 1)
                    {
                        pointedDialogueName++;
                        dialoguesNamesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition += -1f / (instantiatedDialoguesNames.Count - 1);
                    }
                    else
                    {
                        pointedDialogueName = 0;
                        dialoguesNamesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
                    }
                    PointName(pointedDialogueName);
                    keyTimeDelayer = keyTimeDelay;
                }
                if (atNotesNamesList)
                {
                    changeSound.Play();
                    if (pointedNoteName < instantiatedNotesNames.Count - 1)
                    {
                        pointedNoteName++;
                        notesNamesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition += -1f / (instantiatedNotesNames.Count - 1);
                    }
                    else
                    {
                        pointedNoteName = 0;
                        notesNamesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
                    }
                    PointName(pointedNoteName);
                    keyTimeDelayer = keyTimeDelay;
                }
            }

            if (Input.GetKeyDown(KeyCode.A) && atNotesNamesList && pointedDialogueName < instantiatedDialoguesNames.Count)
            {
                changeSound.Play();
                atDialoguesNamesList = true;
                atNotesNamesList = false;
                PointName(pointedDialogueName);
            }

            if (Input.GetKeyDown(KeyCode.D) && atDialoguesNamesList && pointedNoteName < instantiatedNotesNames.Count)
            {
                changeSound.Play();
                atDialoguesNamesList = false;
                atNotesNamesList = true;
                PointName(pointedNoteName);
            }

        }


        if (!atNamesList && journalOpened)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                closeSound.Play();
                DisplayNamesBack();
            }

            if (Input.GetKey(KeyCode.W))
            {
                dialogueScrollSpeed = 1000 * Time.unscaledDeltaTime / (dialogueLinesScrollView.GetComponent<ScrollRect>().content.sizeDelta.y - dialogueLinesScrollView.GetComponent<RectTransform>().sizeDelta.y);
                dialogueLinesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition += dialogueScrollSpeed;
            }   

            if (Input.GetKey(KeyCode.S))
            {
                dialogueScrollSpeed = 1000 * Time.unscaledDeltaTime / (dialogueLinesScrollView.GetComponent<ScrollRect>().content.sizeDelta.y - dialogueLinesScrollView.GetComponent<RectTransform>().sizeDelta.y);
                dialogueLinesScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition += -dialogueScrollSpeed;
            }
        }
    }
    public void OpenJournal()
    {
        instantiatedJournal = Instantiate(journalPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);

        //Disable other controls (close first, because it activates movement and enable other ui)
        PlayerParams.Controllers.inventory.CloseInventory();
        PlayerParams.Controllers.pauseMenu.CloseMenu();
        PlayerParams.Controllers.spellbook.CloseSpellbook();
        PlayerParams.Controllers.pauseMenu.ableToInteract = false;
        PlayerParams.Variables.uiActive = true;
        PlayerParams.Objects.hand.SetActive(false);

        openSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_Open);
        closeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_Close);
        changeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_ChangeOption);
        selectSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_SelectOption);

        pointer = instantiatedJournal.transform.Find("ScrollableDialoguesNames").Find("Pointer").gameObject;
        namesBackground = instantiatedJournal.transform.Find("Background").Find("NamesBackgroud").gameObject;

        dialoguesNamesScrollView = instantiatedJournal.transform.Find("ScrollableDialoguesNames").gameObject;
        dialoguesNamesContentBox = instantiatedJournal.transform.Find("ScrollableDialoguesNames").Find("Content").gameObject;
        emptyDialoguesTitle = instantiatedJournal.transform.Find("Background").Find("NamesBackgroud").Find("EmptyDialogues").gameObject;

        notesNamesScrollView = instantiatedJournal.transform.Find("ScrollableNotesNames").gameObject;
        notesNamesContentBox = instantiatedJournal.transform.Find("ScrollableNotesNames").Find("Content").gameObject;
        emptyNotesTitle = instantiatedJournal.transform.Find("Background").Find("NamesBackgroud").Find("EmptyNotes").gameObject;


        dialogueLinesScrollView = instantiatedJournal.transform.Find("ScrollableDialogue").gameObject;
        dialogueLinesContentBox = instantiatedJournal.transform.Find("ScrollableDialogue").Find("Content").gameObject;
        dialogeLinesBackground = instantiatedJournal.transform.Find("Background").Find("DialogueBackground").gameObject;
        dialogueLinesTitle = instantiatedJournal.transform.Find("Background").Find("DialogueBackground").Find("Title").gameObject;


        dialoguesNamesScrollView.SetActive(true);
        namesBackground.SetActive(true);

        //display dialogues names
        instantiatedDialoguesNames = new List<TextMeshProUGUI>();

        List<string> keys = new List<string>();
        foreach (string key in dialoguesJournal.Keys)
        {
            keys.Add(key);
        }
        keys.Reverse();

        foreach (string key in keys)
        {
            instantiatedDialogueName = Instantiate(namePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, dialoguesNamesContentBox.transform);
            instantiatedDialogueName.GetComponent<TextMeshProUGUI>().text = key;
            instantiatedDialoguesNames.Add(instantiatedDialogueName.GetComponent<TextMeshProUGUI>());
        }

        //display notes names
        instantiatedNotesNames = new List<TextMeshProUGUI>();

        List<string> keys2 = new List<string>();
        foreach (string key in notesJournal.Keys)
        {
            keys2.Add(key);
        }
        keys2.Reverse();

        foreach (string key in keys2)
        {
            instantiatedNoteName = Instantiate(namePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, notesNamesContentBox.transform);
            instantiatedNoteName.GetComponent<TextMeshProUGUI>().text = key;
            instantiatedNotesNames.Add(instantiatedNoteName.GetComponent<TextMeshProUGUI>());
        }

        //pointing and showing info about empy
        pointedDialogueName = 0;
        pointedNoteName = 0;
        atDialoguesNamesList = false;
        atNotesNamesList = false;

        if (instantiatedDialoguesNames.Count > 0)
        {
            atDialoguesNamesList = true;
            PointName(pointedDialogueName);
            pointer.SetActive(true);
        }
        else
        {
            emptyDialoguesTitle.SetActive(true);
        }


        if (instantiatedNotesNames.Count > 0)
        {
            if (!atDialoguesNamesList)
            {
                atNotesNamesList = true;
                PointName(pointedNoteName);
                pointer.SetActive(true);
            }
        }
        else
        {
            emptyNotesTitle.SetActive(true);
        }

        //end
        atNamesList = true;
        journalOpened = true;
    }

    public void CloseJournal()
    {
        if (journalOpened)
        {
            Destroy(openSound.gameObject, openSound.clip.length);
            Destroy(closeSound.gameObject, closeSound.clip.length);
            Destroy(selectSound.gameObject, selectSound.clip.length);
            Destroy(changeSound.gameObject, changeSound.clip.length);
        }
        if (instantiatedNote != null) Destroy(instantiatedNote);
        Destroy(instantiatedJournal);
        journalOpened = false;

        //Enable other controls
        PlayerParams.Controllers.pauseMenu.ableToInteract = true;
        PlayerParams.Variables.uiActive = false;
        PlayerParams.Objects.hand.SetActive(true);
    }

    void PointName(int pointedName)
    {
        //Change color of text and make pointer child of chosen option
        for (int i = 0; i < instantiatedDialoguesNames.Count; i++)
        {
            instantiatedDialoguesNames[i].color = new Color(0.2666f, 0.2666f, 0.2666f);
        }
        for (int i = 0; i < instantiatedNotesNames.Count; i++)
        {
            instantiatedNotesNames[i].color = new Color(0.2666f, 0.2666f, 0.2666f);
        }

        if (atDialoguesNamesList)
        {
            if (pointedNoteName < instantiatedNotesNames.Count) instantiatedNotesNames[pointedNoteName].color = new Color(0.4625f, 0.4625f, 0.4625f);

            if (pointedName < instantiatedDialoguesNames.Count)
            {
                instantiatedDialoguesNames[pointedName].color = new Color(1f, 1f, 1f);

                pointer.transform.SetParent(instantiatedDialoguesNames[pointedName].transform);
            }
        }
        if (atNotesNamesList)
        {
            if (pointedDialogueName < instantiatedDialoguesNames.Count) instantiatedDialoguesNames[pointedDialogueName].color = new Color(0.4625f, 0.4625f, 0.4625f);

            if (pointedName < instantiatedNotesNames.Count)
            {
                instantiatedNotesNames[pointedName].color = new Color(1f, 1f, 1f);

                pointer.transform.SetParent(instantiatedNotesNames[pointedName].transform);
            }
        }
    }

    void DisplayDialogue(string name)
    {
        dialoguesNamesScrollView.SetActive(false);
        notesNamesScrollView.SetActive(false);
        namesBackground.SetActive(false);

        dialogueLinesScrollView.SetActive(true);
        dialogeLinesBackground.SetActive(true);

        instantiatedLines = new List<GameObject>();
        foreach (List<string> list in dialoguesJournal[name])
        {
            instantiatedLine = Instantiate(linePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, dialogueLinesContentBox.transform);
            if (list[0] != "")
            {
                instantiatedLine.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = list[0];
                if (list[0] == "You") instantiatedLine.transform.Find("Title").GetComponent<TextMeshProUGUI>().color = new Color(0.2070577f, 0.8989306f, 0.9339623f); //Blue
                else if (list[0] == "Guide" || list[0] == "Strange voice") instantiatedLine.transform.Find("Title").GetComponent<TextMeshProUGUI>().color = new Color(0.9337825f, 0.9433962f, 0.4583482f); //Yellow
                else instantiatedLine.transform.Find("Title").GetComponent<TextMeshProUGUI>().color = new Color(0.3055397f, 0.6603774f, 0.252314f); //Green
            }
            else Destroy(instantiatedLine.transform.Find("Title").gameObject);
            instantiatedLine.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = list[1];
            instantiatedLines.Add(instantiatedLine);
        }

        dialogueLinesTitle.GetComponent<TextMeshProUGUI>().text = name;
        dialogueLinesScrollView.GetComponent<ScrollRect>().verticalScrollbar.value = 1;

        atNamesList = false;
    }

    public void DisplayNamesBack()
    {
        dialoguesNamesScrollView.SetActive(true);
        notesNamesScrollView.SetActive(true);
        namesBackground.SetActive(true);

        dialogueLinesScrollView.SetActive(false);
        dialogeLinesBackground.SetActive(false);

        if(instantiatedLines != null)
        {
            foreach (GameObject line in instantiatedLines)
            {
                Destroy(line);
            }
        }

        atNamesList = true;
    }

    void DisplayNote(string name)
    {
        dialoguesNamesScrollView.SetActive(false);
        notesNamesScrollView.SetActive(false);
        namesBackground.SetActive(false);

        instantiatedNote = Instantiate(notePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        instantiatedNote.GetComponent<Note>().titleText = name;
        instantiatedNote.GetComponent<Note>().contentText = notesJournal[name];
        instantiatedNote.GetComponent<Note>().OpenNote(givenFromJournal: true);

        atNamesList = false;
    }
}