using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenDialogue : MonoBehaviour
{
    public Canvas dialogueCanvas;
    public GameObject player;
    public GameObject dialogueEntry;
    bool activateDialogue = true;
    private void Update()
    {
        Bounds cubeBounds = dialogueEntry.GetComponent<Renderer>().bounds;
        if (cubeBounds.Contains(player.transform.position) && activateDialogue)
        {
            dialogueCanvas.gameObject.SetActive(true);
            activateDialogue = false;
        }
    }
}