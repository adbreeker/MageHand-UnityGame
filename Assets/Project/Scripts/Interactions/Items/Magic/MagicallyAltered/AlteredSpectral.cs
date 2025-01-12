using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlteredSpectral : MonoBehaviour
{
    public void OnThrow()
    {
        GetComponent<Rigidbody>().useGravity = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<Rigidbody>().useGravity = true;
    }
}
