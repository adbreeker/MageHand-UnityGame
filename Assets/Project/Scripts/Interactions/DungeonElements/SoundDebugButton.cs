using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDebugButton : MonoBehaviour
{
    [Header("Button object")]
    public GameObject button;

    public SoundManager.Sound sound;
    public float maxHearingDistance = 20;
    public float minHearingDistance = 4;

    bool buttonChanging = false;
    private AudioSource clickSound;

    public void OnClick()
    {
        if (!buttonChanging)
        {
            if(clickSound != null) Destroy(clickSound);
            StartCoroutine(ButtonAnimation());
        }
    }

    IEnumerator ButtonAnimation() //button animation
    {
        clickSound = GameParams.Managers.soundManager.CreateAudioSource(sound: sound, soundParent: button, minHearingDistance: minHearingDistance, maxHearingDistance: maxHearingDistance);
        clickSound.Play();
        Destroy(clickSound,clickSound.clip.length);

        buttonChanging = true;
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForFixedUpdate();
            Vector3 newPos = button.transform.localPosition;
            newPos.z -= 0.005f;
            button.transform.localPosition = newPos;
        }
        for (int i = 0; i < 10; i++)
        {

            yield return new WaitForFixedUpdate();
            Vector3 newPos = button.transform.localPosition;
            newPos.z += 0.005f;
            button.transform.localPosition = newPos;
        }
        buttonChanging = false;
    }
}
