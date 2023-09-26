using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject menuPrefab;


    [Header("Settings")]
    public bool ableToInteract = true;
    public bool menuOpened = false;

    private GameObject instantiatedMenu;
    private GameObject pointer;
    private TextMeshProUGUI pointedOption;
    private TextMeshProUGUI pointedOptionMenu;

    private bool atMenu = false;
    private  List<TextMeshProUGUI> menuOptions;


    void Update()
    {
        if (ableToInteract)
        {
            KeysListener();
        }
    }

    void KeysListener()
    {
        //Open or close spellbook
        if (Input.GetKeyDown(KeyCode.Escape) && !PlayerParams.Variables.uiActive)
        {
            OpenMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && menuOpened && atMenu)
        {
            CloseMenu();
        }

        if (Input.GetKeyDown(KeyCode.S) && menuOpened && atMenu)
        {

        }

    }

    public void OpenMenu()
    {
        instantiatedMenu = Instantiate(menuPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);

        PlayerParams.Controllers.inventory.ableToInteract = false;
        PlayerParams.Controllers.spellbook.ableToInteract = false;
        PlayerParams.Variables.uiActive = true;
        PlayerParams.Objects.hand.SetActive(false);
        menuOpened = true;
        atMenu = true;

        pointer = instantiatedMenu.transform.Find("Pointer").gameObject;

        for (int i = 1; i < 7; i++)
        {
            string text = i.ToString();
            menuOptions.Add(instantiatedMenu.transform.Find("Menu").transform.Find("Options").transform.Find(text).GetComponent<TextMeshProUGUI>());
        }
        PointOption(menuOptions[0], menuOptions);
    }

    public void CloseMenu()
    {
        Destroy(instantiatedMenu);

        //Enable other controls
        PlayerParams.Controllers.inventory.ableToInteract = true;
        PlayerParams.Controllers.spellbook.ableToInteract = true;
        PlayerParams.Variables.uiActive = false;
        PlayerParams.Objects.hand.SetActive(true);
        menuOpened = false;
        atMenu = false;

        menuOptions.Clear();
    }

    void PointOption(TextMeshProUGUI option, List<TextMeshProUGUI> allOptions)
    {
        for (int i = 0; i < allOptions.Count; i++)
        {
            allOptions[i].color = new Color(0.2666f, 0.2666f, 0.2666f);
        }

        option.color = new Color(1f, 1f, 1f);

        pointer.transform.parent = option.transform;
        pointer.transform.localPosition = new Vector3(0, 0, 0);

        if (atMenu) pointedOptionMenu = option;
        else pointedOption = option;
    }
}
