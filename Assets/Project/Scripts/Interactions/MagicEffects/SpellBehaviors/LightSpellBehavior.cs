using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpellBehavior : MonoBehaviour
{
    [Header("Flash effect prefab")]
    public GameObject flashEffectPrefab;

    public void OnThrow()
    {

    }

    public void OnImpact() //on impact spawn flash effect
    {
        Instantiate(flashEffectPrefab, transform.position, Quaternion.identity);
    }
}
