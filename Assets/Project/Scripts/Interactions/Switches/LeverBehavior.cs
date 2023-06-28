using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverBehavior : MonoBehaviour
{
    public GameObject lever;
    public bool leverON = false;

    bool leverChanging = false;

    public void OnClick()
    {
        if(!leverChanging)
        {
            StartCoroutine(LeverAnimation());
            GetComponent<SwitchInteraction>().Interact();
        }
    }

    IEnumerator LeverAnimation()
    {
        leverChanging = true;
        if(leverON)
        {
            for(int i = 1; i<=10; i++)
            {
                yield return new WaitForFixedUpdate();
                lever.transform.rotation *= Quaternion.Euler(-5, 0, 0);
            }
            leverON = false;
        }
        else
        {
            for (int i = 1; i <= 10; i++)
            {
                yield return new WaitForFixedUpdate();
                lever.transform.rotation *= Quaternion.Euler(5, 0, 0);
            }
            leverON = true;
        }
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
