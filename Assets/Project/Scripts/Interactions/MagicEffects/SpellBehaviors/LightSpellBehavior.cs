using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpellBehavior : MonoBehaviour
{
    public GameObject flashEffectPrefab;

    public void OnThrow()
    {

    }

    public void OnImpact()
    {
        Instantiate(flashEffectPrefab, transform.position, Quaternion.identity);
    }
}
