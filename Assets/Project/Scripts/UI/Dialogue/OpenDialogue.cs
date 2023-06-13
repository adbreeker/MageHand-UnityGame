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

    private bool activateDialogue = true;
    private GameObject dialogueEntry;
    private void Start()
    {
        dialogueEntry = gameObject;
    }
    private void Update()
    {
        //Activates canvas (dialogue) while player enters bounds of object that this script is connected to
        Bounds cubeBounds = dialogueEntry.GetComponent<Renderer>().bounds;
        if (cubeBounds.Contains(player.transform.position) && activateDialogue)
        {
            dialogueCanvas.gameObject.SetActive(true);
            activateDialogue = false;
        }
    }
}