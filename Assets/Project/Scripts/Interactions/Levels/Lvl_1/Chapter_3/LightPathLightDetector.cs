using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPathLightDetector : MonoBehaviour
{
    [SerializeField] Transform _nextDetector;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<LightSpellBehavior>() != null)
        {
            RotateTowardsNextDetector(other.gameObject);
        }
    }

    void RotateTowardsNextDetector(GameObject lightSpell)
    {
        Vector3 nextDetectorPos = new Vector3(_nextDetector.transform.position.x, lightSpell.transform.position.y, _nextDetector.transform.position.z);
        lightSpell.transform.LookAt(nextDetectorPos);
        lightSpell.GetComponent<Rigidbody>().velocity = lightSpell.transform.forward * 5;
    }

}
