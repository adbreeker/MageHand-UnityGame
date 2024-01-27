using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum Sound
    {
        VOICES_Mage, //1
        VOICES_Guide, //1

        MUSIC_Dungeon1Start, //0.25
        MUSIC_Dungeon1Loop, //0.25
        MUSIC_Dungeon2Start,
        MUSIC_Dungeon2Loop,
        MUSIC_Dungeon3Start,
        MUSIC_Dungeon3Loop,
        MUSIC_Dungeon4Start,
        MUSIC_Dungeon4Loop,
        MUSIC_Dungeon5Start,
        MUSIC_Dungeon5Loop,
        MUSIC_MenuStart, //?? PLACEHOLDER
        MUSIC_MenuLoop, //?? PLACEHOLDER
        MUSIC_LoadingAmbientStart, //?? PLACEHOLDER
        MUSIC_LoadingAmbientLoop, //?? PLACEHOLDER

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
        SFX_UnlockOpenDoor, //0.8
        SFX_IllusionBroken, //0.8
        SFX_Collision1, //0.7
        SFX_Collision2, //0.7
        SFX_Collision3, //0.7
        SFX_MovingWall, //0.7
        SFX_MovingMetalGate, //1 

        SFX_CastingSpell, //0.6
        SFX_CastingSpellFailed, //1
        SFX_CastingSpellFinished, //1

        READING_Light, //1
        READING_PickUp, //1
        READING_Fire,
        READING_Mark,
        READING_Return,
        READING_Levitate,

        SFX_SpellLightRemaining, //1
        SFX_SpellLightBurst, //0.6
        SFX_SpellPickUpActivation, //0.9
        SFX_MagicalTeleportation, //0.8
        SFX_LockPickSpell,
        SFX_FireSpellRemaining, //0.6
        SFX_FireSpellBurst, //1
        SFX_MarkSpellActivation,
        SFX_ReturnSpellActivation,
        SFX_LevitateSpellRemaining,

        SFX_Earthquake, //1
        SFX_BodyFall, //1
        SFX_Punch //0.9
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

    private List<Coroutine> fadingOutCoroutines = new List<Coroutine>();
    private List<Coroutine> fadingInCoroutines = new List<Coroutine>();
    public AudioSource CreateAudioSource(Sound sound, GameObject soundParent = null, float minHearingDistance = 4f, float maxHearingDistance = 20f)
    {
        AudioSource audioSource;

        if (soundParent != null)
        {
            audioSource = soundParent.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
            audioSource.minDistance = minHearingDistance;
            audioSource.maxDistance = maxHearingDistance;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
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

    public void ChangeVolume(float givenVolume, bool fromPauseMenu = false)
    {
        volume = Mathf.Clamp01(givenVolume);

        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in audioSources)
        {
            if (fromPauseMenu && !GetFirstSoundEnumByAudioClip(audioSource.clip).ToString().StartsWith("MUSIC_"))
                audioSource.volume = GetBaseVolume(GetFirstSoundEnumByAudioClip(audioSource.clip)) * volume;

            if (!fromPauseMenu) audioSource.volume = GetBaseVolume(GetFirstSoundEnumByAudioClip(audioSource.clip)) * volume;
        }
    }

    public void PauseAllAudioSourcesAndFadeOutMusic()
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        fadingOutCoroutines = new List<Coroutine>();

        foreach(Coroutine coroutine in fadingInCoroutines) if (coroutine != null) StopCoroutine(coroutine);

        foreach (AudioSource audioSource in audioSources)
        {
            if (!GetFirstSoundEnumByAudioClip(audioSource.clip).ToString().StartsWith("MUSIC_")) audioSource.Pause();
            else
            {
                fadingOutCoroutines.Add(StartCoroutine(FadeOutAudioSource(audioSource)));
            }
        }
    }
    public void UnPauseAllAudioSourcesFadeInMusic()
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        fadingInCoroutines = new List<Coroutine>();

        foreach (Coroutine coroutine in fadingOutCoroutines) if (coroutine != null) StopCoroutine(coroutine);

        foreach (AudioSource audioSource in audioSources)
        {
            if (!GetFirstSoundEnumByAudioClip(audioSource.clip).ToString().StartsWith("MUSIC_")) audioSource.UnPause();
            else if(!GameSettings.muteMusic)
            {
                fadingOutCoroutines.Add(StartCoroutine(FadeInAudioSource(audioSource)));
            }
        }
    }
    IEnumerator FadeOutAudioSource(AudioSource audioSource)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.unscaledDeltaTime * 5;
            yield return new WaitForSecondsRealtime(0.02f);
        }
    }
    IEnumerator FadeInAudioSource(AudioSource audioSource)
    {
        float targetVolume = GetBaseVolume(GetFirstSoundEnumByAudioClip(audioSource.clip)) * volume;
        if(audioSource.volume == targetVolume) audioSource.volume = 0;
        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += targetVolume * Time.unscaledDeltaTime * 5;
            yield return new WaitForSecondsRealtime(0.02f);
        }
    }

    public void FadeOutSFXs()
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in audioSources)
        {
            if (GetFirstSoundEnumByAudioClip(audioSource.clip).ToString().StartsWith("SFX_"))
            {
                StartCoroutine(FadeOutAudioSource(audioSource));
            }
        }
    }

    public float GetVolume()
    {
        return volume;
    }

    public AudioClip GetAudioClip(Sound sound)
    {
        foreach(SoundAudioClip soundAudioClip in soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound) return soundAudioClip.audioClip;
        }
        return null;
    }

    public Sound GetFirstSoundEnumByAudioClip(AudioClip audioClip)
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

    public float GetBaseVolume(Sound sound)
    {
        volume = Mathf.Clamp01(volume);
        foreach (SoundAudioClip soundAudioClip in soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound) return soundAudioClip.baseVolume;
        }
        return 0;
    }
}