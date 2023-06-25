using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ProgressSaving : MonoBehaviour
{
    // ----------------------------------- data to save classes

    [System.Serializable]
    public class GameStateSave
    {
        public string currentLvl = "";
    }

    [System.Serializable]
    public class ItemsSave
    {
        public List<string> items = new List<string>();
    }

    [System.Serializable]
    public class SpellsSave
    {
        public bool spellBook = false;
        public bool light = false;
    }

    // ------------------------------------------- save data struct

    [System.Serializable]
    public class SaveData
    {
        public GameStateSave gameStateSave = new GameStateSave();
        public ItemsSave itemsSave = new ItemsSave();
        public SpellsSave spellsSave = new SpellsSave();
    }

    // ------------------------------------------- code

    [Header("Holders")]
    public ItemHolder itemHolder;
    public SpellScrollsHolder spellScrollsHolder;

    private string savePath;
    private SaveData saveData;

    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "saveData.json");
        Debug.Log("saves are on: " + savePath);

        try
        {
            saveData = LoadProgress();
        }
        catch
        {
            Debug.Log("catched exceptiooooon");
            saveData = new SaveData();
        }

        ManageLoadedData();

    } 

    // ------------------------------------------------------------ loading data

    public SaveData LoadProgress()
    {
        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    void ManageLoadedData()
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        foreach(string itemInData in saveData.itemsSave.items)
        {
            inventory.AddItem(itemHolder.GiveItem(itemInData));
        }

        Spellbook spellbook = FindObjectOfType<Spellbook>();
        if(saveData.spellsSave.spellBook)
        {
            spellbook.bookOwned = true;
        }
        if(saveData.spellsSave.light)
        {
            spellbook.AddSpell(spellScrollsHolder.GiveScroll("Light"));
        }

    }

    // ------------------------------------------------------------- saving data

    public void SaveGameState(string currentLvl)
    {
        saveData.gameStateSave.currentLvl = currentLvl;
    }

    public void SaveItems(List<GameObject> itemsToSave)
    {
        saveData.itemsSave.items = new List<string>();
        foreach(GameObject item in itemsToSave)
        {
            saveData.itemsSave.items.Add(item.name);
        }
    }

    public void SaveSpells(bool spellBook, List<string> spells)
    {
        saveData.spellsSave.spellBook = spellBook;

        saveData.spellsSave.light = spells.Exists(s => string.Equals(s, "light", StringComparison.OrdinalIgnoreCase));
    }

    public void SaveProgressToFile()
    {
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(savePath, json);
    }


}
