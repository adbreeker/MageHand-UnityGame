using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public BackgroundMusic backgroundMusic;

    private Coroutine fadeOutPauseMenuCoroutine;
    private Coroutine fadeInPauseMenuCoroutine;

    private void Awake()
    {
        SetBusVolume(FmodBuses.UI, 1f);
        SetBusVolume(FmodBuses.Music, 1f);
        SetBusVolume(FmodBuses.SFX, 1f);
        FmodBuses.UI.setPaused(false);
        FmodBuses.Music.setPaused(false);
        FmodBuses.SFX.setPaused(false);
    }

    public StudioEventEmitter CreateAudioEmitter(EventReference eventRef, GameObject soundParent)
    {
        StudioEventEmitter eventEmitter;
        eventEmitter = soundParent.AddComponent<StudioEventEmitter>();
        eventEmitter.EventReference = eventRef;
        return eventEmitter;
    }

    public void SetGameVolume(float givenVolume)
    {
        SetBusVolume(FmodBuses.Master, givenVolume);
    }

    public float GetGameVolume()
    {
        FmodBuses.Master.getVolume(out float volume);
        return volume;
    }

    public void SetBusVolume(Bus bus, float volume)
    {
        volume = Mathf.Clamp01(volume);
        bus.setVolume(volume);
    }

    public float GetBusVolume(Bus bus)
    {
        bus.getVolume(out float volume);
        return volume;
    }

    public void PauseSFXsFadeOutMusic(float fadeSpeed)
    {
        FmodBuses.SFX.setPaused(true);
        if (fadeInPauseMenuCoroutine != null) StopCoroutine(fadeInPauseMenuCoroutine);
        fadeOutPauseMenuCoroutine = StartCoroutine(FadeOutBus(FmodBuses.Music, fadeSpeed));
    }
    public void UnpauseSFXsFadeInMusic(float fadeSpeed)
    {
        FmodBuses.SFX.setPaused(false);
        if (fadeOutPauseMenuCoroutine != null) StopCoroutine(fadeOutPauseMenuCoroutine);
        fadeInPauseMenuCoroutine = StartCoroutine(FadeInBus(FmodBuses.Music, fadeSpeed));
    }

    public IEnumerator FadeOutBus(Bus bus, float fadeSpeed)
    {
        bus.getVolume(out float volume);
        while (volume > 0)
        {
            //Debug.Log("FadeOut: " + volume);
            SetBusVolume(bus, volume - fadeSpeed);
            yield return new WaitForSecondsRealtime(0);
            bus.getVolume(out volume);
        }

        bus.setPaused(true);
    }

    public IEnumerator FadeInBus(Bus bus, float fadeSpeed)
    {
        bus.setPaused(false);

        bus.getVolume(out float volume);
        while (volume < 1)
        {
            //Debug.Log("FadeIn: " + volume);
            SetBusVolume(bus, volume + fadeSpeed);
            yield return new WaitForSecondsRealtime(0);
            bus.getVolume(out volume);
        }
    }
}