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
        UI_Open, //? PLACEHOLDER
        UI_PopUp, //1

        SFX_StepStone1, //0.7
        SFX_StepStone2, //0.7
        SFX_OpenChest, //0.9
        SFX_CloseChest, //0.9
        SFX_Button, //0.9
        SFX_LeverToUp, //0.7
        SFX_LeverToDown, //0.7
        SFX_PickUpItem,
        SFX_PutToInventory, //0.9
        SFX_Drink, //0.9
        SFX_IllusionBroken,
        SFX_ObjectHittingWall,
        SFX_MovingWall,
        SFX_MovingMetalGate,

        SFX_StartCastingSpell,
        SFX_CastingSpell,
        SFX_SpellLightCasted,
        SFX_SpellLightRemaining,
        SFX_SpellLightBurst
    }

    public float volume = 0.3f;
    public SoundAudioClip[] soundAudioClipArray;

    [System.Serializable]
    public class SoundAudioClip
    {
        public Sound sound;
        public AudioClip audioClip;
        public float baseVolume;
    }

    public Transform SoundsParent;

    public AudioSource CreateAudioSource(Sound sound)
    {
        GameObject soundGameObject = new GameObject(sound.ToString());
        soundGameObject.transform.parent = SoundsParent;
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.clip = GetAudioClip(sound);
        audioSource.volume = GetBaseVolume(sound) * volume;
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

    private float GetBaseVolume(Sound sound)
    {
        foreach (SoundAudioClip soundAudioClip in soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound) return soundAudioClip.baseVolume;
        }
        return 0;
    }
}
