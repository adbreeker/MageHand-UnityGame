using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChapterExitCube : MonoBehaviour
{
    [Header("Next scene:")]
    public string chapter;
    [Header("Searching only on player layer")]
    public LayerMask playerMask;

    BoxCollider box;

    //objects needed for saving progress:
    private ProgressSaving saveManager;
    private Spellbook spellbook;
    private Inventory inventory;

    private void Start() //finding all necessary objects
    {
        box = GetComponent<BoxCollider>();

        saveManager = FindObjectOfType<ProgressSaving>();
        spellbook = FindObjectOfType<Spellbook>();
        inventory = FindObjectOfType<Inventory>();
    }

    private void Update() //checking if player is inside cube
    {
        Collider[] colliders;
        colliders = Physics.OverlapBox(box.bounds.center, box.bounds.extents, Quaternion.identity, playerMask);
        if (colliders.Length > 0)
        {
            SaveProgress();
            SceneManager.LoadScene(chapter);
        }
    }

    private void SaveProgress() //saving all progress
    {
        //game state
        saveManager.SaveGameState(chapter);

        //spells
        List<string> spells = new List<string>();
        foreach(SpellScrollInfo scroll in spellbook.spells)
        {
            spells.Add(scroll.spellName);
        }
        saveManager.SaveSpells(spellbook.bookOwned, spells);

        //items
        saveManager.SaveItems(inventory.inventory);

        //everything to file
        saveManager.SaveProgressToFile();
    }
}
