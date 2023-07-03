using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeBehavior : MonoBehaviour
{
    public void OnPickUp() //fix knife rotation on pick up
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 front = player.transform.forward * -1000;
        front.y = -2000;
        transform.LookAt(front);
    }
}
