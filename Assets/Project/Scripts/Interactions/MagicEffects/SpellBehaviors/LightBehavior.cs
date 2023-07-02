using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBehavior : MonoBehaviour
{
    public GameObject flashEffectPrefab;

    public void OnImpact()
    {
        Instantiate(flashEffectPrefab, transform.position, Quaternion.identity);
    }
}
