using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLockedDoorsPassage : MonoBehaviour
{
    [Header("Doors to open")]
    public GameObject doors;

    private AudioSource openDoorsSound;
    public void OpenDoors() //open assigned doors
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(RotateDoors());
    }

    IEnumerator RotateDoors() //animating doors opening, then change doors to not obstacle, and make lock object unactive
    {
        openDoorsSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_UnlockOpenDoor, doors.transform);
        openDoorsSound.Play();
        for(int i = 0; i<90; i++)
        {
            yield return new WaitForFixedUpdate();
            doors.transform.localRotation *= Quaternion.Euler(0, -1, 0);
        }
        doors.tag = "Untagged";

        Destroy(openDoorsSound.gameObject, openDoorsSound.clip.length);
        gameObject.SetActive(false);
    }
}
