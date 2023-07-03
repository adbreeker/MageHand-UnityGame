using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour //special camera movement for withdrawn level
{
    [Header("Rotation on X axis")]
    public float xRot;

    [Header("Rotation on Z axis")]
    public float yRot;

    void Update() //rotate camera with WSAD and clamp it on some values
    {
        xRot = transform.rotation.eulerAngles.x;
        yRot = transform.rotation.eulerAngles.y;

        if (Input.GetKey(KeyCode.W))
        {
            xRot -= 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            xRot += 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            yRot -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            yRot += 1;
        }

        xRot = Mathf.Clamp(xRot, 0.0f, 60.0f);
        if(yRot > 300.0f)
        {
            if(yRot < 330.0f)
            {
                yRot = 330.0f;
            }
        }
        if(yRot < 60)
        {
            if(yRot > 30.0f)
            {
                yRot = 30.0f;
            }
        }
        transform.rotation = Quaternion.Euler(xRot, yRot, 0);
    }
}
