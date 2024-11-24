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
    public GameObject fpsCounter;
    public GameObject webCamera;

    [Header("Hands for devhand:")]
    public GameObject handObject;
    public GameObject devHandObject;

    //commands history
    private List<string> previousCommands = new List<string>();
    int currentCommandOnList = 0;

    //currently active commands
    bool ghostmode = false;
    bool devhand = false;
    bool fps = false;
    bool webcam = false;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("ghostmode")) { PlayerPrefs.SetInt("ghostmode", 0); }
        if (!PlayerPrefs.HasKey("devhand")) { PlayerPrefs.SetInt("devhand", 0); }
        if (!PlayerPrefs.HasKey("fps")) { PlayerPrefs.SetInt("fps", 0); }
        if (!PlayerPrefs.HasKey("camera")) { PlayerPrefs.SetInt("camera", 0); }

        if (PlayerPrefs.GetInt("ghostmode") == 1) { GhostMode(); }
        if (PlayerPrefs.GetInt("devhand") == 1) { DevHand(); }
        if (PlayerPrefs.GetInt("fps") == 1) { Fps(); }
        if (PlayerPrefs.GetInt("camera") == 1) { Camera(); }
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
        if(command[0] == "ghostmode" && command.Length == 1)
        {
            GhostMode();
            return;
        }
        if (command[0] == "devhand" && command.Length == 1)
        {
            DevHand();
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

        if (command[0] == "collectallitems" && command.Length == 1)
        {
            CollectAllItems();
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
        PlayerPrefs.SetInt("ghostmode", ghostmode ? 1 : 0);
        PlayerParams.Controllers.playerMovement.ghostmodeActive = ghostmode;
    }

    void DevHand()
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


    //parameters commands ----------------------------------------------------------------------------------------- parameters commands
    void Fps() //turning on/off fps display
    {
        fps = !fps;
        PlayerPrefs.SetInt("fps", fps ? 1 : 0);
        fpsCounter.SetActive(fps);
    }

    void Camera()
    {
        webcam = !webcam;
        PlayerPrefs.SetInt("camera", webcam ? 1 : 0);
        webCamera.SetActive(webcam);
    }


    //game saves commands ----------------------------------------------------------------------------------------- game saves commands
    void SaveState()
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

    void ResetState()
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

    void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


    //inventory commands ----------------------------------------------------------------------------------------- inventory commands
    void AllItems() //adding all existings items to inventory
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

    void CollectAllItems() //adding all existings items to inventory
    {
        Inventory inventory = PlayerParams.Controllers.inventory;

        foreach (ItemBehavior item in FindObjectsOfType<ItemBehavior>())
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
    void AllSpells() //adding all existings spells to spellbook
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
