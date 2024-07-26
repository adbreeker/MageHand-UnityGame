using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassageDialogueChoice : MonoBehaviour
{
    [SerializeField]
    private Dialogue dialogueWithChoice;
    [SerializeField]
    private GameObject key;
    [SerializeField]
    private GameObject dialogueForgot;
    [SerializeField]
    private GameObject dialogueLie;

    private Inventory inventory;
    private string keyItemName;
    private bool check = true;

    private void Start()
    {
        PlayerParams.Controllers.pointsManager.maxPlotPoints += +1;
        PlayerParams.Controllers.pointsManager.minPlotPoints += -2;
        inventory = PlayerParams.Controllers.inventory;
        keyItemName = key.GetComponent<ItemBehavior>().itemName;
    }

    private void Update()
    {
        if (check)
        {
            if (inventory.HasItem(keyItemName) && dialogueWithChoice.playerChoice == dialogueWithChoice.option2Text)
            {
                //Player has the key and chose that they has it
                //They found it and doesn't lie
                PlayerParams.Controllers.pointsManager.plotPoints += 1;
                check = false;
                Destroy(this);
            }
            else if (!inventory.HasItem(keyItemName) && dialogueWithChoice.playerChoice == dialogueWithChoice.option2Text)
            {
                //Player doesn't have the key, but chose that they has it
                //They are lying
                PlayerParams.Controllers.pointsManager.plotPoints += -2;
                StartCoroutine(SpawnDialogue(dialogueLie));
            }
            else if (inventory.HasItem(keyItemName) && dialogueWithChoice.playerChoice == dialogueWithChoice.option1Text)
            {
                //Player has the key, but chose that they doesn't have it
                //They forgot about it?
                PlayerParams.Controllers.pointsManager.plotPoints += -1;
                StartCoroutine(SpawnDialogue(dialogueForgot));
            }
        }    
    }

    IEnumerator SpawnDialogue(GameObject dialogue)
    {
        check = false;
        yield return new WaitForSeconds(2f);
        SpawnDialogueOnPlayer.SpawnDialogue(dialogue);
        Destroy(this);
    }
}
