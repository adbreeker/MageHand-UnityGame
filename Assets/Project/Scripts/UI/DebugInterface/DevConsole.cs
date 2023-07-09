using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        }
        else
        {
            PlayerParams.Variables.uiActive = true;
            PlayerParams.Controllers.spellbook.ableToInteract = false;
            PlayerParams.Controllers.inventory.ableToInteract = false;
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

        //game saves commands
        if (command[0] == "deletesave" && command.Length == 1)
        {
            DeleteSave();
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
        if(ghostmode)
        {
            ghostmode = false;
            PlayerParams.Controllers.playerMovement.ghostmodeActive = false;
            return;
        }
        else
        {
            ghostmode = true;
            PlayerParams.Controllers.playerMovement.ghostmodeActive = true;
            return;
        }
    }


    //parameters commands ----------------------------------------------------------------------------------------- parameters commands
    void Fps() //turning on/off fps display
    {
        if(fps)
        {
            fpsCounter.SetActive(false);
        }
        else
        {
            fpsCounter.SetActive(true);
        }
    }


    //game saves commands ----------------------------------------------------------------------------------------- game saves commands

    void DeleteSave()
    {

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
                inventory.AddItem(itemHolder.GiveItem(item.GetComponent<ItemParameters>().itemName));
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
