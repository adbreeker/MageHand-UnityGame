using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehavior : MonoBehaviour
{
    public GameObject button;
    public int clickCounter = 0;

    bool buttonChanging = false;

    public void OnClick()
    {
        if (!buttonChanging)
        {
            clickCounter++;
            StartCoroutine(ButtonAnimation());
            GetComponent<SwitchInteraction>().Interact();
        }
    }

    IEnumerator ButtonAnimation()
    {
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
