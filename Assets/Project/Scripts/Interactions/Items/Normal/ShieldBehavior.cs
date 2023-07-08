using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBehavior : MonoBehaviour
{
    public void OnPickUp() //fix shield rotation on pick up
    {
        GameObject player = PlayerParams.Objects.player;
        Vector3 front = player.transform.forward * 1000;
        transform.LookAt(front);
    }
}
