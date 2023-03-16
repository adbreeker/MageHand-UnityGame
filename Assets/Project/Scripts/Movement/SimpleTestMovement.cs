using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTestMovement : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        MovementKeysListener();
        RotationKeyListener();
    }

    void MovementKeysListener()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            transform.position += transform.forward * 4;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.position += transform.right * -4;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.position += transform.forward * -4;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.position += transform.right * 4;
        }
    }

    void RotationKeyListener()
    {
        Vector3 eulerRot = transform.rotation.eulerAngles;
        if (Input.GetKeyDown(KeyCode.E))
        {
            transform.Rotate(Vector3.up * 90);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            transform.Rotate(Vector3.up * -90);
        }
    }
}
