using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBehavior : MonoBehaviour
{
    private void Update() //if key is near locked doors then open those doors
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

    public void OnPickUp()
    {

    }
}
