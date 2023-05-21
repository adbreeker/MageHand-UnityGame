using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    public List<GameObject> inventory = new List<GameObject>();
    public GameObject inventoryPrefab;

    private bool inventoryOpened = false;
    private int page;

    private GameObject instantiatedInventory;
    private GameObject player;

    private List<GameObject> itemSlots;
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

        //Change page left if possible
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (inventoryOpened && page > 0)
            {
                page--;
                DisplayPage(page);
            }
        }

        //Change page right if possible
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (inventoryOpened && page+1 < inventoryPages.Count)
                page++;
                DisplayPage(page);
        }
    }

    void OpenInventory()
    {
        instantiatedInventory = Instantiate(inventoryPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        itemSlots = new List<GameObject>();
        //Disable player movement
        player.GetComponent<AdvanceTestMovement>().enabled = false;

        //Create list of slots for items to display on one page
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

        //Display first page
        page = 0;
        DisplayPage(page);
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
        //Deactivate item slots and arrows
        for (int i = 0; i < itemSlots.Count; i++)
        {
            itemSlots[i].SetActive(false);
        }
        instantiatedInventory.transform.Find("Background").Find("ArrowRight").gameObject.SetActive(false);
        instantiatedInventory.transform.Find("Background").Find("ArrowLeft").gameObject.SetActive(false);

        //Activate correct item slots and arrows
        for (int i = 0; i < inventoryPages[pageToDisplay].Count; i++)
        {
            itemSlots[i].SetActive(true);
            itemSlots[i].transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = inventoryPages[pageToDisplay][i].name;
        }
        if (pageToDisplay > 0) instantiatedInventory.transform.Find("Background").Find("ArrowLeft").gameObject.SetActive(true);
        if (inventoryPages.Count > pageToDisplay + 1) instantiatedInventory.transform.Find("Background").Find("ArrowRight").gameObject.SetActive(true);
    }
}