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
        CreateSavesDirectory();

        try
        {
            string json = File.ReadAllText(savePath);
            saveData = JsonUtility.FromJson<SaveData>(json);
        }
        catch
        {
            Debug.Log("error: no existing save");
            saveData = new SaveData();
            SaveProgressToFile();
        }

        ManageLoadedData();
    } 

    // ------------------------------------------------------------ loading data


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

        //loading dialogue diary
        DialogueDiary dialogueDiary = FindObjectOfType<DialogueDiary>();
        dialogueDiary.dialogueDiary = new Dictionary<string, List<List<string>>>();

        for(int i = 0; i < saveData.dialogueDiarySave.diaryTitles.Count; i++)
        {
            List<List<string>> note = new List<List<string>>();
            foreach(SaveData.DialogueDiarySave.DictionarySerializationNote.InnerNote serializedNote in saveData.dialogueDiarySave.diaryNotes[i].innerNotes)
            {
                List<string> innerNote = new List<string>();
                foreach(string innerSerializedNote in serializedNote.innerInnerNotes)
                {
                    innerNote.Add(innerSerializedNote);
                }
                note.Add(innerNote);
            }
            dialogueDiary.dialogueDiary[saveData.dialogueDiarySave.diaryTitles[i]] = note;
        }
        
    }

    // ------------------------------------------------------------- saving data
    void CreateSavesDirectory()
    {
        if(!Directory.Exists(Path.Combine(Application.persistentDataPath, "Saves")))
        {
            Debug.Log("Directory status: " + Directory.Exists(Path.Combine(Application.persistentDataPath, "Saves")));
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Saves"));
            Debug.Log("Directory status: " + Directory.Exists(Path.Combine(Application.persistentDataPath, "Saves")));
        }
    }
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

    public void SaveDialogueDiary(Dictionary<string, List<List<string>>> diaryToSave)
    {
        saveData.dialogueDiarySave.diaryTitles = new List<string>(diaryToSave.Keys);
        saveData.dialogueDiarySave.diaryNotes = new List<SaveData.DialogueDiarySave.DictionarySerializationNote>();

        foreach(List<List<string>> note in diaryToSave.Values)
        {
            SaveData.DialogueDiarySave.DictionarySerializationNote noteToSerialize = new SaveData.DialogueDiarySave.DictionarySerializationNote();
            foreach (List<string> innerNote in note)
            {
                SaveData.DialogueDiarySave.DictionarySerializationNote.InnerNote innerNoteToSerialize = new SaveData.DialogueDiarySave.DictionarySerializationNote.InnerNote();
                foreach(string innerInnerNote in innerNote)
                {
                    innerNoteToSerialize.innerInnerNotes.Add(innerInnerNote);
                }
                noteToSerialize.innerNotes.Add(innerNoteToSerialize);
            }
            saveData.dialogueDiarySave.diaryNotes.Add(noteToSerialize);
        }
        
    }

    public void SaveProgressToFile() //saving progress to json file (the one from which data was loaded)
    {
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(savePath, json);
    }

    // -------------------------------------------------------------- save files managing

    public static List<string> GetSaves() //returnig list of existing saves names
    {
        string savesLocation = Path.Combine(Application.persistentDataPath, "Saves");
        string[] savesNames = Directory.GetFiles(savesLocation, "*.json");
        for(int i = 0; i<savesNames.Length; i++)
        {
            savesNames[i] = savesNames[i].Remove(0, savesLocation.Length + 1);
            savesNames[i] = savesNames[i].Remove(savesNames[i].Length - 5, 5);
        }
        List<string> listOfNames = new List<string>(savesNames);
        return listOfNames;
    }

    public static void CreateNewSave(string saveToCreate) //creating new save
    {
        string saveToCreatePath = Path.Combine(Application.persistentDataPath, "Saves", saveToCreate + ".json");
        string json = JsonUtility.ToJson(new SaveData());
        File.WriteAllText(saveToCreatePath, json);
    }

    public static void DeleteExistingSave(string saveToDelete) //deleting existing save
    {
        string saveToDeletePath = Path.Combine(Application.persistentDataPath, "Saves", saveToDelete + ".json");
        if(File.Exists(saveToDeletePath))
        {
            File.Delete(saveToDeletePath);
        }
    }

    public static SaveData GetSaveByName(string saveToGet)
    {
        SaveData chosenSave;
        try
        {
            string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "Saves", saveToGet + ".json"));
            chosenSave = JsonUtility.FromJson<SaveData>(json);
        }
        catch
        {
            Debug.LogError("Save: " + saveToGet + " does not exist");
            chosenSave = new SaveData();
        }
        return chosenSave;
    }

    public static string GetRecentlyChangedSave()
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "Saves");

        FileInfo mostRecentFile;

        if (Directory.Exists(directoryPath))
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            FileInfo[] files = directoryInfo.GetFiles();

            if (files.Length > 0)
            {
                mostRecentFile = files[0];

                foreach (FileInfo file in files)
                {
                    if (file.LastWriteTime > mostRecentFile.LastWriteTime)
                    {
                        mostRecentFile = file;
                    }
                }
            }
            else mostRecentFile = null;
        }
        else mostRecentFile = null;

        return Path.GetFileNameWithoutExtension(mostRecentFile.Name);
    }

    public static string GetChangeDateOfSaveByName(string save)
    {
        FileInfo fileInfo;
        try
        {
            fileInfo = new FileInfo(Path.Combine(Application.persistentDataPath, "Saves", save + ".json"));
        }
        catch
        {
            Debug.LogError("Save: " + save + " does not exist");
            return null;
        }
        return fileInfo.LastWriteTime.ToString("yyyy-MM-dd<br>HH:mm:ss");
    }

    //data to save classes ----------------------------------------------------------------------------------------------------- data to save classes


    [System.Serializable]
    public class SaveData
    {
        public GameStateSave gameStateSave = new GameStateSave();
        public ItemsSave itemsSave = new ItemsSave();
        public SpellsSave spellsSave = new SpellsSave();
        public DialogueDiarySave dialogueDiarySave = new DialogueDiarySave();

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

        [System.Serializable]
        public class DialogueDiarySave
        {
            public List<string> diaryTitles = new List<string>();
            public List<DictionarySerializationNote> diaryNotes = new List<DictionarySerializationNote>();

            [System.Serializable]
            public class DictionarySerializationNote
            {
                public List<InnerNote> innerNotes = new List<InnerNote>();

                [System.Serializable]
                public class InnerNote
                {
                    public List<string> innerInnerNotes = new List<string>();
                }
            }
        }
    }
}
