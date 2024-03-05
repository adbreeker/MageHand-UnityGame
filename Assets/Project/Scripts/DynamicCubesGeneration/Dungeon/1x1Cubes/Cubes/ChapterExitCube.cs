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

    public bool changeMusic = false;

    BoxCollider box;

    private bool _isAnimationGoing = false;
    //objects needed for saving progress:
    private ProgressSaving saveManager;
    private Spellbook spellbook;
    private Inventory inventory;
    private Journal journal;

    private void Start() //finding all necessary objects
    {
        box = GetComponent<BoxCollider>();

        saveManager = FindObjectOfType<ProgressSaving>();
        spellbook = PlayerParams.Controllers.spellbook;
        inventory = PlayerParams.Controllers.inventory;
        journal = PlayerParams.Controllers.journal;
    }

    private void Update() //checking if player is inside cube
    {
        Collider[] colliders;
        colliders = Physics.OverlapBox(box.bounds.center, box.bounds.extents, Quaternion.identity, playerMask);
        if (colliders.Length > 0 && !_isAnimationGoing)
        {
            SaveProgress();
            
            FindObjectOfType<FadeInFadeOut>().ChangeScene(chapter, fadeOutAndChangeMusic: changeMusic);
            _isAnimationGoing = true;
        }
    }

    private void SaveProgress() //saving all progress
    {
        //game state
        saveManager.SaveGameState(chapter, PlayerParams.Controllers.pointsManager.plotPoints);

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
}