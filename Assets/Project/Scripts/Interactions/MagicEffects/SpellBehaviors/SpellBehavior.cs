using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBehavior : MonoBehaviour
{
    [Header("Special effect")]
    public GameObject specialEffectPrefab;

    [Header("Sound distance:")]
    public float minHearingDistance = 10.0f;
    public float maxHearingDistance = 30.0f;

    public virtual void OnThrow() //rotate fireball on throw to face good direction
    {

    }

    public virtual void OnImpact(GameObject impactTarget) //spawn explosion on impact
    {

    }
}
