using System.Collections;
using UnityEngine;

public class OpenBarsPassage : MonoBehaviour
{
    public bool passageOpen = false;
    public GameObject bars;
    public float barsSpeed = 2.0f;


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
            yield return new WaitForSeconds(0.01f);
            bars.transform.position = Vector3.MoveTowards(bars.transform.position, new Vector3(bars.transform.position.x, barsDestination, bars.transform.position.z), barsSpeed * Time.deltaTime);
        }
    }
}
