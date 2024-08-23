using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public EventReference musicRef;
    private EventInstance eventInstance;

    void Start()
    {
        eventInstance = RuntimeManager.CreateInstance(musicRef);
        eventInstance.start();
        eventInstance.release();
    }

    private void OnDestroy()
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
