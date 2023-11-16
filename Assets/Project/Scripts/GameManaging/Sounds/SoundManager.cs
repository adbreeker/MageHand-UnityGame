using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum Sound
    {
        VOICES_Mage, //? PLACEHOLDER
        VOICES_Guide, //? PLACEHOLDER

        MUSIC_ForestAmbient, //? PLACEHOLDER
        MUSIC_Dungeon, //? PLACEHOLDER
        MUSIC_Menu, //? PLACEHOLDER

        READING_Light, //1

        UI_ChangeOption, //1
        UI_SelectOption, //1
        UI_Close, //1
        UI_Open, //1
        UI_PopUp, //1

        SFX_StepStone1, //0.5
        SFX_StepStone2, //0.5
        SFX_OpenChest, //0.7
        SFX_CloseChest, //0.7
        SFX_Button, //0.8
        SFX_LeverToUp, //0.7
        SFX_LeverToDown, //0.7
        SFX_PickUpItem, //0.7
        SFX_PutToInventory, //0.9
        SFX_Drink, //0.9
        SFX_UnlockOpenDoor, //0.9
        SFX_IllusionBroken, //0.8
        SFX_Collision1, //0.7
        SFX_Collision2, //0.7
        SFX_Collision3, //0.7
        SFX_MovingWall, //0.7
        SFX_MovingMetalGate, //1 

        SFX_CastingSpell, //0.6
        SFX_CastingSpellFailed, //1
        SFX_CastingSpellFinished, //1

        SFX_SpellLightRemaining, //1
        SFX_SpellLightBurst //0.6
    }

    private float volume;

    public SoundAudioClip[] soundAudioClipArray;

    [System.Serializable]
    public class SoundAudioClip
    {
        public Sound sound;
        public AudioClip audioClip;
        public float baseVolume;
    }

    public Transform SoundsParent;

    public AudioSource CreateAudioSource(Sound sound, GameObject soundParent = null, float minHearingDistance = 4f, float maxHearingDistance = 20f)
    {
        AudioSource audioSource;

        if (soundParent != null)
        {
            audioSource = soundParent.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
            audioSource.minDistance = minHearingDistance;
            audioSource.maxDistance = maxHearingDistance;
        }
        else
        {
            GameObject soundGameObject = new GameObject(sound.ToString());
            audioSource = soundGameObject.AddComponent<AudioSource>();

            soundGameObject.transform.parent = SoundsParent;
        }

        audioSource.clip = GetAudioClip(sound);
        audioSource.volume = GetBaseVolume(sound) * volume;

        return audioSource;
    }

    public void ChangeVolume(float givenVolume)
    {
        volume = Mathf.Clamp01(givenVolume);

        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.volume = GetBaseVolume(GetFirstSoundEnumByAudioClip(audioSource.clip)) * volume;
        }
    }

    public void PauseAllAudioSources()
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.Pause();
        }
    }

    public void UnPauseAllAudioSources()
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.UnPause();
        }
    }

    public float GetVolume()
    {
        return volume;
    }

    private AudioClip GetAudioClip(Sound sound)
    {
        foreach(SoundAudioClip soundAudioClip in soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound) return soundAudioClip.audioClip;
        }
        return null;
    }

    private Sound GetFirstSoundEnumByAudioClip(AudioClip audioClip)
    {
        foreach(SoundAudioClip soundAudioClip in soundAudioClipArray)
        {
            if (soundAudioClip.audioClip == audioClip)
            {
                return soundAudioClip.sound;
            }
        }
        return Sound.SFX_CastingSpellFailed;
    }

    private float GetBaseVolume(Sound sound)
    {
        volume = Mathf.Clamp01(volume);
        foreach (SoundAudioClip soundAudioClip in soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound) return soundAudioClip.baseVolume;
        }
        return 0;
    }
}
