using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public BackgroundMusic backgroundMusic;
    public LayerMask occlusionLayer;

    private Coroutine fadeOutPauseMenuCoroutine;
    private Coroutine fadeInPauseMenuCoroutine;

    private void Awake()
    {
        SetBusVolume(FmodBuses.Nonpausable, 1f);
        SetBusVolume(FmodBuses.Music, 1f);
        SetBusVolume(FmodBuses.SFX, 1f);

        FmodBuses.Nonpausable.setPaused(false);
        FmodBuses.Music.setPaused(false);
        FmodBuses.SFX.setPaused(false);
    }

    public EventInstance CreateSpatializedInstance(EventReference eventRef, Transform audioParent)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, audioParent);
        return eventInstance;
    }

    public EventInstance CreateOccludedInstance(EventReference eventRef, Transform audioParent, float audioOcclusionWidening = 1f, float playerOcclusionWidening = 1f)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, audioParent);

        AudioOcclusion audioOcclusion = audioParent.gameObject.AddComponent<AudioOcclusion>();
        audioOcclusion.audioEvent = eventInstance;
        audioOcclusion.audioRef = eventRef;
        audioOcclusion.occlusionLayer = occlusionLayer;
        audioOcclusion.audioOcclusionWidening = audioOcclusionWidening;
        audioOcclusion.playerOcclusionWidening = playerOcclusionWidening;

        return eventInstance;
    }

    public EventInstance PlayOneShotSpatialized(EventReference eventRef, Transform audioParent)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, audioParent);
        eventInstance.start();
        eventInstance.release();
        return eventInstance;
    }

    public EventInstance PlayOneShotOccluded(EventReference eventRef, Transform audioParent, float audioOcclusionWidening = 1f, float playerOcclusionWidening = 1f)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, audioParent);

        AudioOcclusion audioOcclusion = audioParent.gameObject.AddComponent<AudioOcclusion>();
        audioOcclusion.audioEvent = eventInstance;
        audioOcclusion.audioRef = eventRef;
        audioOcclusion.occlusionLayer = occlusionLayer;
        audioOcclusion.audioOcclusionWidening = audioOcclusionWidening;
        audioOcclusion.playerOcclusionWidening = playerOcclusionWidening;

        eventInstance.start();
        eventInstance.release();
        return eventInstance;
    }

    public EventInstance PlayOneShotReturnInstance(EventReference eventRef)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
        eventInstance.start();
        eventInstance.release();
        return eventInstance;
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

    public void SetInstanceVolume(EventInstance eventInstance, float volume)
    {
        volume = Mathf.Clamp01(volume);
        eventInstance.setVolume(volume);
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
            SetBusVolume(bus, volume + fadeSpeed);
            yield return new WaitForSecondsRealtime(0);
            bus.getVolume(out volume);
        }
    }
}