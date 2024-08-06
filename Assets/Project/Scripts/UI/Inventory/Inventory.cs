using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class Inventory : MonoBehaviour
{
    //List of prefabs
    public List<GameObject> inventory = new List<GameObject>();
    [Header("Game objects")]
    public GameObject inventoryPrefab;
    public Camera UiCamera;

    [Header("Settings")]
    public bool ableToInteract = true;
    public bool inventoryOpened = false;

    private int page;

    private GameObject instantiatedInventory;

    private List<GameObject> itemSlots;
    private List<GameObject> itemIconActiveInstances = new List<GameObject>();
    private List<List<GameObject>> inventoryPages;

    private AudioSource equipSound;
    private AudioSource closeSound;
    private AudioSource openSound;
    private AudioSource changeSound;

    void Update()
    {
        if (ableToInteract)
        {
            KeysListener();
        }

        if (ableToInteract && inventoryOpened)
        {
            PointIcon();
        }
    }

    void KeysListener()
    {
        //Open or close inventory
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
        {
            if (!inventoryOpened && PlayerParams.Controllers.handInteractions.inHand != null)
            {
                if (PlayerParams.Controllers.handInteractions.inHand.GetComponent<ItemBehavior>() != null)
                {
                    PlayerParams.Controllers.handInteractions.PutDownObject();
                }
                OpenInventory();
                openSound.Play();
            }
            else if (!inventoryOpened)
            {
                OpenInventory();
                openSound.Play();
            }
            else
            {
                closeSound.Play();
                CloseInventory();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && inventoryOpened)
        {
            closeSound.Play();
            CloseInventory();
        }

        //Change page left if possible
        if (Input.GetKeyDown(KeyCode.A) && inventoryOpened)
        {
            if (page > 0)
            {
                changeSound.Play();
                page--;
                DisplayPage(page);
            }
        }

        //Change page right if possible
        if (Input.GetKeyDown(KeyCode.D) && inventoryOpened)
        {
            if (page + 1 < inventoryPages.Count)
            {
                changeSound.Play();
                page++;
                DisplayPage(page);
            }
        }
    }

    void OpenInventory()
    {
        //Instatiate inventory and assign it to UiCamera
        instantiatedInventory = Instantiate(inventoryPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        instantiatedInventory.GetComponent<Canvas>().worldCamera = UiCamera;
        instantiatedInventory.GetComponent<Canvas>().planeDistance = 1.05f;

        //Disable other controls (close first, because it activates movement and enable other ui)
        PlayerParams.Controllers.spellbook.CloseSpellbook();
        PlayerParams.Controllers.journal.CloseJournal();
        PlayerParams.Controllers.pauseMenu.CloseMenu();
        PlayerParams.Controllers.pauseMenu.ableToInteract = false;
        PlayerParams.Variables.uiActive = true;

        openSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.UI_Open);
        closeSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.UI_Close);
        changeSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.UI_ChangeOption);

        //Create list of slots for items to display on one page
        itemSlots = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            itemSlots.Add(instantiatedInventory.transform.Find("Items").Find("Top").Find((i + 1).ToString()).gameObject);
        }
        for (int i = 0; i < 4; i++)
        {
            itemSlots.Add(instantiatedInventory.transform.Find("Items").Find("Bottom").Find((i + 4).ToString()).gameObject);
        }

        //Divide items to pages
        inventoryPages = new List<List<GameObject>>();
        int pageToAdd = -1;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (i % 8 == 0)
            {
                pageToAdd++;
                inventoryPages.Add(new List<GameObject>());
                inventoryPages[pageToAdd].Add(inventory[i]);
            }
            else
            {
                inventoryPages[pageToAdd].Add(inventory[i]);
            }
        }

        //Display currency counter
        instantiatedInventory.transform.Find("Background").Find("InventoryBackground").Find("MoneyCounter")
            .gameObject.GetComponent<TextMeshProUGUI>().text = ": " + PlayerParams.Controllers.pointsManager.currency.ToString();

        //Display first page if there are items in inventory
        page = 0;
        if (inventoryPages.Count > 0)
        {
            instantiatedInventory.transform.Find("Background").Find("InventoryBackground").Find("Empty").gameObject.SetActive(false);
            instantiatedInventory.transform.Find("Background").Find("InventoryBackground").Find("Title").gameObject.SetActive(true);
            DisplayPage(page);
        }
        else
        {
            instantiatedInventory.transform.Find("Background").Find("InventoryBackground").Find("Empty").gameObject.SetActive(true);
            instantiatedInventory.transform.Find("Background").Find("InventoryBackground").Find("Title").gameObject.SetActive(false);
        }
        inventoryOpened = true;
    }

    public void CloseInventory()
    {
        Destroy(instantiatedInventory);
        if (inventoryOpened)
        {
            Destroy(openSound.gameObject, openSound.clip.length);
            Destroy(closeSound.gameObject, closeSound.clip.length);
            Destroy(changeSound.gameObject, changeSound.clip.length);
        }
        inventoryOpened = false;

        //Enable other controls
        PlayerParams.Variables.uiActive = false;
        PlayerParams.Controllers.pauseMenu.ableToInteract = true;
    }

    void DisplayPage(int pageToDisplay)
    {
        //Deactivate item slots, destroy icons and arrows
        for (int i = 0; i < itemSlots.Count; i++)
        {
            itemSlots[i].SetActive(false);
        }
        for (int i = 0; i < itemIconActiveInstances.Count; i++)
        {
            Destroy(itemIconActiveInstances[i]);
        }
        itemIconActiveInstances.Clear();
        instantiatedInventory.transform.Find("Background").Find("InventoryBackground").Find("ArrowRight").gameObject.SetActive(false);
        instantiatedInventory.transform.Find("Background").Find("InventoryBackground").Find("ArrowLeft").gameObject.SetActive(false);

        //Activate correct item slots, spawn icons and arrows
        for (int i = 0; i < inventoryPages[pageToDisplay].Count; i++)
        {
            itemSlots[i].SetActive(true);
            itemSlots[i].transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = inventoryPages[pageToDisplay][i].GetComponent<ItemBehavior>().itemName;
            GameObject itemIcon = Instantiate(inventoryPages[pageToDisplay][i].GetComponent<ItemBehavior>().itemIcon, itemSlots[i].transform);
            itemIcon.GetComponent<IconParameters>().originalObject = inventoryPages[pageToDisplay][i];
            itemIconActiveInstances.Add(itemIcon);
            itemIconActiveInstances[i].transform.localScale = new Vector3(200f, 200f, 200f);
            //itemIconActiveInstances[i].GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            itemIconActiveInstances[i].layer = LayerMask.NameToLayer("UI");
        }
        if (pageToDisplay > 0) instantiatedInventory.transform.Find("Background").Find("InventoryBackground").Find("ArrowLeft").gameObject.SetActive(true);
        if (inventoryPages.Count > pageToDisplay + 1) instantiatedInventory.transform.Find("Background").Find("InventoryBackground").Find("ArrowRight").gameObject.SetActive(true);
    }

    public void AddItem(GameObject item)
    {
        if(item.tag == "Treasure")
        {
            PlayerParams.Controllers.pointsManager.currency += item.GetComponent<TreasureBehavior>().value;
            Destroy(item);
        }
        else
        {
            inventory.Add(item);
            inventory = inventory.OrderBy(go => go.GetComponent<ItemBehavior>().itemName).ToList();
            item.SetActive(false);
        }
        
        try
        {
            PlayerParams.Controllers.handInteractions.inHand = null;
        }
        catch
        {
            Debug.Log("PlayerParams.Controllers.handInteractions.inHand is not detected");
        }
    }

    public bool HasItem(string itemName) 
    {
        foreach(GameObject item in inventory)
        {
            if (item.GetComponent<ItemBehavior>().itemName == itemName)
            {
                return true;
            }
        }
        return false;
    }

    //Probably could be done better to lag less
    public void PointIcon()
    {
        if (inventoryPages.Count > 0)
        {
            for (int i = 0; i < itemIconActiveInstances.Count; i++)
            {
                //Change background of pointed item to white (255, 255, 255)
                if (itemIconActiveInstances[i].transform.Find("Icon").GetComponent<EnlightObject>() != null)
                {
                    itemIconActiveInstances[i].transform.parent.GetComponent<Image>().color = new Color(1f, 1f, 1f);
                    //itemIconActiveInstances[i].transform.parent.transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
                }
                //Change background of pointed item to darkGrey (68, 68, 68)
                else if (itemIconActiveInstances[i].transform.parent.GetComponent<Image>().color == new Color(1f, 1f, 1f))
                {
                    itemIconActiveInstances[i].transform.parent.GetComponent<Image>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                    //itemIconActiveInstances[i].transform.parent.transform.Find("Name").GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                }
            }
        }
    }
}