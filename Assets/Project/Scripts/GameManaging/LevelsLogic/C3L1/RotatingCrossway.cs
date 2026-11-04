using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RotatingCrossway : MonoBehaviour
{
    [SerializeField] Transform _rotatingHallway;
    [SerializeField] Transform _rotatingCrossway;

    bool _rotated = false;

    private void Update()
    {
        if (PlayerParams.Controllers.playerMovement.currentTile == _rotatingHallway && !_rotated)
        {
            PlayerParams.Objects.player.transform.rotation *= Quaternion.Euler(0, 180, 0);
            _rotated = true;
        }
        if (PlayerParams.Controllers.playerMovement.currentTile == _rotatingCrossway && !_rotated)
        {
            PlayerParams.Objects.player.transform.rotation *= Quaternion.Euler(0, 90*Random.Range(1,4), 0);
            _rotated = true;
        }
        if (_rotated && (PlayerParams.Controllers.playerMovement.currentTile != _rotatingHallway && PlayerParams.Controllers.playerMovement.currentTile != _rotatingCrossway))
        {
            _rotated = false;
        }
    }
}
