using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicTacToeTile : MagicImpactTarget
{
    public bool tileMarked = false;

    GameObject _mark;

    public void MarkTile(GameObject markPrefab)
    {
        tileMarked = true;
        _mark = Instantiate(markPrefab, 
            new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), 
            Quaternion.Euler(90f,0f,0f));
    }

    public override void ImpactInteraction(GameObject impactingSpell)
    {
        Debug.Log("trafiony");
        if(impactingSpell.GetComponent<MarkSpellBehavior>() != null)
        {
            if(impactingSpell.transform.position.y < transform.position.y + 0.5f)
            {
                tileMarked = true;
                PlayerParams.Controllers.spellCasting.magicMark = null;
                _mark = impactingSpell;
            }
        }
    }
}
