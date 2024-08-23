using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FmodBuses
{
    public static Bus Master => RuntimeManager.GetBus("bus:/");
    public static Bus Music => RuntimeManager.GetBus("bus:/Music");
    public static Bus UI => RuntimeManager.GetBus("bus:/UI");
    public static Bus SFX => RuntimeManager.GetBus("bus:/SFX");
}
