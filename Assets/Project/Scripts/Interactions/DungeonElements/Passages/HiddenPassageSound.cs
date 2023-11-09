using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenPassageSound : MonoBehaviour
{
    private AudioSource sound;
    private bool activateSound = true;

    private void Start()
    {
        sound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_IllusionBroken);
    }
    private void Update()
    {
        Bounds cubeBounds = GetComponent<Renderer>().bounds;
        if (cubeBounds.Contains(PlayerParams.Objects.player.transform.position) && activateSound)
        {
            sound.Play();
            activateSound = false;
            Destroy(this, sound.clip.length);
        }
    }
}