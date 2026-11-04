using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dance9x9RoomPuzzle : MonoBehaviour
{
    [SerializeField] List<OpenWallPassage> _wallsToOpen;
    [SerializeField] List<Transform> _danceTiles = new List<Transform>();

    [SerializeField] int _pirouetteDanceMoveIndex1;
    bool _pirouette1Done = false;
    [SerializeField] int _pirouetteDanceMoveIndex2;
    bool _pirouette2Done = false;

    int _tileIndex = 0;
    bool _isOnPath = false;

    Coroutine _pirouetteCoroutine = null;

    void Update()
    {
        Transform playerPos = PlayerParams.Controllers.playerMovement.currentTile;

        if (!_isOnPath)
        {
            if (playerPos == _danceTiles[_tileIndex])
            {
                _isOnPath = true;
            }
        }
        else
        {
            //resolving special situations - being on last tile of path
            if (_tileIndex == _danceTiles.Count - 1)
            {
                if (playerPos == _danceTiles[_tileIndex])
                {
                    foreach(OpenWallPassage owp in _wallsToOpen)
                    {
                        owp.Interaction();
                    }
                    Destroy(this);
                    return;
                }
                else
                {
                    PlayerMissedPath();
                    return;
                }
            }
            //checking if player following path
            if (playerPos != _danceTiles[_tileIndex] && playerPos != _danceTiles[_tileIndex + 1])
            {
                PlayerMissedPath();
                return;
            }
            //increasing path index if path is followed
            if (playerPos == _danceTiles[_tileIndex + 1])
            {
                _tileIndex += 1;
            }

            //resolving pirouettes
            if(_pirouetteCoroutine == null)
            {
                if(_tileIndex == _pirouetteDanceMoveIndex1 && !_pirouette1Done)
                {
                    _pirouetteCoroutine = StartCoroutine(PirouetteCoroutine());
                    _pirouette1Done = true;
                }
                if (_tileIndex == _pirouetteDanceMoveIndex2 && !_pirouette2Done)
                {
                    _pirouetteCoroutine = StartCoroutine(PirouetteCoroutine());
                    _pirouette2Done = true;
                }
            }
        }
    }

    IEnumerator PirouetteCoroutine()
    {
        GameObject player = PlayerParams.Objects.player;
        Vector3 startPos = player.transform.position;
        float accumulatedRotation = 0f;
        float lastCheckRotation = 0f;

        while (true) 
        {
            yield return null;
            if(player.transform.position != startPos)
            {
                PlayerMissedPath();
                break;
            }

            float currentFrameRotation = player.transform.eulerAngles.y;

            float deltaRotation = currentFrameRotation - lastCheckRotation;

            // Handle wrapping at 360 degrees
            if (deltaRotation < -180f)
            {
                deltaRotation += 360f;
            }
            else if (deltaRotation > 180f)
            {
                deltaRotation -= 360f;
            }

            accumulatedRotation += deltaRotation;

            lastCheckRotation = currentFrameRotation;

            if (accumulatedRotation >= 359f || accumulatedRotation <= -359f)
            {
                break;
            }
        }
        _pirouetteCoroutine = null;
    }

    void PlayerMissedPath()
    {
        _isOnPath = false;
        _tileIndex = 0;

        _pirouette1Done = false;
        _pirouette2Done = false;

        if (_pirouetteCoroutine != null) 
        {
            StopCoroutine(_pirouetteCoroutine);
            _pirouetteCoroutine = null;
        }
    }
}
