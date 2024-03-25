using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LightImpactOpenWall : SpellImpactInteraction
{
    [SerializeField] OpenWallPassage _openWallPassage;

    [SerializeField] GameObject _particlesPrefab;
    [SerializeField] GameObject _particles = null;

    Vector3 overlapPos;
    Vector3 overlapSize = new Vector3(2,2,1);

    private void Awake()
    {
        overlapPos = transform.position;
        overlapPos.y = 2;
        overlapPos -= 2 * transform.forward * transform.localScale.z;
        Debug.Log(overlapPos);
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 overlapGizmoPos = transform.position;
        overlapGizmoPos.y = 2;
        overlapGizmoPos -= 1 * transform.forward * transform.localScale.z;

        Vector3 overlapGizmoSize = new Vector3(4, 4, 2);
        overlapGizmoSize = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * overlapGizmoSize;

        Gizmos.DrawCube(overlapGizmoPos, overlapGizmoSize);
    }*/

    private void Update()
    {
        Collider[] colliders = Physics.OverlapBox(overlapPos, overlapSize, Quaternion.identity, LayerMask.GetMask("TransparentFX", "UI"));

        bool lightInRange = false;
        foreach(Collider potentialLight in colliders)
        {
            if(potentialLight.GetComponent<LightSpellBehavior>() != null) 
            {
                lightInRange = true;
            }
        }

        if(lightInRange)
        {
            _particles.SetActive(true);
        }
        else
        {
            _particles.SetActive(false);
        }
    }

    public override void OnSpellImpact(GameObject spell)
    {
        if(spell.GetComponent<LightSpellBehavior>() != null) 
        {
            _openWallPassage.Interaction();
            Destroy(this);
        }
    }

    private void OnValidate()
    {
        if (_openWallPassage == null)
        {
            _openWallPassage = gameObject.GetComponent<OpenWallPassage>();
        }

        if (_particlesPrefab != null && _particles == null)
        {
            _particles = (GameObject)PrefabUtility.InstantiatePrefab(_particlesPrefab, gameObject.transform);
            _particles.SetActive(false);
        }
    }
}
