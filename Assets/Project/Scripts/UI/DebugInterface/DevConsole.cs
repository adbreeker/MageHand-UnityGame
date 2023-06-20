using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DevConsole : MonoBehaviour
{
    [Header("Objects of DebugCanvas")]
    public TMP_InputField console;
    public GameObject fps;
    public GameObject webCamera;

    [Header("Player and his stuff")]
    public GameObject player;

    private List<string> previousCommands = new List<string>();
    int currentCommandOnList = 0;


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
            player.GetComponent<PlayerMovement>().uiActive = false;
            player.GetComponent<Spellbook>().ableToInteract = true;
            player.GetComponent<Inventory>().ableToInteract = true;
        }
        else
        {
            player.GetComponent<PlayerMovement>().uiActive = true;
            player.GetComponent<Spellbook>().ableToInteract = false;
            player.GetComponent<Inventory>().ableToInteract = false;
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
        if(command[0] == "ghostmode" && command.Length == 2)
        {
            GhostMode(command[1]);
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
    void GhostMode(string param) //making player able to move through objects
    {
        if(param == "on" || param == "true")
        {
            player.GetComponent<PlayerMovement>().ghostmodeActive = true;
            return;
        }
        if (param == "off" || param == "false")
        {
            player.GetComponent<PlayerMovement>().ghostmodeActive = false;
            return;
        }

        Debug.Log("Wrong Command");
    }


    //parameters commands ----------------------------------------------------------------------------------------- parameters commands
    void Fps() //turning on/off fps display
    {
        if(fps.activeSelf)
        {
            fps.SetActive(false);
        }
        else
        {
            fps.SetActive(true);
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
            Inventory inventory = player.GetComponent<Inventory>();
            foreach(GameObject item in itemHolder.items)
            {
                inventory.AddItem(itemHolder.GiveItem(item.name));
            }
        }
    }


    //spellbook commands ----------------------------------------------------------------------------------------- spellbook commands
    void AllSpells() //adding all existings spells to spellbook
    {
        SpellScrollsHolder spellScrollsHolder = FindObjectOfType<SpellScrollsHolder>();

        if (spellScrollsHolder != null)
        {
            Spellbook spellbook = player.GetComponent<Spellbook>();
            spellbook.bookOwned = true;

            spellbook.AddSpellFromScroll(spellScrollsHolder.GiveScroll("Light"));
        }
    }
}
