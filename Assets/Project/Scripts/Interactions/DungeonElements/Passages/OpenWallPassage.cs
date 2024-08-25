using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWallPassage : MonoBehaviour
{
    [Header("Is passage open")]
    public bool passageOpen = false;

    [Header("Wall")]
    public GameObject wall;

    [Header("Opening speed")]
    public float wallSpeed = 0.03f;

    private EventInstance wallSound;

    public void Interaction() //open or close passage on interaction
    {
        StopAllCoroutines();
        if(wallSound.isValid()) wallSound.stop(STOP_MODE.IMMEDIATE);

        if (passageOpen) //if passage open then close
        {
            passageOpen = false;
            StartCoroutine(MoveWall(0.0f));
        }
        else // else open passage
        {
            passageOpen = true;
            StartCoroutine(MoveWall(-5f));    
        }

    }

    IEnumerator MoveWall(float wallDestination) //animating passage opening
    {
        wallSound = GameParams.Managers.audioManager.PlayOneShotOccluded(GameParams.Managers.fmodEvents.SFX_MovingWall, wall.transform);
        while (wall.transform.localPosition.y != wallDestination)
        {
            yield return new WaitForFixedUpdate();
            wall.transform.localPosition = Vector3.MoveTowards(wall.transform.localPosition, new Vector3(wall.transform.localPosition.x, wallDestination, wall.transform.localPosition.z), wallSpeed);
        }
        wallSound.stop(STOP_MODE.IMMEDIATE);
    }

    private void OnValidate()
    {
        if(wall==null) { wall = gameObject; }

        if (passageOpen)
        {
            wall.transform.localPosition = new Vector3(wall.transform.localPosition.x, -5f, wall.transform.localPosition.z);
        }
        else
        {
            wall.transform.localPosition = new Vector3(wall.transform.localPosition.x, 0, wall.transform.localPosition.z);
        }
    }
}
