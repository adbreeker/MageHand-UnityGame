using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using static ProgressSaving.SaveData.JournalSave.DialoguesDict;
using static ProgressSaving;

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
        PointsManager pointsManager = PlayerParams.Controllers.pointsManager;
        pointsManager.plotPoints = saveData.gameStateSave.plotPoints;
        pointsManager.foundSecrets = saveData.gameStateSave.foundSecrets;
        pointsManager.currency = saveData.gameStateSave.currency;

        pointsManager.maxPlotPoints = saveData.gameStateSave.maxPlotPoints;
        pointsManager.minPlotPoints = saveData.gameStateSave.minPlotPoints;
        pointsManager.maxFoundSecrets = saveData.gameStateSave.maxFoundSecrets;
        pointsManager.maxCurrency = saveData.gameStateSave.maxCurrency;

    //loading inventory
        Inventory inventory = PlayerParams.Controllers.inventory;
        foreach(string itemInData in saveData.itemsSave.items)
        {
            inventory.AddItem(itemHolder.GiveItem(itemInData));
        }

        //loading spellbook and spells
        Spellbook spellbook = PlayerParams.Controllers.spellbook;
        if(saveData.spellsSave.spellBook)
        {
            spellbook.bookOwned = true;
        }

        if(saveData.spellsSave.light)
        {
            spellbook.AddSpell(spellScrollsHolder.GiveScroll("Light"));
        }
        if (saveData.spellsSave.collect)
        {
            spellbook.AddSpell(spellScrollsHolder.GiveScroll("Collect"));
        }
        if (saveData.spellsSave.fire)
        {
            spellbook.AddSpell(spellScrollsHolder.GiveScroll("Fire"));
        }
        if (saveData.spellsSave.speak)
        {
            spellbook.AddSpell(spellScrollsHolder.GiveScroll("Speak"));
        }
        if (saveData.spellsSave.mark)
        {
            spellbook.AddSpell(spellScrollsHolder.GiveScroll("Mark"));
        }
        if (saveData.spellsSave.slow)
        {
            spellbook.AddSpell(spellScrollsHolder.GiveScroll("Slow"));
        }
        if (saveData.spellsSave.dispel)
        {
            spellbook.AddSpell(spellScrollsHolder.GiveScroll("Dispel"));
        }

        //loading journal contents
        Journal journal = PlayerParams.Controllers.journal;
        journal.notesJournal = saveData.journalSave.notes.DeserializeNotes();
        journal.dialoguesJournal = saveData.journalSave.dialogues.DeserializeDialogues();
    }

    // ------------------------------------------------------------- saving data
    public static void CreateSavesDirectory()
    {
        if(!Directory.Exists(Path.Combine(Application.persistentDataPath, "Saves")))
        {
            Debug.Log("Directory status: " + Directory.Exists(Path.Combine(Application.persistentDataPath, "Saves")));
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Saves"));
            Debug.Log("Directory status: " + Directory.Exists(Path.Combine(Application.persistentDataPath, "Saves")));
        }
    }

    public void SaveGameState(string currentLvl, int plotPoints, int foundSecrets, int currency, int maxPlotPoints, int minPlotPoints, int maxFoundSecrets, int maxCurrency)
    {
        saveData.gameStateSave.currentLvl = currentLvl;
        saveData.gameStateSave.plotPoints = plotPoints;
        saveData.gameStateSave.foundSecrets = foundSecrets;
        saveData.gameStateSave.currency = currency;

        saveData.gameStateSave.maxPlotPoints = maxPlotPoints;
        saveData.gameStateSave.minPlotPoints = minPlotPoints;
        saveData.gameStateSave.maxFoundSecrets = maxFoundSecrets;
        saveData.gameStateSave.maxCurrency = maxCurrency;
    }

    public void SaveItems(List<GameObject> itemsToSave) //saving all "itemsToSave"
    {
        saveData.itemsSave.items = new List<string>();
        foreach(GameObject item in itemsToSave)
        {
            saveData.itemsSave.items.Add(item.GetComponent<ItemBehavior>().itemName);
        }
    }

    public void SaveSpells(bool spellBook, List<string> spells) //saving spellbook state, and all "spells"
    {
        saveData.spellsSave.spellBook = spellBook;

        saveData.spellsSave.light = spells.Exists(s => string.Equals(s, "light", StringComparison.OrdinalIgnoreCase));
        saveData.spellsSave.collect = spells.Exists(s => string.Equals(s, "collect", StringComparison.OrdinalIgnoreCase));
        saveData.spellsSave.fire = spells.Exists(s => string.Equals(s, "fire", StringComparison.OrdinalIgnoreCase));
        saveData.spellsSave.speak = spells.Exists(s => string.Equals(s, "speak", StringComparison.OrdinalIgnoreCase));
        saveData.spellsSave.mark = spells.Exists(s => string.Equals(s, "mark", StringComparison.OrdinalIgnoreCase));
        saveData.spellsSave.slow = spells.Exists(s => string.Equals(s, "slow", StringComparison.OrdinalIgnoreCase));
        saveData.spellsSave.dispel = spells.Exists(s => string.Equals(s, "dispel", StringComparison.OrdinalIgnoreCase));
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
            public int plotPoints = 0;
            public int foundSecrets = 0;
            public int currency = 0;

            public int maxPlotPoints = 0;
            public int minPlotPoints = 0;
            public int maxFoundSecrets = 0;
            public int maxCurrency = 0;
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
            public bool collect = false;
            public bool fire = false;
            public bool speak = false;
            public bool mark = false;
            public bool slow = false;
            public bool dispel = false;
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
