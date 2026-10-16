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
    [Header("Objects of DebugCanvas:")]
    public TMP_InputField console;
    public GameObject perfoOverlay;
    public GameObject webCamera;
    public GameObject narrativeOverlay;

    [Header("Hands for devhand:")]
    public GameObject handObject;
    public GameObject devHandObject;

    //commands history
    private List<string> previousCommands = new List<string>();
    int currentCommandOnList = 0;

    //currently active commands
    bool ghostmode = false;
    bool devhand = false;
    bool perfo = false;
    bool webcam = false;
    bool narrative = false;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("ghostmode")) { PlayerPrefs.SetInt("ghostmode", 0); }
        if (!PlayerPrefs.HasKey("devhand")) { PlayerPrefs.SetInt("devhand", 0); }
        if (!PlayerPrefs.HasKey("perfo")) { PlayerPrefs.SetInt("perfo", 0); }
        if (!PlayerPrefs.HasKey("camera")) { PlayerPrefs.SetInt("camera", 0); }
        if (!PlayerPrefs.HasKey("narrative")) { PlayerPrefs.SetInt("narrative", 0); }

        if (PlayerPrefs.GetInt("ghostmode") == 1) { Command_GhostMode(); }
        if (PlayerPrefs.GetInt("devhand") == 1) { Command_DevHand(); }
        if (PlayerPrefs.GetInt("perfo") == 1) { Command_Perfo(); }
        if (PlayerPrefs.GetInt("camera") == 1) { Command_Camera(); }
        if (PlayerPrefs.GetInt("narrative") == 1) { Command_Narrative(); }
    }

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
        if((command[0] == "ghostmode" || command[0] == "ghost") && command.Length == 1)
        {
            Command_GhostMode();
            return;
        }
        if (command[0] == "devhand" && command.Length == 1)
        {
            Command_DevHand();
            return;
        }

        //debug display
        if(command[0] == "perfo" && command.Length == 1)
        {
            Command_Perfo();
            return;
        }
        if (command[0] == "camera" && command.Length == 1)
        {
            Command_Camera();
            return;
        }
        if (command[0] == "narrative" && command.Length == 1)
        {
            Command_Narrative();
            return;
        }

        //game saves commands
        if (command[0] == "savestate" && command.Length == 1)
        {
            Command_SaveState();
            return;
        }
        if (command[0] == "resetstate" && command.Length == 1)
        {
            Command_ResetState();
            return;
        }
        if (command[0] == "loadscene" && command.Length == 2)
        {
            Command_LoadScene(command[1]);
            return;
        }

        //inventory commands
        if (command[0] == "allitems" && command.Length == 1)
        {
            Command_AllItems();
            return;
        }
        if (command[0] == "collectallitems" && command.Length == 1)
        {
            Command_CollectAllItems();
            return;
        }

        //spellbook commands
        if (command[0] == "allspells" && command.Length == 1)
        {
            Command_AllSpells();
            return;
        }

        Debug.Log("Wrong Command");
    }

    //player and movement commands ----------------------------------------------------------------------------------------- player and movement commands
    void Command_GhostMode() //making player able to move through objects
    {
        ghostmode = !ghostmode;
        PlayerPrefs.SetInt("ghostmode", ghostmode ? 1 : 0);
        PlayerParams.Controllers.playerMovement.ghostmodeActive = ghostmode;
    }

    void Command_DevHand()
    {
        devhand = !devhand;
        PlayerPrefs.SetInt("devhand", devhand ? 1 : 0);

        if(devhand && devHandObject != null) 
        {
            devHandObject.SetActive(true);
            handObject.SetActive(false);

            PlayerParams.Controllers.handInteractions = devHandObject.GetComponent<HandInteractions>();
            PlayerParams.Objects.hand = devHandObject;
        }
        if(!devhand && handObject != null)
        {
            devHandObject.SetActive(false);
            handObject.SetActive(true);

            PlayerParams.Controllers.handInteractions = handObject.GetComponent<HandInteractions>();
            PlayerParams.Objects.hand = handObject;
        }
    }



    //debug display commands ----------------------------------------------------------------------------------------- debug display commands
    void Command_Perfo() //turning on/off perfo display
    {
        perfo = !perfo;
        PlayerPrefs.SetInt("perfo", perfo ? 1 : 0);
        perfoOverlay.SetActive(perfo);
    }

    void Command_Camera()
    {
        webcam = !webcam;
        PlayerPrefs.SetInt("camera", webcam ? 1 : 0);
        webCamera.SetActive(webcam);
    }

    void Command_Narrative()
    {
        narrative = !narrative;
        PlayerPrefs.SetInt("narrative", narrative ? 1 : 0);
        narrativeOverlay.SetActive(narrative);
    }


    //game saves commands ----------------------------------------------------------------------------------------- game saves commands
    void Command_SaveState()
    {
        ProgressSaving saveManager;

        try
        {
            saveManager = GameParams.Managers.saveManager;
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

    void Command_ResetState()
    {
        ProgressSaving saveManager;

        try
        {
            saveManager = GameParams.Managers.saveManager;
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

    void Command_LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


    //inventory commands ----------------------------------------------------------------------------------------- inventory commands
    void Command_AllItems() //adding all existings items to inventory
    {
        ItemHolder itemHolder = GameParams.Holders.itemHolder;

        if(itemHolder != null)
        {
            Inventory inventory = PlayerParams.Controllers.inventory;
            foreach(GameObject item in itemHolder.items)
            {
                inventory.AddItem(itemHolder.GiveItem(item.GetComponent<ItemBehavior>().itemName));
            }
        }
    }

    void Command_CollectAllItems() //adding all existings items to inventory
    {
        Inventory inventory = PlayerParams.Controllers.inventory;

        foreach (ItemBehavior item in FindObjectsByType<ItemBehavior>(FindObjectsSortMode.None))
        {
            item.isInteractable = true;
            if (item.GetComponent<ReadableBehavior>() is ReadableBehavior readableBehavior && !readableBehavior.destroy
                    || item.GetComponent<ReadableBehavior>() == null)
            {
                // Add item when ReadableBehavior does not exist or destroy is false
                inventory.AddItem(item.gameObject);
            }
        }
    }


    //spellbook commands ----------------------------------------------------------------------------------------- spellbook commands
    void Command_AllSpells() //adding all existings spells to spellbook
    {
        SpellScrollsHolder spellScrollsHolder = GameParams.Holders.spellScrollsHolder;

        if (spellScrollsHolder != null)
        {
            Spellbook spellbook = PlayerParams.Controllers.spellbook;
            spellbook.bookOwned = true;
            foreach (GameObject spellScroll in GameObject.FindGameObjectsWithTag("SpellScroll"))
            {
                spellScroll.layer = LayerMask.NameToLayer("Item");
            }

            foreach (SpellScrollInfo spellScroll in spellScrollsHolder.GiveAllScrolls())
            {
                spellbook.AddSpell(spellScroll);
            }
        }
    }
}
