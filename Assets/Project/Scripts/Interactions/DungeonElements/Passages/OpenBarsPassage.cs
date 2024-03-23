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

    private AudioSource chainSound;

    public void Interaction() //open or close passage on interaction
    {
        StopAllCoroutines();
        if (chainSound != null) Destroy(chainSound);

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
        chainSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_MovingMetalGate, bars, 8f, 30f);
        chainSound.Play();
        while (bars.transform.position.y != barsDestination)
        {
            yield return new WaitForFixedUpdate();
            bars.transform.localPosition = Vector3.MoveTowards(bars.transform.localPosition, new Vector3(0, barsDestination, 0), barsSpeed);
        }
        Destroy(chainSound);
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
