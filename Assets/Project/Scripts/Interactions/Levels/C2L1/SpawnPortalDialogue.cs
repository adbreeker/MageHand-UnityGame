using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPortalDialogue : MonoBehaviour
{
    [SerializeField] OpenLockedDoorsPassage doors;
    [SerializeField] GameObject dialogue;
    void Update()
    {
        if (doors.doorsOpen)
        {
            dialogue.SetActive(true);
            Destroy(this);
        }
    }
}
