using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBehavior : MonoBehaviour
{
    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f, 1);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "Lock")
            {
                collider.SendMessage("OpenDoors");
                Destroy(gameObject);
                break;
            }
        }
    }
}
