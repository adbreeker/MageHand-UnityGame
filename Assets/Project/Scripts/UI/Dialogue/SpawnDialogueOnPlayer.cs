using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDialogueOnPlayer : MonoBehaviour
{
    public static GameObject SpawnDialogue(GameObject dialoguePrefab)
    {
        return Instantiate(dialoguePrefab, PlayerParams.Objects.player.transform.position, Quaternion.identity);
    }
}
