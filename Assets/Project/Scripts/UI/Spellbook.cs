using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    //[Header("Voices")]
    private bool voiceIsPlaying;
    private AudioSource closeSound;
    private AudioSource openSound;
    private AudioSource changeSound;
    private AudioSource lightVoice;
    private AudioSource pickUpVoice;
    
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
                openSound.Play();
            }
            else if (spellbookOpened) 
            {
                closeSound.Play();
                CloseSpellbook();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && spellbookOpened)
        {
            closeSound.Play();
            CloseSpellbook();
        }

        //Play pointed voice
        if (Input.GetKeyDown(KeyCode.Space) && spellbookOpened)
        {

            if (lightVoice.isPlaying || pickUpVoice.isPlaying) voiceIsPlaying = true;
            //if (lightVoice.isPlaying || fireVoice.isPlaying etc.) voiceIsPlaying = true; ^in place of that
            else voiceIsPlaying = false;

            if (!voiceIsPlaying)
            {
                if (spellbookPages[page][pointed - 1].spellName == "Light") lightVoice.Play();
                if (spellbookPages[page][pointed - 1].spellName == "Pick Up") pickUpVoice.Play();
                //if (spellbookPages[page][pointed - 1].spellName == "Fire") fireVoice.Play(); etc.
            }
        

        }

        //Go left if possible
        if (Input.GetKeyDown(KeyCode.A) && spellbookOpened)
        {
            if (pointed == 2)
            {
                changeSound.Play();
                pointed = 1;
                PointOption(pointed);
            }
            else if (pointed == 1 && page > 0)
            {
                changeSound.Play();
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
                changeSound.Play();
                pointed = 2;
                PointOption(pointed);
            }
            else if (pointed == 2 && page + 1 < spellbookPages.Count)
            {
                changeSound.Play();
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

        //Assign proper voices
        openSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_Open);
        closeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_Close);
        changeSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_ChangeOption);
        lightVoice = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.READING_Light);
        lightVoice.volume *= 2f;
        pickUpVoice = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.READING_PickUp);
        pickUpVoice.volume *= 2f;

        //Disable other controls (close first, because it activates movement and enable other ui)
        PlayerParams.Controllers.inventory.CloseInventory();
        PlayerParams.Controllers.pauseMenu.CloseMenu();
        PlayerParams.Controllers.journal.CloseJournal();
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
        pictureLeft = instantiatedSpellbook.transform.Find("Left spell").Find("Picture").gameObject;
        pictureRight = instantiatedSpellbook.transform.Find("Right spell").Find("Picture").gameObject;
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
        if(spellbookOpened)
        {
            Destroy(openSound.gameObject, openSound.clip.length);
            Destroy(closeSound.gameObject, closeSound.clip.length);
            Destroy(changeSound.gameObject, changeSound.clip.length);
            Destroy(lightVoice.gameObject, lightVoice.clip.length);
            //Destroy(fireVoice.gameObject); etc.
        }
        Destroy(instantiatedSpellbook);
        spellbookOpened = false;

        //Enable other controls
        PlayerParams.Controllers.pauseMenu.ableToInteract = true;
        PlayerParams.Variables.uiActive = false;
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
            descriptionLeft.GetComponent<TextMeshProUGUI>().text = spellbookPages[pageToDisplay][0].spellDescription;
            descriptionLeft.SetActive(true);
            pictureLeft.GetComponent<RawImage>().texture = spellbookPages[pageToDisplay][0].spellPicture;
            pictureLeft.SetActive(true);
            numberLeft.GetComponent<TextMeshProUGUI>().text = (((pageToDisplay + 1) * 2) - 1).ToString();
            numberLeft.SetActive(true);
        }

        if (spellbookPages[pageToDisplay].Count == 2)
        {
            titleRight.GetComponent<TextMeshProUGUI>().text = spellbookPages[pageToDisplay][1].spellName;
            titleRight.SetActive(true);
            descriptionRight.GetComponent<TextMeshProUGUI>().text = spellbookPages[pageToDisplay][1].spellDescription;
            descriptionRight.SetActive(true);
            pictureRight.GetComponent<RawImage>().texture = spellbookPages[pageToDisplay][1].spellPicture;
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