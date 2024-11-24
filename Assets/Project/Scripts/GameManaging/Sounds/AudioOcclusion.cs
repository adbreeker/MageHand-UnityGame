using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOcclusion : MonoBehaviour
{
    [HideInInspector] public EventInstance audioEvent;
    public EventReference audioRef;
    [HideInInspector] public LayerMask occlusionLayer;
    [HideInInspector] public float audioOcclusionWidening = 1f;
    [HideInInspector] public float playerOcclusionWidening = 1f;

    private EventDescription audioDes;
    private StudioListener listener;
    private PLAYBACK_STATE pb;

    private bool audioIsVirtual;
    private float maxDistance;
    private float listenerDistance;
    private string wallTag = "Wall";
    private float lineCastCounter = 0f;
    private float lineCastHitCount = 0f;

    private void Start()
    {
        audioDes = RuntimeManager.GetEventDescription(audioRef);
        audioDes.getMinMaxDistance(out float minDistance, out maxDistance);

        listener = FindObjectOfType<StudioListener>();
    }

    private void FixedUpdate()
    {
        audioEvent.isVirtual(out audioIsVirtual);
        audioEvent.getPlaybackState(out pb);
        listenerDistance = Vector3.Distance(transform.position, listener.transform.position);

        if (!audioIsVirtual && pb == PLAYBACK_STATE.PLAYING && listenerDistance <= maxDistance)
            OccludeBetween(transform.position, listener.transform.position);
    }

    private void OccludeBetween(Vector3 sound, Vector3 listener)
    {
        Vector3 soundLeft = CalculatePoint(sound, listener, audioOcclusionWidening, true);
        Vector3 soundRight = CalculatePoint(sound, listener, audioOcclusionWidening, false);

        Vector3 listenerLeft = CalculatePoint(listener, sound, playerOcclusionWidening, true);
        Vector3 listenerRight = CalculatePoint(listener, sound, playerOcclusionWidening, false);

        CastLine(soundLeft, listenerLeft);
        CastLine(soundLeft, listener);
        CastLine(soundLeft, listenerRight);

        CastLine(sound, listenerLeft);
        CastLine(sound, listener);
        CastLine(sound, listenerRight);

        CastLine(soundRight, listenerLeft);
        CastLine(soundRight, listener);
        CastLine(soundRight, listenerRight);

        SetParameter();

        lineCastCounter = 0f;
        lineCastHitCount = 0f;
    }

    private Vector3 CalculatePoint(Vector3 a, Vector3 b, float m, bool left)
    {
        float x;
        float z;
        float n = Vector3.Distance(new Vector3(a.x, 0f, a.z), new Vector3(b.x, 0f, b.z));
        float mn = (m / n);
        if (left)
        {
            x = a.x + (mn * (a.z - b.z));
            z = a.z - (mn * (a.x - b.x));
        }
        else
        {
            x = a.x - (mn * (a.z - b.z));
            z = a.z + (mn * (a.x - b.x));
        }
        return new Vector3(x, a.y, z);
    }

    private void CastLine(Vector3 start, Vector3 end)
    {
        lineCastCounter++;
        int hitWallsCount = 0;
        RaycastHit[] hits = Physics.RaycastAll(start, (end - start).normalized, Vector3.Distance(start, end), occlusionLayer);

        foreach(RaycastHit hit in hits)
        {
            if(hit.transform.CompareTag(wallTag))
            {
                hitWallsCount++;
            }
        }

        if (hitWallsCount == 1)
        {
            lineCastHitCount++;
            Debug.DrawLine(start, end, Color.yellow);
        }
        else if (hitWallsCount > 1)
        {
            lineCastHitCount += 2;
            Debug.DrawLine(start, end, Color.red);
        }
        else
        {
            Debug.DrawLine(start, end, Color.green);
        }
    }

    private void SetParameter()
    {
        //max value of occlusion is 1 and we can get it only when all lines are hitting more than 1 wall
        //max occlusion value that we can get with only 1 wall is 0.5f
        audioEvent.getParameterByName("occlusion", out float value);

        if (value > (lineCastHitCount / (lineCastCounter * 2)) + 0.002f) //+ 0.002f is correction for floating point imprecision
        {
            value = value - 0.025f;
            audioEvent.setParameterByName("occlusion", (float)value);
        }
        else if (value < (lineCastHitCount / (lineCastCounter * 2)) - 0.002f) //- 0.002f is correction for floating point imprecision
        {
            value = value + 0.025f;
            audioEvent.setParameterByName("occlusion", (float)value);
        }
    }
}
