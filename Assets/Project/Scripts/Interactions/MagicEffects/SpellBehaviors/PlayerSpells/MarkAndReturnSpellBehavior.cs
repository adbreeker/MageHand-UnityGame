using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkAndReturnSpellBehavior : SpellBehavior
{
    public void TeleportationPerformed()
    {
        Instantiate(specialEffectPrefab, PlayerParams.Objects.player.transform);
        Destroy(gameObject);
    }
}
