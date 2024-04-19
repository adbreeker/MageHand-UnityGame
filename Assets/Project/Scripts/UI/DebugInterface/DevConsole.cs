using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO.MemoryMappedFiles;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DevConsole : MonoBehaviour
{
    [Header("Objects of DebugCanvas")]
    public TMP_InputField console;
    public GameObject fpsCounter;
    public GameObject webCamera;

    //commands history
    private List<string> previousCommands = new List<string>();
    int currentCommandOnList = 0;

    //currently active commands
    bool ghostmode = false;
    bool fps = false;
    bool webcam = false;


    void Update()
    {
        // turn on dev console
        if(Input.GetKey(KeyCode.Backspace) && Input.GetKeyDown(KeyCode.Slash))
        {
            SetStateOfOtherPlayerListeners(false);
            console.text = "";
            currentCommandOnList = 0;
            console.gameObject.SetActive(true);
            Time.timeScale = 0;
            console.ActivateInputField();
        }

        //if console is actually active, check other options
        if(console.IsActive())
        {
            //close without command
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = 1;
                console.gameObject.SetActive(false);
                SetStateOfOtherPlayerListeners(true);
            }

            //previous command
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentCommandOnList++;
                ShowPreviousCommand();
            }
            //next command
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentCommandOnList--;
                ShowPreviousCommand();
            }

            // active command and turn off dev console
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ExecuteCommand();
                Time.timeScale = 1;
                console.gameObject.SetActive(false);
                SetStateOfOtherPlayerListeners(true);
            }
        }
    }

    void SetStateOfOtherPlayerListeners(bool activate)
    {
        if(activate)
        {
            PlayerParams.Variables.uiActive = false;
            PlayerParams.Controllers.spellbook.ableToInteract = true;
            PlayerParams.Controllers.inventory.ableToInteract = true;
            PlayerParams.Controllers.journal.ableToInteract = true;
        }
        else
        {
            PlayerParams.Variables.uiActive = true;
            PlayerParams.Controllers.spellbook.ableToInteract = false;
            PlayerParams.Controllers.inventory.ableToInteract = false;
            PlayerParams.Controllers.journal.ableToInteract = false;
        }
    }

    void ShowPreviousCommand()
    {
        if(currentCommandOnList >= 1 && currentCommandOnList <= previousCommands.Count)
        {
            console.text = previousCommands[previousCommands.Count - currentCommandOnList];
            console.caretPosition = console.text.Length;
        }

        if(currentCommandOnList == 0)
        {
            console.text = "";
        }

        currentCommandOnList = Mathf.Clamp(currentCommandOnList, 0, previousCommands.Count + 1);
    }

    void ExecuteCommand()
    {
        string[] command = console.text.Split(' ');
        if(command.Length > 0)
        {
            previousCommands.Add(console.text);
        }

        // test command
        if(command[0] == "test" && command.Length == 1)
        {
            Debug.Log("dev console tested successfully");
            return;
        }

        //player and movement commands
        if(command[0] == "ghostmode" && command.Length == 1)
        {
            GhostMode();
            return;
        }

        //parameters commands
        if(command[0] == "fps" && command.Length == 1)
        {
            Fps();
            return;
        }
        if (command[0] == "camera" && command.Length == 1)
        {
            Camera();
            return;
        }

        //game saves commands
        if (command[0] == "savestate" && command.Length == 1)
        {
            SaveState();
            return;
        }

        if (command[0] == "resetstate" && command.Length == 1)
        {
            ResetState();
            return;
        }

        if (command[0] == "loadscene" && command.Length == 2)
        {
            LoadScene(command[1]);
            return;
        }

        //inventory commands
        if (command[0] == "allitems" && command.Length == 1)
        {
            AllItems();
            return;
        }

        //spellbook commands
        if (command[0] == "allspells" && command.Length == 1)
        {
            AllSpells();
            return;
        }

        Debug.Log("Wrong Command");
    }

    //player and movement commands ----------------------------------------------------------------------------------------- player and movement commands
    void GhostMode() //making player able to move through objects
    {
        ghostmode = !ghostmode;
        PlayerParams.Controllers.playerMovement.ghostmodeActive = ghostmode;
    }


    //parameters commands ----------------------------------------------------------------------------------------- parameters commands
    void Fps() //turning on/off fps display
    {
        fps = !fps;
        fpsCounter.SetActive(fps);
    }

    void Camera()
    {
        webcam = !webcam;
        webCamera.SetActive(webcam);
    }


    //game saves commands ----------------------------------------------------------------------------------------- game saves commands
    void SaveState()
    {
        ProgressSaving saveManager;

        try
        {
            saveManager = FindObjectOfType<ProgressSaving>();
        }
        catch
        {
            Debug.Log("Save manager not found");
            return;
        }

        Spellbook spellbook = PlayerParams.Controllers.spellbook;
        Inventory inventory = PlayerParams.Controllers.inventory;
        Journal journal = PlayerParams.Controllers.journal;

        //game state
        saveManager.SaveGameState(SceneManager.GetActiveScene().name,
            PlayerParams.Controllers.pointsManager.plotPoints,
            PlayerParams.Controllers.pointsManager.foundSecrets,
            PlayerParams.Controllers.pointsManager.currency,
            PlayerParams.Controllers.pointsManager.maxPlotPoints,
            PlayerParams.Controllers.pointsManager.minPlotPoints,
            PlayerParams.Controllers.pointsManager.maxFoundSecrets,
            PlayerParams.Controllers.pointsManager.maxCurrency);

        //spells
        List<string> spells = new List<string>();
        foreach (SpellScrollInfo scroll in spellbook.spells)
        {
            spells.Add(scroll.spellName);
        }
        saveManager.SaveSpells(spellbook.bookOwned, spells);

        //items
        saveManager.SaveItems(inventory.inventory);

        //journal
        saveManager.SaveJournal(journal.notesJournal, journal.dialoguesJournal);

        //everything to file
        saveManager.SaveProgressToFile();
    }

    void ResetState()
    {
        ProgressSaving saveManager;

        try
        {
            saveManager = FindObjectOfType<ProgressSaving>();
        }
        catch
        {
            Debug.Log("Save manager not found");
            return;
        }

        Spellbook spellbook = PlayerParams.Controllers.spellbook;
        Inventory inventory = PlayerParams.Controllers.inventory;
        Journal journal = PlayerParams.Controllers.journal;

        //game state
        saveManager.SaveGameState(SceneManager.GetActiveScene().name, 0, 0, 0, 0, 0, 0, 0);

        //spells
        saveManager.SaveSpells(false, new List<string>());

        //items
        saveManager.SaveItems(new List<GameObject>());

        //journal
        saveManager.SaveJournal(new Dictionary<string, string>(), new Dictionary<string, List<List<string>>>());

        //everything to file
        saveManager.SaveProgressToFile();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


    //inventory commands ----------------------------------------------------------------------------------------- inventory commands
    void AllItems() //adding all existings items to inventory
    {
        ItemHolder itemHolder = FindObjectOfType<ItemHolder>();

        if(itemHolder != null)
        {
            Inventory inventory = PlayerParams.Controllers.inventory;
            foreach(GameObject item in itemHolder.items)
            {
                inventory.AddItem(itemHolder.GiveItem(item.GetComponent<ItemBehavior>().itemName));
            }
        }
    }


    //spellbook commands ----------------------------------------------------------------------------------------- spellbook commands
    void AllSpells() //adding all existings spells to spellbook
    {
        SpellScrollsHolder spellScrollsHolder = FindObjectOfType<SpellScrollsHolder>();

        if (spellScrollsHolder != null)
        {
            Spellbook spellbook = PlayerParams.Controllers.spellbook;
            spellbook.bookOwned = true;

            foreach(SpellScrollInfo spellScroll in spellScrollsHolder.GiveAllScrolls())
            {
                spellbook.AddSpell(spellScroll);
            }
        }
    }
}
