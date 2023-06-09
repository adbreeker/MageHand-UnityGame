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
        public string currentLvl;
    }

    [System.Serializable]
    public class ItemsSave
    {
        public List<string> items;
    }

    [System.Serializable]
    public class SpellsSave
    {
        public bool spellBook = false;
        public bool light = false;
    }

    // ------------------------------------------- save data struct

    [System.Serializable]
    public struct SaveData
    {
        public GameStateSave gameStateSave;
        public ItemsSave itemsSave;
        public SpellsSave spellsSave;
    }

    // ------------------------------------------- code

    private string savePath;
    private SaveData saveData;

    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "saveData.json");
        Debug.Log("saves are on: " + savePath);

        saveData = LoadProgress();
    } 

    public SaveData LoadProgress()
    {
        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<SaveData>(json);
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
