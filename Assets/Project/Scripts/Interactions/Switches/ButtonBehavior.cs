using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehavior : MonoBehaviour
{
    public GameObject button;

    bool buttonChanging = false;

    public void OnClick()
    {
        if (!buttonChanging)
        {
            StartCoroutine(ButtonAnimation());
            GetComponent<SwitchInteraction>().Interact();
        }
    }

    IEnumerator ButtonAnimation()
    {
        buttonChanging = true;
        for(int i=0; i<10; i++)
        {

            yield return new WaitForSeconds(0.01f);
            Vector3 newPos = button.transform.localPosition;
            newPos.z -= 0.005f;
            button.transform.localPosition = newPos;
        }
        for (int i = 0; i < 10; i++)
        {

            yield return new WaitForSeconds(0.01f);
            Vector3 newPos = button.transform.localPosition;
            newPos.z += 0.005f;
            button.transform.localPosition = newPos;
        }
        buttonChanging = false;
    }
}
