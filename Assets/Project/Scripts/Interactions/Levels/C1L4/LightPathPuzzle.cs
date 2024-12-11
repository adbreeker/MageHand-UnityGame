using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPathPuzzle : MonoBehaviour
{
    [SerializeField] Transform _enterRoomPos;
    [SerializeField] Transform _exitRoomPos;
    [SerializeField] List<Transform> _lightPath = new List<Transform>();
    
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

    public void LeadLightThroughPath(LightSpellBehavior light)
    {
        List<Transform> path = new List<Transform>();
        Debug.Log(_isOnPath + " " + _tileIndex);

        if (!_isOnPath)
        {
            if(PlayerParams.Controllers.playerMovement.currentTile == _enterRoomPos)
            {
                path.Add(_lightPath[_tileIndex]);
            }
            else
            {
                path.Add(_lightPath[_lightPath.Count - 1]);
                light.StartCoroutine(LeadLightCoroutine(light, path));
                return;
            }
        }

        for(int i = _tileIndex + 1; i< _lightPath.Count; i++)
        {
            path.Add(_lightPath[i]);
        }
        light.StartCoroutine(LeadLightCoroutine(light, path));
    }

    IEnumerator LeadLightCoroutine(LightSpellBehavior light, List<Transform> path)
    {
        Rigidbody rb = light.GetComponent<Rigidbody>();
        float speed = (rb.velocity.magnitude * Time.fixedDeltaTime) * 0.7f;
        Destroy(rb);

        foreach(Transform tile in path)
        {
            Vector3 destination = tile.position;
            destination.y = light.transform.position.y;
            while(light.transform.position != destination)
            {
                yield return new WaitForFixedUpdate();
                light.transform.position = Vector3.MoveTowards(light.transform.position, destination, speed);
            }
        }

        light.OnImpact(null);
    }
}
