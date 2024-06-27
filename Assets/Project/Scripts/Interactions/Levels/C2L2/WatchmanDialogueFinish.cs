using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchmanDialogueFinish : MonoBehaviour
{
    [SerializeField] OpenBarsPassage passage;
    [SerializeField] GameObject dialogue;

    bool started = false;

    void Update()
    {
        if(!started)
        {
            if(dialogue.activeSelf)
            {
                started = true;
                StartCoroutine(OpenPassageAfterDialogue());
            }
        }
    }

    IEnumerator OpenPassageAfterDialogue()
    {
        while(dialogue.activeSelf)
        {
            yield return null;
        }

        passage.Interaction();
    }
}
