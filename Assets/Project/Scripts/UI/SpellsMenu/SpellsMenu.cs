using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMODUnity;
using FMOD.Studio;

public class SpellsMenu : MonoBehaviour
{
    [Header("Game objects")]
    [SerializeField] private GameObject spellsMenuPrefab;
    [SerializeField] private GameObject spellCellPrefab;

    [Header("Settings")]
    public bool ableToInteract = true;
    public bool menuOpened = false;

    private GameObject instantiatedMenu;
    private List<SpellCell> spellCells;
    EventInstance castingSound;

    FmodEvents FmodEvents => GameParams.Managers.fmodEvents;

    void Update()
    {
        if (menuOpened && Input.GetKeyDown(KeyCode.Escape))
        {
            RuntimeManager.PlayOneShot(FmodEvents.NP_UiClose);
            CloseMenu();
        }

        if (menuOpened) PointIcon();
    }

    public void OpenMenu()
    {
        //Instatiate inventory and assign it to UiCamera
        instantiatedMenu = Instantiate(spellsMenuPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        instantiatedMenu.GetComponent<Canvas>().worldCamera = PlayerParams.Objects.uiCamera;
        instantiatedMenu.GetComponent<Canvas>().planeDistance = 1.05f;

        //Disable other controls (close first, because it activates movement and enable other ui)
        PlayerParams.Controllers.inventory.CloseInventory();
        PlayerParams.Controllers.spellbook.CloseSpellbook();
        PlayerParams.Controllers.pauseMenu.CloseMenu();
        PlayerParams.Controllers.journal.CloseJournal();

        PlayerParams.Controllers.inventory.ableToInteract = false;
        PlayerParams.Controllers.spellbook.ableToInteract = false;
        PlayerParams.Controllers.pauseMenu.ableToInteract = false;
        PlayerParams.Controllers.journal.ableToInteract = false;
        PlayerParams.Variables.uiActive = true;


        RuntimeManager.PlayOneShot(FmodEvents.NP_UiOpen);
        castingSound = GameParams.Managers.audioManager.PlayOneShotReturnInstance(FmodEvents.SFX_CastingSpell);

        spellCells = new List<SpellCell>();

        GameObject layoutObject = instantiatedMenu.transform.Find("Layout").gameObject;
        GridLayoutGroup layoutGroup = layoutObject.GetComponent<GridLayoutGroup>();

        List<SpellScrollInfo> playersSpells = PlayerParams.Controllers.spellbook.spells;
        int fontSize = 0;
        int hitboxSize = 0;

        if (playersSpells.Count < 3)
        {
            layoutGroup.cellSize = new Vector2(400, 400);
            hitboxSize = 400;
            layoutGroup.constraintCount = 2;
            fontSize = 64;
        }
        else if(playersSpells.Count < 5)
        {
            layoutGroup.cellSize = new Vector2(300, 300);
            hitboxSize = 300;
            layoutGroup.constraintCount = 2;
            fontSize = 50;
        }
        else if (playersSpells.Count < 7)
        {
            layoutGroup.cellSize = new Vector2(300, 300);
            hitboxSize = 300;
            layoutGroup.constraintCount = 3;
            fontSize = 50;
        }
        else if (playersSpells.Count < 9)
        {
            layoutGroup.cellSize = new Vector2(300, 300);
            hitboxSize = 300;
            layoutGroup.constraintCount = 4;
            fontSize = 50;
        }
        else if (playersSpells.Count < 13)
        {
            layoutGroup.cellSize = new Vector2(250, 250);
            hitboxSize = 250;
            layoutGroup.constraintCount = 4;
            fontSize = 40;
        }
        else
        {
            Debug.LogError("Too much spells! The SpellsMenu is prepered to handle numbers of spells under 13.");
        }

        foreach (SpellScrollInfo spell in playersSpells)
        {
            string spellName = spell.spellName;
            GameObject instantiatedSpellCell = Instantiate(spellCellPrefab, layoutObject.transform);
            SpellCell spellCell = instantiatedSpellCell.transform.Find("CellHitbox").GetComponent<SpellCell>();
            spellCell.spellNameTMP.text = spellName;
            spellCell.hitbox.size = new Vector3(hitboxSize, hitboxSize, 50);
            spellCell.spellNameTMP.fontSize = fontSize;
            spellCells.Add(spellCell);
        }

        menuOpened = true;
    }

    public void CloseMenu()
    {
        Destroy(instantiatedMenu);

        if (menuOpened)
        {
            castingSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        menuOpened = false;

        //Enable other controls
        PlayerParams.Variables.uiActive = false;
        PlayerParams.Controllers.inventory.ableToInteract = true;
        PlayerParams.Controllers.spellbook.ableToInteract = true;
        PlayerParams.Controllers.pauseMenu.ableToInteract = true;
        PlayerParams.Controllers.journal.ableToInteract = true;
    }

    void PointIcon()
    {
        foreach(SpellCell spellCell in spellCells)
        {
            if (spellCell.gameObject.GetComponent<EnlightObject>() != null)
            {
                spellCell.highlight.SetActive(true);
                spellCell.spellNameTMP.color = new Color(1f, 1f, 1f);
            }
            else if (spellCell.highlight.activeSelf)
            {
                spellCell.highlight.SetActive(false);
                spellCell.spellNameTMP.color = new Color(0.2666f, 0.2666f, 0.2666f);
            }
        }
    }
}
