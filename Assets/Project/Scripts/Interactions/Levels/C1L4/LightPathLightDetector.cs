using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPathLightDetector : MonoBehaviour
{
    [SerializeField] LightPathPuzzle _puzzleScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<LightSpellBehavior>() != null && other.transform.parent == null)
        {
            _puzzleScript.LeadLightThroughPath(other.gameObject.GetComponent<LightSpellBehavior>());
        }
    }
}
