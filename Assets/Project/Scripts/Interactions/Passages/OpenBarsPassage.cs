using System.Collections;
using UnityEngine;

public class OpenBarsPassage : MonoBehaviour
{
    public bool passageOpen = false;
    public GameObject bars;
    public float barsSpeed = 0.05f;

    public void Interaction()
    {
        StopAllCoroutines();

        if (passageOpen)
        {
            passageOpen = false;
            StartCoroutine(MoveBars(0.0f));
        }
        else
        {
            passageOpen = true;
            StartCoroutine(MoveBars(2.5f));
        }

    }

    IEnumerator MoveBars(float barsDestination)
    {
        while (bars.transform.position.y != barsDestination)
        {
            yield return new WaitForFixedUpdate();
            bars.transform.position = Vector3.MoveTowards(bars.transform.position, new Vector3(bars.transform.position.x, barsDestination, bars.transform.position.z), barsSpeed);
        }
    }
}
