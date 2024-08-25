using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWallPassage : MonoBehaviour
{
    [Header("Wall")]
    public GameObject wall;

    [Header("Rotating arround point")]
    public Transform pivot;

    [Header("Rotatrion multiplier")]
    public float rotationMultiplier = 1.0f;

    bool wallMoving = false;

    private EventInstance wallSound;

    public void Interaction() //on iteraction start rotating
    {
        if(!wallMoving)
        {
            wallMoving = true;
            StartCoroutine(RotateDoors());
        }
    }

    IEnumerator RotateDoors() //walls rotation animation
    {
        wallSound = GameParams.Managers.audioManager.PlayOneShotOccluded(GameParams.Managers.fmodEvents.SFX_MovingWall, wall.transform);
        for (int i = 0; i < 90; i++)
        {
            yield return new WaitForFixedUpdate();
            wall.transform.RotateAround(pivot.position ,new Vector3(0, 1, 0), 1.0f * rotationMultiplier);
        }
        wallSound.stop(STOP_MODE.IMMEDIATE);
    }
}
