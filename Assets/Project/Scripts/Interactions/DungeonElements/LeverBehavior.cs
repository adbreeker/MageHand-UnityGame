using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverBehavior : MonoBehaviour
{
    [Header("Lever object")]
    public GameObject lever;

    [Header("Is lever currenlty active")]
    public bool leverON = false;

    bool leverChanging = false;
    private AudioSource changingSound;

    public void OnClick() //on lever click invoke interaction on connected object
    {
        if(!leverChanging)
        {
            StartCoroutine(LeverAnimation());
            GetComponent<SwitchInteraction>().Interact();
        }
    }

    IEnumerator LeverAnimation() //animating lever movement
    {
        leverChanging = true;
        if(leverON) //if leverOn then lever go up
        {
            changingSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_LeverToUp, gameObject);
            changingSound.Play();

            for (int i = 1; i<=10; i++)
            {
                yield return new WaitForFixedUpdate();
                lever.transform.rotation *= Quaternion.Euler(-5, 0, 0);
            }
            leverON = false;
        }
        else //else lever go down
        {
            changingSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_LeverToDown);
            changingSound.Play();

            for (int i = 1; i <= 10; i++)
            {
                yield return new WaitForFixedUpdate();
                lever.transform.rotation *= Quaternion.Euler(5, 0, 0);
            }
            leverON = true;
        }

        while (changingSound.isPlaying) yield return null;

        Destroy(changingSound);
        leverChanging = false;
    }


    // changing state in editor;
    bool previousLeverState = false;
    private void OnValidate()
    {
        if(previousLeverState != leverON)
        {
            previousLeverState = leverON;
            if (leverON)
            {
                lever.transform.rotation *= Quaternion.Euler(50, 0, 0);
            }
            else
            {
                lever.transform.rotation *= Quaternion.Euler(-50, 0, 0);
            }
        }
    }
}
