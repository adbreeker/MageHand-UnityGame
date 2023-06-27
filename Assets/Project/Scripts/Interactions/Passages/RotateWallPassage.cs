using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWallPassage : MonoBehaviour
{
    public GameObject wall;
    public Transform pivot;

    public float rotationMultiplier = 1.0f;

    bool wallMoving = false;

    public void Interaction()
    {
        if(!wallMoving)
        {
            wallMoving = true;
            StartCoroutine(RotateDoors());
        }
    }

    IEnumerator RotateDoors()
    {
        for (int i = 0; i < 90; i++)
        {
            yield return new WaitForFixedUpdate();
            wall.transform.RotateAround(pivot.position ,new Vector3(0, 1, 0), 1.0f * rotationMultiplier);
        }
        //gameObject.SetActive(false);
    }
}
