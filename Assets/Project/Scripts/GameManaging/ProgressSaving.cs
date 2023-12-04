using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using static ProgressSaving.SaveData.JournalSave.DialoguesDict;

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
        //loading game state
        PlotPointsManager plotPointsManager = FindObjectOfType<PlotPointsManager>();
        plotPointsManager.plotPoints = saveData.gameStateSave.plotPoints;

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
        if (saveData.spellsSave.pickUp)
        {
            spellbook.AddSpell(spellScrollsHolder.GiveScroll("Pick Up"));
        }
        if (saveData.spellsSave.fire)
        {
            spellbook.AddSpell(spellScrollsHolder.GiveScroll("Fire"));
        }
        if (saveData.spellsSave.markAndReturn)
        {
            spellbook.AddSpell(spellScrollsHolder.GiveScroll("Mark And Return"));
        }

        //loading journal contents
        Journal journal = FindObjectOfType<Journal>();
        journal.notesJournal = saveData.journalSave.notes.DeserializeNotes();
        journal.dialoguesJournal = saveData.journalSave.dialogues.DeserializeDialogues();
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
    public void SaveGameState(string currentLvl, float plotPoints)
    {
        saveData.gameStateSave.currentLvl = currentLvl;
        saveData.gameStateSave.plotPoints = plotPoints;
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
        saveData.spellsSave.pickUp = spells.Exists(s => string.Equals(s, "pick up", StringComparison.OrdinalIgnoreCase));
        saveData.spellsSave.fire = spells.Exists(s => string.Equals(s, "fire", StringComparison.OrdinalIgnoreCase));
        saveData.spellsSave.markAndReturn = spells.Exists(s => string.Equals(s, "mark and return", StringComparison.OrdinalIgnoreCase));
    }

    public void SaveJournal(Dictionary<string, string> notesJournal, Dictionary<string, List<List<string>>> dialoguesJournal) //saving journal contents
    {
        saveData.journalSave.notes.SerializeNotes(notesJournal);
        saveData.journalSave.dialogues.SerializeDialogues(dialoguesJournal);
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
        public JournalSave journalSave = new JournalSave();

        [System.Serializable]
        public class GameStateSave //for saving current level
        {
            public string currentLvl = "Intro";
            public float plotPoints = 0.0f;
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
            public bool pickUp = false;
            public bool fire = false;
            public bool markAndReturn = false;
        }

        [System.Serializable]
        public class JournalSave //for saving dialogues and notes from journal
        {
            public NotesDict notes = new NotesDict();
            public DialoguesDict dialogues = new DialoguesDict();

            [System.Serializable]
            public class NotesDict
            {
                public List<string> notesTitles = new List<string>();
                public List<string> notesContents = new List<string>();

                public void SerializeNotes(Dictionary<string, string> notesToSave)
                {
                    notesTitles = new List<string>(notesToSave.Keys);
                    notesContents = new List<string>(notesToSave.Values);
                }

                public Dictionary<string, string> DeserializeNotes()
                {
                    Dictionary<string, string> notesDict = new Dictionary<string, string>();
                    for (int i = 0; i < notesTitles.Count; i++)
                    {
                        notesDict[notesTitles[i]] = notesContents[i];
                    }
                    return notesDict;
                }

            }

            [System.Serializable]
            public class DialoguesDict
            {
                public List<string> dialoguesTitles = new List<string>();
                public List<Serialization_Dialogues> dialoguesContents = new List<Serialization_Dialogues>();

                public void SerializeDialogues(Dictionary<string, List<List<string>>> dialoguesToSave)
                {
                    dialoguesTitles = new List<string>(dialoguesToSave.Keys);
                    dialoguesContents = new List<Serialization_Dialogues>();

                    foreach (List<List<string>> dialogue in dialoguesToSave.Values)
                    {
                        Serialization_Dialogues dialogueToSerialize = new Serialization_Dialogues();
                        foreach (List<string> speaker in dialogue)
                        {
                            Serialization_Dialogues.Serialization_Speakers speakerToSerialize = new Serialization_Dialogues.Serialization_Speakers();
                            foreach (string text in speaker)
                            {
                                speakerToSerialize.text.Add(text);
                            }
                            dialogueToSerialize.speakers.Add(speakerToSerialize);
                        }
                        dialoguesContents.Add(dialogueToSerialize);
                    }
                }

                public Dictionary<string, List<List<string>>> DeserializeDialogues()
                {
                    Dictionary<string, List<List<string>>> dialoguesDict = new Dictionary<string, List<List<string>>>();
                    for (int i = 0; i < dialoguesTitles.Count; i++)
                    {
                        List<List<string>> dialogueToDeserialize = new List<List<string>>();
                        foreach (Serialization_Dialogues.Serialization_Speakers speaker in dialoguesContents[i].speakers)
                        {
                            List<string> speakerToDeserialize = new List<string>();
                            foreach (string text in speaker.text)
                            {
                                speakerToDeserialize.Add(text);
                            }
                            dialogueToDeserialize.Add(speakerToDeserialize);
                        }
                        dialoguesDict[dialoguesTitles[i]] = dialogueToDeserialize;
                    }
                    return dialoguesDict;
                }

                [System.Serializable]
                public class Serialization_Dialogues
                {
                    public List<Serialization_Speakers> speakers = new List<Serialization_Speakers>();

                    [System.Serializable]
                    public class Serialization_Speakers
                    {
                        public List<string> text = new List<string>();
                    }
                }
            }
        }
    }
}
