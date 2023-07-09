using System.Collections;
using UnityEngine;

public class OpenBarsPassage : MonoBehaviour
{
    [Header("Is passage open")]
    public bool passageOpen = false;

    [Header("Bars to open")]
    public GameObject bars;

    [Header("Opening speed")]
    public float barsSpeed = 0.05f;

    public void Interaction() //open or close passage on interaction
    {
        StopAllCoroutines();

        if (passageOpen) //if passege is open then close
        {
            passageOpen = false;
            StartCoroutine(MoveBars(0.0f));
        }
        else //else open passage
        {
            passageOpen = true;
            StartCoroutine(MoveBars(2.5f));
        }

    }

    IEnumerator MoveBars(float barsDestination) //animating passage opening
    {
        while (bars.transform.position.y != barsDestination)
        {
            yield return new WaitForFixedUpdate();
            bars.transform.position = Vector3.MoveTowards(bars.transform.position, new Vector3(bars.transform.position.x, barsDestination, bars.transform.position.z), barsSpeed);
        }
    }
}
