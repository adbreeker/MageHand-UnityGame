using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehavior : MonoBehaviour
{
    [Header("Button object")]
    public GameObject button;

    [Header("Button clicks counter")]
    public int clickCounter = 0;
    public int clickCounterCap = 0;

    bool buttonChanging = false;
    private AudioSource clickSound;

    public void OnClick() //on click invoke interaction on connected object and increase click counter
    {
        if (!buttonChanging)
        {
            clickCounter++;
            if(clickCounterCap > 0) { ClickCounterCaping(); }
            StartCoroutine(ButtonAnimation());
            foreach(SwitchInteraction switchInteraction in GetComponents<SwitchInteraction>())
            {
                switchInteraction.Interact();
            }
        }
    }

    void ClickCounterCaping()
    {
        clickCounter = clickCounter % (clickCounterCap + 1);
    }

    IEnumerator ButtonAnimation() //button animation
    {
        clickSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_Button, button);
        clickSound.Play();
        Destroy(clickSound, clickSound.clip.length);

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
        buttonChanging = false;
    }
}
