using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
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

    public void PauseSFXsFadeOutMusic()
    {
        FmodBuses.SFX.setPaused(true);
        FadeOutBus(FmodBuses.Music);
    }
    public void UnPauseSFXsFadeInMusic()
    {
        FmodBuses.SFX.setPaused(false);
        FadeInBus(FmodBuses.Music);
    }

    IEnumerator FadeOutBus(Bus bus)
    {
        bus.getVolume(out float volume);
        while (volume > 0)
        {
            SetBusVolume(bus, volume - Time.unscaledDeltaTime * 5f);
            yield return new WaitForSecondsRealtime(0.02f);
            bus.getVolume(out volume);
        }

        bus.setPaused(true);
    }

    IEnumerator FadeInBus(Bus bus)
    {
        bus.setPaused(false);

        bus.getVolume(out float volume);
        while (volume < 1)
        {
            SetBusVolume(bus, volume + Time.unscaledDeltaTime * 5f);
            yield return new WaitForSecondsRealtime(0.02f);
            bus.getVolume(out volume);
        }
    }

    public void SetBusVolume(Bus bus, float volume)
    {
        volume = Mathf.Clamp01(volume);
        bus.setVolume(volume);
    }   
}