using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWallPassage : MonoBehaviour
{
    public bool passageOpen = false;
    public GameObject wall;
    public float wallSpeed = 0.03f;

    public void Interaction()
    {
        StopAllCoroutines();

        if (passageOpen)
        {
            passageOpen = false;
            StartCoroutine(MoveWall(0.0f));
        }
        else
        {
            passageOpen = true;
            StartCoroutine(MoveWall(-5.5f));
        }

    }

    IEnumerator MoveWall(float wallDestination)
    {
        while (wall.transform.position.y != wallDestination)
        {
            yield return new WaitForFixedUpdate();
            wall.transform.position = Vector3.MoveTowards(wall.transform.position, new Vector3(wall.transform.position.x, wallDestination, wall.transform.position.z), wallSpeed);
        }
    }
}
