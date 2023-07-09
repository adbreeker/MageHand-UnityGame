using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkAndReturnSpellBehavior : MonoBehaviour
{
    public GameObject markedTeleportationEffectPrefab;

    public void TeleportationPerformed()
    {
        Instantiate(markedTeleportationEffectPrefab, PlayerParams.Objects.player.transform);
        Destroy(gameObject);
    }
}
