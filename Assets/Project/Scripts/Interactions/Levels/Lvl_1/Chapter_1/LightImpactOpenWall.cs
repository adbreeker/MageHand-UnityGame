using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightImpactOpenWall : SpellImpactInteraction
{
    [SerializeField] OpenWallPassage _openWallPassage;
    public override void OnSpellImpact(GameObject spell)
    {
        if(spell.GetComponent<LightSpellBehavior>() != null) 
        {
            _openWallPassage.Interaction();
            Destroy(this);
        }
    }
}
