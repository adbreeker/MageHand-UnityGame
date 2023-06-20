using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChapterExitCube : MonoBehaviour
{
    public LayerMask playerMask;
    public string chapter;

    //objects needed for saving progress:
    private ProgressSaving saveManager;
    private Spellbook spellbook;
    private Inventory inventory;

    private void Start()
    {
        saveManager = FindObjectOfType<ProgressSaving>();
        spellbook = FindObjectOfType<Spellbook>();
        inventory = FindObjectOfType<Inventory>();
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(new Vector3(transform.position.x, 2f, transform.position.z), 0.5f, playerMask);
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
        foreach(GameObject scroll in spellbook.spells)
        {
            spells.Add(scroll.GetComponent<SpellScrollBehavior>().spellName);
        }
        saveManager.SaveSpells(spellbook.bookOwned, spells);

        saveManager.SaveItems(inventory.inventory);

        saveManager.SaveProgressToFile();
    }
}
