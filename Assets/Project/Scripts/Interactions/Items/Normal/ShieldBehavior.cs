using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBehavior : MonoBehaviour
{
    public void OnPickUp()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 front = player.transform.forward * 1000;
        transform.LookAt(front);
    }
}
