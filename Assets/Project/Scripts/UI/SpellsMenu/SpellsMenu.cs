using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellsMenu : MonoBehaviour
{
    [Header("Game objects")]
    public GameObject spellsMenuPrefab;

    [Header("Settings")]
    public bool ableToInteract = true;
    public bool menuOpened = false;

    private GameObject instantiatedMenu;

    private List<GameObject> spellIcons;

    private AudioSource openSound;
    private AudioSource closeSound;
    private AudioSource spellCastingSound;

    void Update()
    {
        if (menuOpened && Input.GetKeyDown(KeyCode.Escape))
        {
            closeSound.Play();
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


        openSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.UI_Open);
        openSound.Play();
        Destroy(openSound.gameObject, openSound.clip.length);
        spellCastingSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_CastingSpell);
        spellCastingSound.loop = true;
        spellCastingSound.Play();

        closeSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.UI_Close);
        spellIcons = new List<GameObject>();

        GameObject iconsParent = instantiatedMenu.transform.Find("Spell " + (PlayerParams.Controllers.spellbook.spells.Count).ToString()).gameObject;
        iconsParent.SetActive(true);
        for (int i = 1; i < PlayerParams.Controllers.spellbook.spells.Count + 1; i++)
        {
            string text = i.ToString();
            spellIcons.Add(iconsParent.transform.Find(text).gameObject);
        }

        //activate propper spells (as much as there are in spellbook)
        for (int i = 0; i < spellIcons.Count; i++)
        {
            spellIcons[i].SetActive(true);
            spellIcons[i].GetComponent<TextMeshProUGUI>().text = PlayerParams.Controllers.spellbook.spells[i].spellName;
        }

        menuOpened = true;
    }

    public void CloseMenu()
    {
        Destroy(instantiatedMenu);

        if (menuOpened)
        {
            Destroy(closeSound.gameObject, closeSound.clip.length);

            Destroy(spellCastingSound.gameObject);
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
        for (int i = 0; i < spellIcons.Count; i++)
        {
            if (spellIcons[i].activeSelf)
            {
                if (spellIcons[i].GetComponent<EnlightObject>() != null)
                {
                    spellIcons[i].GetComponent<SpellIcon>().background.GetComponent<Image>().color = new Color(1f, 0.482f, 0f, 1f);
                    spellIcons[i].GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);
                }
                else if (spellIcons[i].GetComponent<SpellIcon>().background.GetComponent<Image>().color == new Color(1f, 0.482f, 0f, 1f))
                {
                    spellIcons[i].GetComponent<SpellIcon>().background.GetComponent<Image>().color = new Color(1f, 0.482f, 0f, 0f);
                    spellIcons[i].GetComponent<TextMeshProUGUI>().color = new Color(0.2666f, 0.2666f, 0.2666f);
                }
            }
        }
    }
}
