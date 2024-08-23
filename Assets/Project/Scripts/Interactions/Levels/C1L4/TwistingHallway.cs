using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwistingHallway : MonoBehaviour
{
    [SerializeField] Transform _rotationCube;

    bool rotated = false;

    // Update is called once per frame
    void Update()
    {
        if(PlayerParams.Controllers.playerMovement.currentTile == _rotationCube && !rotated)
        {
            PlayerParams.Objects.player.transform.rotation *= Quaternion.Euler(0, 180, 0);
            rotated = true;
        }
        if(rotated && PlayerParams.Controllers.playerMovement.currentTile != _rotationCube)
        {
            rotated = false;
        }
    }
}
