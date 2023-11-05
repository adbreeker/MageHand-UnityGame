using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ProgressSaving : MonoBehaviour
{
    [Header("Save name:")]
    public static string saveName = "TestSaveDevEdit";

    [Header("Holders")] //for save loading
    public ItemHolder itemHolder;
    public SpellScrollsHolder spellScrollsHolder;

    private string savePath;
    private SaveData saveData;

    private void Awake() //geting json file with save, or creating new if no save exists
    {
        savePath = Path.Combine(Application.persistentDataPath, "Saves", saveName+".json");

        CreateNewSave("test1");
        DeleteExistingSave("test1");

        try
        {
            saveData = LoadProgress();
        }
        catch
        {
            //Debug.Log("error: no existing save");
            saveData = new SaveData();
            SaveProgressToFile();
        }

        ManageLoadedData();

    } 

    // ------------------------------------------------------------ loading data

    public SaveData LoadProgress() //get save data from json
    {
        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    void ManageLoadedData() //managing loaded data
    {
        //loading inventory
        Inventory inventory = FindObjectOfType<Inventory>();
        foreach(string itemInData in saveData.itemsSave.items)
        {
            inventory.AddItem(itemHolder.GiveItem(itemInData));
        }

        //loading spellbook and spells
        Spellbook spellbook = FindObjectOfType<Spellbook>();
        if(saveData.spellsSave.spellBook)
        {
            spellbook.bookOwned = true;
        }

        if(saveData.spellsSave.light)
        {
            spellbook.AddSpell(spellScrollsHolder.GiveScroll("Light"));
        }
        if (saveData.spellsSave.light)
        {
            spellbook.AddSpell(spellScrollsHolder.GiveScroll("Fire"));
        }
        if (saveData.spellsSave.markAndReturn)
        {
            spellbook.AddSpell(spellScrollsHolder.GiveScroll("Mark And Return"));
        }
    }

    // ------------------------------------------------------------- saving data

    public void SaveGameState(string currentLvl)
    {
        saveData.gameStateSave.currentLvl = currentLvl;
    }

    public void SaveItems(List<GameObject> itemsToSave) //saving all "itemsToSave"
    {
        saveData.itemsSave.items = new List<string>();
        foreach(GameObject item in itemsToSave)
        {
            saveData.itemsSave.items.Add(item.GetComponent<ItemParameters>().itemName);
        }
    }

    public void SaveSpells(bool spellBook, List<string> spells) //saving spellbook state, and all "spells"
    {
        saveData.spellsSave.spellBook = spellBook;

        saveData.spellsSave.light = spells.Exists(s => string.Equals(s, "light", StringComparison.OrdinalIgnoreCase));
        saveData.spellsSave.fire = spells.Exists(s => string.Equals(s, "fire", StringComparison.OrdinalIgnoreCase));
        saveData.spellsSave.markAndReturn = spells.Exists(s => string.Equals(s, "mark and return", StringComparison.OrdinalIgnoreCase));
    }

    public void SaveProgressToFile() //saving progress to json file (the one from which data was loaded)
    {
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(savePath, json);
    }

    // -------------------------------------------------------------- save files managing

    public static string[] GetSaves()
    {
        string savesLocation = Path.Combine(Application.persistentDataPath, "Saves");
        string[] savesNames = Directory.GetFiles(savesLocation, "*.json");
        for(int i = 0; i<savesNames.Length; i++)
        {
            savesNames[i] = savesNames[i].Remove(0, savesLocation.Length + 1);
            savesNames[i] = savesNames[i].Remove(savesNames[i].Length - 5, 5);
        }
        return savesNames;
    }

    public static void CreateNewSave(string saveToCreate)
    {
        string saveToCreatePath = Path.Combine(Application.persistentDataPath, "Saves", saveToCreate + ".json");
        string json = JsonUtility.ToJson(new SaveData());
        File.WriteAllText(saveToCreatePath, json);
    }

    public static void DeleteExistingSave(string saveToDelete)
    {
        string saveToDeletePath = Path.Combine(Application.persistentDataPath, "Saves", saveToDelete + ".json");
        if(File.Exists(saveToDeletePath))
        {
            File.Delete(saveToDeletePath);
        }
    }

    //data to save classes ----------------------------------------------------------------------------------------------------- data to save classes


    [System.Serializable]
    public class SaveData
    {
        public GameStateSave gameStateSave = new GameStateSave();
        public ItemsSave itemsSave = new ItemsSave();
        public SpellsSave spellsSave = new SpellsSave();

        [System.Serializable]
        public class GameStateSave //for saving current level
        {
            public string currentLvl = "Level_1_Chapter_1";
        }

        [System.Serializable]
        public class ItemsSave //for saving all items from inventory
        {
            public List<string> items = new List<string>();
        }

        [System.Serializable]
        public class SpellsSave //for saving if spellbook was picked up, and all spells from spellbook 
        {
            public bool spellBook = false;
            public bool light = false;
            public bool fire = false;
            public bool markAndReturn = false;
        }
    }
}
