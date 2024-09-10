using FMOD.Studio;
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

    private EventInstance chainSound;

    public void Interaction() //open or close passage on interaction
    {
        StopAllCoroutines();
        if (chainSound.isValid()) chainSound.stop(STOP_MODE.IMMEDIATE);

        if (passageOpen) //if passege is open then close
        {
            passageOpen = false;
            StartCoroutine(MoveBars(0.0f));
        }
        else //else open passage
        {
            passageOpen = true;
            StartCoroutine(MoveBars(1.95f));
        }

    }

    IEnumerator MoveBars(float barsDestination) //animating passage opening
    {
        chainSound = GameParams.Managers.audioManager.PlayOneShotOccluded(GameParams.Managers.fmodEvents.SFX_MovingMetalGate, bars.transform);
        while (bars.transform.localPosition.y != barsDestination)
        {
            yield return new WaitForFixedUpdate();
            bars.transform.localPosition = Vector3.MoveTowards(bars.transform.localPosition, new Vector3(0, barsDestination, 0), barsSpeed);
        }
        chainSound.stop(STOP_MODE.IMMEDIATE);
    }

    private void OnValidate()
    {
        if (passageOpen)
        {
            bars.transform.localPosition = new Vector3(0, 1.95f, 0);
        }
        else
        {
            bars.transform.localPosition = new Vector3(0, 0, 0);
        }
    }
}
