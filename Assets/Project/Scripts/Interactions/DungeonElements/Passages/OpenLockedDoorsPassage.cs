using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLockedDoorsPassage : MonoBehaviour
{
    [Header("Doors to open")]
    public GameObject doors;

    [Header("Doors rotation")]
    public bool rotateOutside = true;

    [Header("Is door open?")]
    public bool doorsOpen = false;

    private void Start()
    {
        if(doorsOpen) { doors.tag = "Untagged"; }
        else { doors.tag = "Wall"; }
    }

    public void Interaction() //open assigned doors
    {
        StopAllCoroutines();
        if (doorsOpen) //if passege is open then close
        {
            doorsOpen = false;
            StartCoroutine(RotateDoors(0));
        }
        else //else open passage
        {
            doorsOpen = true;
            if (rotateOutside) { StartCoroutine(RotateDoors(-90)); }
            else { StartCoroutine(RotateDoors(90)); }
        }
    }

    IEnumerator RotateDoors(float destinationDegree) //animating doors opening, then change doors to not obstacle, and make lock object unactive
    {
        GameParams.Managers.audioManager.PlayOneShotSpatialized(GameParams.Managers.fmodEvents.SFX_UnlockOpenDoor, doors.transform);

        while (transform.localRotation.eulerAngles.y  != destinationDegree)
        {
            yield return new WaitForFixedUpdate();
            doors.transform.localRotation = Quaternion.RotateTowards(doors.transform.localRotation, Quaternion.Euler(0, destinationDegree, 0), 1f);

            if(doors.transform.localRotation.eulerAngles.y < 300 && doors.transform.localRotation.eulerAngles.y != 0)
            {
                //Debug.Log("otwarte = " + doors.transform.localRotation.eulerAngles.y);
                doors.tag = "Untagged";
            }
            else
            {
                //Debug.Log("zamkniete = " + doors.transform.localRotation.eulerAngles.y);
                doors.tag = "Wall";
            }
        }
    }

    private void OnValidate()
    {
        if (doorsOpen)
        {
            if(rotateOutside)
            {
                doors.transform.localRotation = Quaternion.Euler(0, -90, 0);
            }
            else
            {
                doors.transform.localRotation = Quaternion.Euler(0, 90, 0);
            }
        }
        else
        {
            doors.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
