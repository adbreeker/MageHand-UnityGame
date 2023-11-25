using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExitCube : MonoBehaviour
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
    private DialogueDiary dialogueDiary;

    bool _isAnimationGoing = false;

    private void Start() //finding all necessary objects
    {
        box = GetComponent<BoxCollider>();

        saveManager = FindObjectOfType<ProgressSaving>();
        spellbook = PlayerParams.Controllers.spellbook;
        inventory = PlayerParams.Controllers.inventory;
        dialogueDiary = PlayerParams.Controllers.dialogueDiary;
    }

    private void Update() //checking if player is inside cube
    {
        Collider[] colliders;
        colliders = Physics.OverlapBox(box.bounds.center, box.bounds.extents, Quaternion.identity, playerMask);
        if (colliders.Length > 0 && !_isAnimationGoing)
        {
            _isAnimationGoing=true;
            StartCoroutine(ChangeLevel());
        }
    }

    private void SaveProgress() //saving all progress
    {
        //game state
        saveManager.SaveGameState(chapter);

        //spells
        List<string> spells = new List<string>();
        foreach (SpellScrollInfo scroll in spellbook.spells)
        {
            spells.Add(scroll.spellName);
        }
        saveManager.SaveSpells(spellbook.bookOwned, spells);

        //items
        saveManager.SaveItems(inventory.inventory);

        //dialogue diary
        saveManager.SaveDialogueDiary(dialogueDiary.dialogueDiary);

        //everything to file
        saveManager.SaveProgressToFile();
    }

    IEnumerator ChangeLevel()
    {
        PlayerParams.Controllers.playerMovement.movementSpeed = 0;
        float stairsHigh = PlayerParams.Objects.player.transform.position.y + 2.0f;
        int stairCounter = 0;
        while (true)
        {
            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForFixedUpdate();
                Vector3 destination = transform.position;
                destination.y = PlayerParams.Objects.player.transform.position.y;
                PlayerParams.Objects.player.transform.position = Vector3.MoveTowards(PlayerParams.Objects.player.transform.position, destination, 0.05f);
            }

            for (int i=0; i < 10; i++)
            {
                yield return new WaitForFixedUpdate();
                Vector3 destination = PlayerParams.Objects.player.transform.position;
                destination.y = stairsHigh;
                PlayerParams.Objects.player.transform.position = Vector3.MoveTowards(PlayerParams.Objects.player.transform.position, destination, 0.05f);
            }

            stairCounter++;

            if (stairCounter >= 5)
            {
                break;
            }
        }
        SaveProgress();
        SceneManager.LoadScene(chapter);
    }
}
