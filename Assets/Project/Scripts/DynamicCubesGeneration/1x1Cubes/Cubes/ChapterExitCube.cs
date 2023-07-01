using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChapterExitCube : MonoBehaviour
{
    public LayerMask playerMask;
    public string chapter;

    BoxCollider box;

    //objects needed for saving progress:
    private ProgressSaving saveManager;
    private Spellbook spellbook;
    private Inventory inventory;

    private void Start()
    {
        box = GetComponent<BoxCollider>();

        saveManager = FindObjectOfType<ProgressSaving>();
        spellbook = FindObjectOfType<Spellbook>();
        inventory = FindObjectOfType<Inventory>();
    }

    private void Update()
    {
        Collider[] colliders;
        colliders = Physics.OverlapBox(box.bounds.center, box.bounds.extents, Quaternion.identity, playerMask);
        if (colliders.Length > 0)
        {
            SaveProgress();
            SceneManager.LoadScene(chapter);
        }
    }

    private void SaveProgress()
    {
        saveManager.SaveGameState(chapter);

        List<string> spells = new List<string>();
        foreach(SpellScrollInfo scroll in spellbook.spells)
        {
            spells.Add(scroll.spellName);
        }
        saveManager.SaveSpells(spellbook.bookOwned, spells);

        saveManager.SaveItems(inventory.inventory);

        saveManager.SaveProgressToFile();
    }
}
