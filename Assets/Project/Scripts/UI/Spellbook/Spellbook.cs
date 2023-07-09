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
    public bool bookOwned = false;
    public bool ableToInteract = true;

    private bool spellbookOpened = false;
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
        //Open or close inventory
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!spellbookOpened && bookOwned) OpenSpellbook();
            else if (spellbookOpened) CloseSpellbook();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && spellbookOpened)
        {
            CloseSpellbook();
        }

        //Play pointed voice
        if (Input.GetKeyDown(KeyCode.Space) && spellbookOpened)
        {

        }

        //Go left if possible
        if (Input.GetKeyDown(KeyCode.A) && spellbookOpened)
        {
            if (pointed == 2)
            {
                pointed = 1;
                PointOption(pointed);
            }
            else if (pointed == 1 && page > 0)
            {
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
                pointed = 2;
                PointOption(pointed);
            }
            else if (pointed == 2 && page + 1 < spellbookPages.Count)
            {
                page++;
                DisplayPage(page);
                pointed = 1;
                PointOption(pointed);
            }
        }
    }

    void OpenSpellbook()
    {
        //Instatiate inventory and assign it to UiCamera
        instantiatedSpellbook = Instantiate(spellbookPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        instantiatedSpellbook.GetComponent<Canvas>().worldCamera = UiCamera;
        instantiatedSpellbook.GetComponent<Canvas>().planeDistance = 1.05f;

        //Disable other controls (close first, because it activates movement and enable other ui)
        PlayerParams.Controllers.inventory.CloseInventory();
        PlayerParams.Controllers.inventory.ableToInteract = false;
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

        //Display first page if there are items in inventory
        page = 0;
        if (spellbookPages.Count > 0)
        {
            DisplayPage(page);
            pointed = 1;
            PointOption(pointed);
        }
        spellbookOpened = true;
    }

    public void CloseSpellbook()
    {
        Destroy(instantiatedSpellbook);
        spellbookOpened = false;

        //Enable other controls
        PlayerParams.Controllers.inventory.ableToInteract = true;
        PlayerParams.Variables.uiActive = false;
        PlayerParams.Objects.hand.SetActive(true);
    }
    void PointOption(int option)
    {
        if (option == 1)
        {
            //Change color of pointed option to white (255, 255, 255)
            //titleLeft.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
            //Change color of not pointed option to lightGrey (118, 118, 118)
            //titleRight.GetComponent<TextMeshProUGUI>().color = new Color(0.4625f, 0.4625f, 0.4625f);
            pointerRight.SetActive(false);
            pointerLeft.SetActive(true);
        }
        else if (option == 2)
        {
            //Change color of not pointed option to lightGrey (118, 118, 118)
            //titleLeft.GetComponent<TextMeshProUGUI>().color = new Color(0.4625f, 0.4625f, 0.4625f);
            //Change color of pointed option to white (255, 255, 255)
            //titleRight.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
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

        //Activate correct texts, arrows etc. and assign correct things to it
        if (spellbookPages[pageToDisplay].Count > 0)
        {
            titleLeft.GetComponent<TextMeshProUGUI>().text = spellbookPages[pageToDisplay][0].spellName;
            titleLeft.SetActive(true);
            descriptionLeft.GetComponent<TextMeshProUGUI>().text = spellbookPages[pageToDisplay][0].spellDescription;
            descriptionLeft.SetActive(true);
            pictureLeft.GetComponent<RawImage>().texture = spellbookPages[pageToDisplay][0].spellPicture;
            pictureLeft.SetActive(true);
        }

        if (spellbookPages[pageToDisplay].Count == 2)
        {
            titleRight.GetComponent<TextMeshProUGUI>().text = spellbookPages[pageToDisplay][1].spellName;
            titleRight.SetActive(true);
            descriptionRight.GetComponent<TextMeshProUGUI>().text = spellbookPages[pageToDisplay][1].spellDescription;
            descriptionRight.SetActive(true);
            pictureRight.GetComponent<RawImage>().texture = spellbookPages[pageToDisplay][1].spellPicture;
            pictureRight.SetActive(true);
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