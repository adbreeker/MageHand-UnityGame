using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverBehavior : MonoBehaviour
{
    [Header("Lever object")]
    public GameObject lever;

    [Header("Is lever currenlty active")]
    public bool leverON = false;

    [Header("Is lever single use")]
    public bool singleUse = false;

    bool leverChanging = false;
    private AudioSource changingSound;

    public void OnClick() //on lever click invoke interaction on connected object
    {
        if(singleUse)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
        if(!leverChanging)
        {
            StartCoroutine(LeverAnimation());
            foreach(SwitchInteraction switchInteraction in GetComponents<SwitchInteraction>())
            {
                switchInteraction.Interact();
            }
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
    private void OnValidate()
    {
        if (leverON)
        {
            lever.transform.localRotation = Quaternion.Euler(25, 0, 0);
        }
        else
        {
            lever.transform.localRotation = Quaternion.Euler(-25, 0, 0);
        }
    }
}
