using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenDialogue : MonoBehaviour
{
    [Header("Game objects")]
    public Canvas dialogueCanvas;
    public GameObject player;
    [Header("Parameters")]
    public float textSpeed = 0.02f;
    //Should not be long
    [Tooltip("Shouldn't be long")]
    public string dialogueName;

    private bool activateDialogue = true;

    private void Update()
    {
        //Activates canvas (dialogue) while player enters bounds of object that this script is connected to
        Bounds cubeBounds = GetComponent<Renderer>().bounds;
        if (cubeBounds.Contains(player.transform.position) && activateDialogue)
        {
            PlayerParams.Controllers.dialogueDiary.dialogueDiary.Add(dialogueName, new List<List<string>>());
            dialogueCanvas.gameObject.SetActive(true);
            activateDialogue = false;
        }
    }
}