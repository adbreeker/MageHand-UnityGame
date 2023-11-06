using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWallPassage : MonoBehaviour
{
    [Header("Wall")]
    public GameObject wall;

    [Header("Rotating arround point")]
    public Transform pivot;

    [Header("Rotatrion multiplier")]
    public float rotationMultiplier = 1.0f;

    bool wallMoving = false;

    private AudioSource wallSound;

    public void Interaction() //on iteraction start rotating
    {
        if(!wallMoving)
        {
            wallMoving = true;
            StartCoroutine(RotateDoors());
        }
    }

    IEnumerator RotateDoors() //walls rotation animation
    {
        wallSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_MovingWall, wall.transform, 8f, 20f);
        wallSound.gameObject.transform.localPosition = new Vector3(0, 2, 0);
        wallSound.Play();
        for (int i = 0; i < 90; i++)
        {
            yield return new WaitForFixedUpdate();
            wall.transform.RotateAround(pivot.position ,new Vector3(0, 1, 0), 1.0f * rotationMultiplier);
        }
        Destroy(wallSound.gameObject);
    }
}
