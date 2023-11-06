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
        if (chainSound != null) Destroy(chainSound.gameObject);

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
        chainSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_MovingMetalGate, bars.transform, maxHearingDistance: 20f);
        chainSound.Play();
        while (bars.transform.position.y != barsDestination)
        {
            yield return new WaitForFixedUpdate();
            bars.transform.position = Vector3.MoveTowards(bars.transform.position, new Vector3(bars.transform.position.x, barsDestination, bars.transform.position.z), barsSpeed);
        }
        Destroy(chainSound.gameObject);
    }
}
