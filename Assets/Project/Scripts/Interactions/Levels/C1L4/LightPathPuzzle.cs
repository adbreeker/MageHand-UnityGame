using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPathPuzzle : MonoBehaviour
{
    [SerializeField] Transform _enterRoomPos;
    [SerializeField] Transform _exitRoomPos;
    [SerializeField] List<Transform> _lightPath = new List<Transform>();

    [SerializeField] GameObject _teleportationEffectPrefab;
    
    int _tileIndex = 0;
    bool _isOnPath = false;

    void Update()
    {
        Transform playerPos = PlayerParams.Controllers.playerMovement.currentTile;

        if (!_isOnPath)
        {
            if ( playerPos == _lightPath[_tileIndex] || playerPos == _lightPath[_lightPath.Count - 1])
            {
                _isOnPath = true;
            }
        }
        else
        {
            //resolving special situations - leaving puzzle room
            if(_tileIndex == 0 && playerPos == _enterRoomPos)
            {
                _isOnPath = false;
                _tileIndex = 0;
                return;
            }
            //resolving special situations - being on last tile of path
            if (_tileIndex == _lightPath.Count - 1)
            {
                if(playerPos == _exitRoomPos)
                {
                    _isOnPath = false;
                    _tileIndex = 0;
                    return;
                }
                if(playerPos != _lightPath[_lightPath.Count - 1])
                {
                    PlayerMissedPath();
                    return;
                }
            }
            else
            {
                //checking if player following path
                if (playerPos != _lightPath[_tileIndex] && playerPos != _lightPath[_tileIndex + 1])
                {
                    PlayerMissedPath();
                    return;
                }
                //increasing path index if path is followed
                if (playerPos == _lightPath[_tileIndex + 1])
                {
                    _tileIndex += 1;
                    return;
                }
            }
        }
    }

    void PlayerMissedPath()
    {
        _isOnPath = false;
        _tileIndex = 0;

        Vector3 tpDest = _enterRoomPos.position;
        tpDest.y = PlayerParams.Objects.player.transform.position.y;
        PlayerParams.Controllers.playerMovement.TeleportTo(tpDest, 180f, null);
    }
}
