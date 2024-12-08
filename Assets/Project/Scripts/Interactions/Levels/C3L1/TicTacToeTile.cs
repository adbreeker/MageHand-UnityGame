using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicTacToeTile : MagicImpactTarget
{
    public bool tileMarked = false;
    public GameObject mark;

    public event Action<TicTacToeTile> OnPlayerMarkedTile;

    public void MarkTile(GameObject markPrefab)
    {
        tileMarked = true;
        mark = Instantiate(markPrefab, 
            new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), 
            Quaternion.Euler(90f,0f,0f));
    }

    public override void ImpactInteraction(GameObject impactingSpell)
    {
        if(impactingSpell.GetComponent<MarkSpellBehavior>() != null)
        {
            if(impactingSpell.transform.position.y < transform.position.y + 0.5f)
            {
                if (!tileMarked)
                {
                    tileMarked = true;
                    PlayerParams.Controllers.spellCasting.magicMark = null;
                    mark = impactingSpell;
                    OnPlayerMarkedTile?.Invoke(this);
                }
                else
                {
                    Destroy(impactingSpell);
                }
            }
        }
    }
}
