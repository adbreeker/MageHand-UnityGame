using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWallPassage : MonoBehaviour
{
    [Header("Is passage open")]
    public bool passageOpen = false;

    [Header("Wall")]
    public GameObject wall;

    [Header("Opening speed")]
    public float wallSpeed = 0.03f;

    private AudioSource wallSound;

    public void Interaction() //open or close passage on interaction
    {
        StopAllCoroutines();
        if (wallSound != null) Destroy(wallSound);

        if (passageOpen) //if passage open then close
        {
            passageOpen = false;
            StartCoroutine(MoveWall(0.0f));
        }
        else // else open passage
        {
            passageOpen = true;
            StartCoroutine(MoveWall(-5.5f));    
        }

    }

    IEnumerator MoveWall(float wallDestination) //animating passage opening
    {
        wallSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_MovingWall, wall, 8f, 30f);
        wallSound.Play();

        while (wall.transform.position.y != wallDestination)
        {
            yield return new WaitForFixedUpdate();
            wall.transform.position = Vector3.MoveTowards(wall.transform.position, new Vector3(wall.transform.position.x, wallDestination, wall.transform.position.z), wallSpeed);
        }
        Destroy(wallSound);
    }
}
