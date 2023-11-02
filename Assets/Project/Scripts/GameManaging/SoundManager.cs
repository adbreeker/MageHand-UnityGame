using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum Sound
    {
        READING_Light,

        SFX_StepStone1,
        SFX_StepStone2,
        SFX_OpenChest,
        SFX_CloseChest,

        VOICES_mage,
        VOICES_guide
    }

    public SoundAudioClip[] soundAudioClipArray;

    [System.Serializable]
    public class SoundAudioClip
    {
        public Sound sound;
        public AudioClip audioClip;
    }

    public Transform SoundsParent;

    public AudioSource CreateAudioSource(Sound sound)
    {
        GameObject soundGameObject = new GameObject(sound.ToString());
        soundGameObject.transform.parent = SoundsParent;
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.clip = GetAudioClip(sound);
        return audioSource;
    }

    private AudioClip GetAudioClip(Sound sound)
    {
        foreach(SoundAudioClip soundAudioClip in soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound) return soundAudioClip.audioClip;
        }
        return null;
    }
}
