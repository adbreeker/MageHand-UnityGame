using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehavior : InteractableBehavior
{
    [Header("Button object")]
    public GameObject button;

    [Header("Button clicks counter")]
    public int clickCounter = 0;
    public int clickCounterCap = 0;

    bool buttonChanging = false;

    public Action<GameObject> OnButtonClickedEvents;

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
            OnButtonClickedEvents?.Invoke(gameObject);
        }
    }

    void ClickCounterCaping()
    {
        clickCounter = clickCounter % (clickCounterCap + 1);
    }

    IEnumerator ButtonAnimation() //button animation
    {
        GameParams.Managers.audioManager.PlayOneShotSpatialized(GameParams.Managers.fmodEvents.SFX_ButtonPress, button.transform);

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
