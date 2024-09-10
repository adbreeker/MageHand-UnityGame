using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.UIElements;

public class Spellbook : MonoBehaviour
{
    //List of prefabs
    public List<SpellScrollInfo> spells = new List<SpellScrollInfo>();
    [Header("Game objects")]
    public GameObject spellbookPrefab;
    public Camera UiCamera;

    [Header("Settings")]
    public bool spellbook3D = false;
    public bool bookOwned = false;
    public bool ableToInteract = true;
    public bool spellbookOpened = false;

    private EventInstance readingSound;

    private int page;
    private int pointed;

    private GameObject instantiatedSpellbook;

    private GameObject arrowLeft;
    private GameObject arrowRight;
    private GameObject titleLeft;
    private GameObject titleRight;
    private GameObject pointerLeft;
    private GameObject pointerRight;
    private GameObject descriptionLeft;
    private GameObject descriptionRight;
    private GameObject pictureLeft;
    private GameObject pictureRight;
    private GameObject numberLeft;
    private GameObject numberRight;

    //this means views not actual pages (spellbookPage = 1 view = 2 real pages)
    private List<List<SpellScrollInfo>> spellbookPages;

    private FmodEvents FmodEvents => GameParams.Managers.fmodEvents;


    void Update()
    {
        if(ableToInteract)
        {
            KeysListener();
        }
    }

    void KeysListener()
    {
        //Open or close spellbook
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!spellbookOpened && bookOwned)
            {
                OpenSpellbook();
                RuntimeManager.PlayOneShot(FmodEvents.NP_UiOpen);
            }
            else if (spellbookOpened) 
            {
                RuntimeManager.PlayOneShot(FmodEvents.NP_UiClose);
                CloseSpellbook();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && spellbookOpened)
        {
            RuntimeManager.PlayOneShot(FmodEvents.NP_UiClose);
            CloseSpellbook();
        }

        //Play pointed voice
        if (Input.GetKeyDown(KeyCode.Space) && spellbookOpened && GameSettings.useSpeech)
        {
            if (!readingSound.isValid())
            {
                EventReference readingSpellRef;
                if (spellbookPages[page][pointed - 1].spellName == "Light") readingSpellRef = FmodEvents.SFX_ReadLight;
                else if (spellbookPages[page][pointed - 1].spellName == "Mark") readingSpellRef = FmodEvents.SFX_ReadMark;
                else if (spellbookPages[page][pointed - 1].spellName == "Fire") readingSpellRef = FmodEvents.SFX_ReadFire;
                else if (spellbookPages[page][pointed - 1].spellName == "Speak") readingSpellRef = FmodEvents.SFX_ReadSpeak;
                //etc.
                else
                {
                    Debug.LogError("Spell with no reading assign");
                    readingSpellRef = FmodEvents.NP_UiSelectOption;
                } 

                readingSound = GameParams.Managers.audioManager.PlayOneShotReturnInstance(readingSpellRef);
            }



        }

        //Go left if possible
        if (Input.GetKeyDown(KeyCode.A) && spellbookOpened)
        {
            if (pointed == 2)
            {
                RuntimeManager.PlayOneShot(FmodEvents.NP_UiChangeOption);
                pointed = 1;
                PointOption(pointed);
            }
            else if (pointed == 1 && page > 0)
            {
                RuntimeManager.PlayOneShot(FmodEvents.NP_UiChangeOption);
                page--;
                DisplayPage(page);
                pointed = 2;
                PointOption(pointed);
            }
        }

        //Go right if possible
        if (Input.GetKeyDown(KeyCode.D) && spellbookOpened)
        {
            if (pointed == 1 && spellbookPages[page].Count == 2)
            {
                RuntimeManager.PlayOneShot(FmodEvents.NP_UiChangeOption);
                pointed = 2;
                PointOption(pointed);
            }
            else if (pointed == 2 && page + 1 < spellbookPages.Count)
            {
                RuntimeManager.PlayOneShot(FmodEvents.NP_UiChangeOption);
                page++;
                DisplayPage(page);
                pointed = 1;
                PointOption(pointed);
            }
        }
    }

    void OpenSpellbook()
    {
        //Instatiate spellbook and assign it to UiCamera if in 3D
        instantiatedSpellbook = Instantiate(spellbookPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);


        if (spellbook3D)
        {
            instantiatedSpellbook.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            instantiatedSpellbook.GetComponent<Canvas>().worldCamera = UiCamera;
            instantiatedSpellbook.GetComponent<Canvas>().planeDistance = 1.05f;
        }
        else
        {
            instantiatedSpellbook.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        }

        //Disable other controls (close first, because it activates movement and enable other ui)
        PlayerParams.Controllers.inventory.CloseInventory();
        PlayerParams.Controllers.journal.CloseJournal();
        PlayerParams.Controllers.pauseMenu.CloseMenu();
        PlayerParams.Controllers.pauseMenu.ableToInteract = false;
        PlayerParams.Variables.uiActive = true;
        PlayerParams.Objects.hand.SetActive(false);

        //Get right objects from prefab
        arrowLeft = instantiatedSpellbook.transform.Find("Background").Find("ArrowLeft").gameObject;
        arrowRight = instantiatedSpellbook.transform.Find("Background").Find("ArrowRight").gameObject;
        titleLeft = instantiatedSpellbook.transform.Find("Left spell").Find("Title").gameObject;
        titleRight = instantiatedSpellbook.transform.Find("Right spell").Find("Title").gameObject;
        pointerLeft = instantiatedSpellbook.transform.Find("Left spell").Find("Title").Find("Pointer left").gameObject;
        pointerRight = instantiatedSpellbook.transform.Find("Right spell").Find("Title").Find("Pointer right").gameObject;
        descriptionLeft = instantiatedSpellbook.transform.Find("Left spell").Find("Description").gameObject;
        descriptionRight = instantiatedSpellbook.transform.Find("Right spell").Find("Description").gameObject;
        pictureLeft = instantiatedSpellbook.transform.Find("Left spell").Find("Frame").gameObject;
        pictureRight = instantiatedSpellbook.transform.Find("Right spell").Find("Frame").gameObject;
        numberLeft = instantiatedSpellbook.transform.Find("Left spell").Find("Number").gameObject;
        numberRight = instantiatedSpellbook.transform.Find("Right spell").Find("Number").gameObject;

        //Divide items to pages
        spellbookPages = new List<List<SpellScrollInfo>>();
        int pageToAdd = -1;
        for (int i = 0; i < spells.Count; i++)
        {
            if (i % 2 == 0)
            {
                pageToAdd++;
                spellbookPages.Add(new List<SpellScrollInfo>());
                spellbookPages[pageToAdd].Add(spells[i]);
            }
            else
            {
                spellbookPages[pageToAdd].Add(spells[i]);
            }
        }

        //Display first page if there are spells
        page = 0;
        if (spellbookPages.Count > 0)
        {
            instantiatedSpellbook.transform.Find("Empty").gameObject.SetActive(false);
            DisplayPage(page);
            pointed = 1;
            PointOption(pointed);
        }
        else instantiatedSpellbook.transform.Find("Empty").gameObject.SetActive(true);
        spellbookOpened = true;
    }

    public void CloseSpellbook()
    {
        Destroy(instantiatedSpellbook);
        spellbookOpened = false;

        //Enable other controls
        PlayerParams.Variables.uiActive = false;
        PlayerParams.Controllers.pauseMenu.ableToInteract = true;
        PlayerParams.Objects.hand.SetActive(true);
    }
    void PointOption(int option)
    {
        if (option == 1)
        {
            pointerRight.SetActive(false);
            pointerLeft.SetActive(true);
        }
        else if (option == 2)
        {
            pointerLeft.SetActive(false);
            pointerRight.SetActive(true);
        }
    }

    void DisplayPage(int pageToDisplay)
    {
        //Deactivate texts, arrows etc.
        arrowLeft.SetActive(false);
        arrowRight.SetActive(false);
        titleLeft.SetActive(false);
        titleRight.SetActive(false);
        descriptionLeft.SetActive(false);
        descriptionRight.SetActive(false);
        pictureLeft.SetActive(false);
        pictureRight.SetActive(false);
        numberLeft.SetActive(false);
        numberRight.SetActive(false);

        //Activate correct texts, arrows etc. and assign correct things to it
        if (spellbookPages[pageToDisplay].Count > 0)
        {
            titleLeft.GetComponent<TextMeshProUGUI>().text = spellbookPages[pageToDisplay][0].spellName;
            titleLeft.SetActive(true);
            descriptionLeft.GetComponent<TextMeshProUGUI>().text = 
                "Mana cost: " + spellbookPages[pageToDisplay][0].manaCost + "<br><br>Description: " + spellbookPages[pageToDisplay][0].spellDescription;
            descriptionLeft.SetActive(true);
            pictureLeft.transform.Find("Picture").GetComponent<RawImage>().texture = spellbookPages[pageToDisplay][0].spellPicture;
            pictureLeft.SetActive(true);
            numberLeft.GetComponent<TextMeshProUGUI>().text = (((pageToDisplay + 1) * 2) - 1).ToString();
            numberLeft.SetActive(true);
        }

        if (spellbookPages[pageToDisplay].Count == 2)
        {
            titleRight.GetComponent<TextMeshProUGUI>().text = spellbookPages[pageToDisplay][1].spellName;
            titleRight.SetActive(true);
            descriptionRight.GetComponent<TextMeshProUGUI>().text = 
                "Mana cost: " + spellbookPages[pageToDisplay][1].manaCost + "<br><br>Description: " + spellbookPages[pageToDisplay][1].spellDescription;
            descriptionRight.SetActive(true);
            pictureRight.transform.Find("Picture").GetComponent<RawImage>().texture = spellbookPages[pageToDisplay][1].spellPicture;
            pictureRight.SetActive(true);
            numberRight.GetComponent<TextMeshProUGUI>().text = ((pageToDisplay + 1) * 2).ToString();
            numberRight.SetActive(true);
        }

        if (pageToDisplay > 0) arrowLeft.SetActive(true);

        if (spellbookPages.Count > pageToDisplay + 1) arrowRight.SetActive(true);
    }

    public void AddSpell(SpellScrollInfo spellToAdd)
    {
        if(spellToAdd != null)
        {
            spells.Add(spellToAdd);
        }
    }

    public SpellScrollInfo GetSpellInfo(string spellName)
    {
        foreach(SpellScrollInfo s in spells)
        {
            if(s.spellName == spellName)
            {
                return s;
            }
        }

        return null;
    }
}