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
        Vector3 playerPos = PlayerParams.Controllers.playerMovement.currentTilePos;

        if (!_isOnPath)
        {
            if ( playerPos == TileToPlayerPos(_lightPath[_tileIndex].position) || playerPos == TileToPlayerPos(_lightPath[_lightPath.Count-1].position))
            {
                _isOnPath = true;
            }
        }
        else
        {
            //resolving special situations - leaving puzzle room
            if(_tileIndex == 0 && playerPos == TileToPlayerPos(_enterRoomPos.position))
            {
                _isOnPath = false;
                _tileIndex = 0;
                return;
            }
            //resolving special situations - being on last tile of path
            if (_tileIndex == _lightPath.Count - 1)
            {
                if(playerPos == TileToPlayerPos(_exitRoomPos.position))
                {
                    _isOnPath = false;
                    _tileIndex = 0;
                    return;
                }
                if(playerPos != TileToPlayerPos(_lightPath[_lightPath.Count - 1].position))
                {
                    PlayerMissedPath();
                    return;
                }
            }
            else
            {
                //checking if player following path
                if (playerPos != TileToPlayerPos(_lightPath[_tileIndex].position) && playerPos != TileToPlayerPos(_lightPath[_tileIndex + 1].position))
                {
                    PlayerMissedPath();
                    return;
                }
                //increasing path index if path is followed
                if (playerPos == TileToPlayerPos(_lightPath[_tileIndex + 1].position))
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
        PlayerParams.Objects.player.transform.rotation = Quaternion.Euler(0, 0, 0);
        PlayerParams.Controllers.playerMovement.TeleportTo(TileToPlayerPos(_enterRoomPos.position));
        Instantiate(_teleportationEffectPrefab, PlayerParams.Objects.player.transform);
    }

    Vector3 TileToPlayerPos(Vector3 tilePos)
    {
        Vector3 playerPos = tilePos;
        playerPos.y =PlayerParams.Objects.player.transform.position.y;
        return playerPos;
    }

}
