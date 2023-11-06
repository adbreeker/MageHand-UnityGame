using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollisionSound : MonoBehaviour
{
    private AudioSource collisionSound;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "DungeonCube")
        {
            int random = Random.Range(1, 4);

            if (random == 1) collisionSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_Collision1, transform, maxHearingDistance: 15f);
            else if (random == 2) collisionSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_Collision2, transform, maxHearingDistance: 15f);
            else if (random == 3) collisionSound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_Collision3, transform, maxHearingDistance: 15f);

            collisionSound.Play();
            Destroy(collisionSound.gameObject, collisionSound.clip.length);
        }
    }
}
