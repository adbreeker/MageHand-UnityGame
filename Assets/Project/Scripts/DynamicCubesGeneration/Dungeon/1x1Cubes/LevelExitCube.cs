using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExitCube : MonoBehaviour
{
    [Header("Next scene:")]
    public string chapter;

    [Header("Change music?")]
    public bool changeMusic = false;

    public static event Action OnLevelChange;

    protected BoxCollider _exitBounds;

    protected bool _isAnimationGoing = false;
    //objects needed for saving progress:
    protected ProgressSaving saveManager;
    protected Spellbook spellbook;
    protected Inventory inventory;
    protected Journal journal;

    protected void Start() //finding all necessary objects
    {
        _exitBounds = GetComponent<BoxCollider>();

        saveManager = GameParams.Managers.saveManager;
        spellbook = PlayerParams.Controllers.spellbook;
        inventory = PlayerParams.Controllers.inventory;
        journal = PlayerParams.Controllers.journal;
    }

    protected void Update() //checking if player is inside cube
    {
        if (_exitBounds.bounds.Contains(PlayerParams.Objects.player.transform.position) && !_isAnimationGoing)
        {
            _isAnimationGoing = true;
            if (OnLevelChange != null) { OnLevelChange(); }
            ChangeLevel();
        }
    }

    protected virtual void ChangeLevel()
    {
        SaveProgress();

        GameParams.Managers.fadeInOutManager.ChangeScene(chapter, fadeOutAndChangeMusic: changeMusic);
    }

    protected void SaveProgress() //saving all progress
    {
        //game state
        saveManager.SaveGameState(chapter, 
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
}