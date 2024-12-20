using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LightImpactOpenWall : MagicImpactTarget
{
    [SerializeField] OpenWallPassage _openWallPassage;
    [SerializeField] GameObject _particles;
    [SerializeField] LayerMask _lightSpellMask;

    BoxCollider _lightBounds;

    private void Start()
    {
        _lightBounds = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        bool isLightInRange = false;
        Collider[] colliders = Physics.OverlapBox(_lightBounds.bounds.center, _lightBounds.bounds.extents, Quaternion.identity, _lightSpellMask);
        foreach (Collider collider in colliders) 
        {
            if (collider.GetComponent<LightSpellBehavior>() != null)
            {
                isLightInRange = true;
                break;
            }
        }

        if (isLightInRange)
        {
            _particles.SetActive(true);
        }
        else
        {
            _particles.SetActive(false);
        }
    }

    public override void ImpactInteraction(GameObject impactingSpell)
    {
        if (impactingSpell.GetComponent<LightSpellBehavior>() != null)
        {
            _openWallPassage.Interaction();
            _particles.SetActive(false);
            Destroy(this);
        }
    }
}
