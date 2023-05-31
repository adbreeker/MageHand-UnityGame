using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLockedDoorsPassage : MonoBehaviour
{
    public GameObject doors;

    public void OpenDoors()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(RotateDoors());
    }

    IEnumerator RotateDoors()
    {
        for(int i = 0; i<90; i++)
        {
            yield return new WaitForSeconds(0.01f);
            doors.transform.localRotation *= Quaternion.Euler(0, -1, 0);
        }
        doors.tag = "Untagged";
        gameObject.SetActive(false);
    }
}
