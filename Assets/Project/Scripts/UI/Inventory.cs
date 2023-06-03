using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    //List of prefabs
    public List<GameObject> inventory = new List<GameObject>();
    public GameObject inventoryPrefab;
    public Camera UiCamera;

    private bool inventoryOpened = false;
    private int page;

    private GameObject instantiatedInventory;
    private GameObject player;

    private List<GameObject> itemSlots;
    private List<GameObject> itemIconActiveInstances = new List<GameObject>();
    private List<List<GameObject>> inventoryPages;

    private void Start()
    {
        player = gameObject;
    }

    void Update()
    {
       KeysListener();
    }

    void KeysListener()
    {
        //Open or close inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!inventoryOpened) OpenInventory();
            else CloseInventory();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && inventoryOpened)
        {
            CloseInventory();
        }

        //Change page left if possible
        if (Input.GetKeyDown(KeyCode.A) && inventoryOpened)
        {
            if (page > 0)
            {
                page--;
                DisplayPage(page);
            }
        }

        //Change page right if possible
        if (Input.GetKeyDown(KeyCode.D) && inventoryOpened)
        {
            if (page+1 < inventoryPages.Count)
                page++;
                DisplayPage(page);
        }
    }

    void OpenInventory()
    {
        //Instatiate inventory and assign it to UiCamera
        instantiatedInventory = Instantiate(inventoryPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        instantiatedInventory.GetComponent<Canvas>().worldCamera = UiCamera;
        instantiatedInventory.GetComponent<Canvas>().planeDistance = 10f; 
        //^ tu coœ nie dzia³a, ¿e po instatiate on nie jest od razu tam, gdzie ma byæ i pierwszy page jest jakoœ zbugowany je¿eli chodzi o pozycje

        //Disable player movement
        player.GetComponent<AdvanceTestMovement>().enabled = false;

        //Create list of slots for items to display on one page
        itemSlots = new List<GameObject>();
        for (int i = 0; i < 3; i++)
        {
            itemSlots.Add(instantiatedInventory.transform.Find("Items").Find("Top").Find((i+1).ToString()).gameObject);
        }
        for (int i = 0; i < 3; i++)
        {
            itemSlots.Add(instantiatedInventory.transform.Find("Items").Find("Bottom").Find((i + 4).ToString()).gameObject);
        }

        //Divide items to pages
        inventoryPages = new List<List<GameObject>>();
        int pageToAdd = -1;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (i%6 == 0)
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

        //Display first page if there are items in inventory
        page = 0;
        if (inventoryPages.Count > 0)
        {
            DisplayPage(page);
        }
        inventoryOpened = true;
    }

    void CloseInventory()
    {
        Destroy(instantiatedInventory);
        inventoryOpened = false;

        //Enable player movement
        player.GetComponent<AdvanceTestMovement>().enabled = true;
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
        instantiatedInventory.transform.Find("Background").Find("ArrowRight").gameObject.SetActive(false);
        instantiatedInventory.transform.Find("Background").Find("ArrowLeft").gameObject.SetActive(false);

        //Activate correct item slots, spawn icons and arrows
        for (int i = 0; i < inventoryPages[pageToDisplay].Count; i++)
        {
            itemSlots[i].SetActive(true);
            itemSlots[i].transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = inventoryPages[pageToDisplay][i].name;

            itemIconActiveInstances.Add(Instantiate(inventoryPages[pageToDisplay][i], itemSlots[i].transform));
            itemIconActiveInstances[i].transform.localScale = new Vector3(200f, 200f, 200f);
            itemIconActiveInstances[i].GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            itemIconActiveInstances[i].layer = LayerMask.NameToLayer("UI");
            itemIconActiveInstances[i].transform.rotation *= Quaternion.Euler(0, 90, 0);
        }
        if (pageToDisplay > 0) instantiatedInventory.transform.Find("Background").Find("ArrowLeft").gameObject.SetActive(true);
        if (inventoryPages.Count > pageToDisplay + 1) instantiatedInventory.transform.Find("Background").Find("ArrowRight").gameObject.SetActive(true);
    }
}