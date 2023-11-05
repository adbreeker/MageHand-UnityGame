using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehavior : MonoBehaviour
{
    [Header("Button object")]
    public GameObject button;

    [Header("Button clicks counter")]
    public int clickCounter = 0;

    bool buttonChanging = false;
    private AudioSource clickSound;

    public void OnClick() //on click invoke interaction on connected object and increase click counter
    {
        if (!buttonChanging)
        {
            clickCounter++;
            StartCoroutine(ButtonAnimation());
            GetComponent<SwitchInteraction>().Interact();
        }
    }

    IEnumerator ButtonAnimation() //button animation
    {
        clickSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_Button);
        clickSound.Play();

        buttonChanging = true;
        for(int i=0; i<10; i++)
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

        while (clickSound.isPlaying) yield return null;

        Destroy(clickSound.gameObject);
        buttonChanging = false;
    }
}
