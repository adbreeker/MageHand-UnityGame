using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dance9x9RoomSecret : MonoBehaviour
{
    [SerializeField] OpenWallPassage _wallToOpen;
    [SerializeField] List<Transform> _danceTiles = new List<Transform>();

    [SerializeField] int[] _pirouetteDanceMoveIndexes = new int[5];
    bool[] _pirouettesDone = { false, false, false, false, false };

    int _tileIndex = 0;
    bool _isOnPath = false;

    Coroutine _pirouetteCoroutine = null;

    void Update()
    {
        Vector3 playerPos = PlayerParams.Controllers.playerMovement.currentTilePos;

        if (!_isOnPath)
        {
            if (playerPos == TileToPlayerPos(_danceTiles[_tileIndex].position))
            {
                _isOnPath = true;
            }
        }
        else
        {
            //resolving special situations - being on last tile of path
            if (_tileIndex == _danceTiles.Count - 1)
            {
                if (playerPos == TileToPlayerPos(_danceTiles[_tileIndex].position))
                {
                    if (_pirouetteCoroutine == null)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (_tileIndex == _pirouetteDanceMoveIndexes[i] && !_pirouettesDone[i])
                            {
                                _pirouetteCoroutine = StartCoroutine(PirouetteCoroutine(i));
                                _pirouettesDone[i] = true;
                            }
                        }
                    }
                    return;
                }
                else
                {
                    PlayerMissedPath();
                    return;
                }
            }
            //checking if player following path
            if (playerPos != TileToPlayerPos(_danceTiles[_tileIndex].position) && playerPos != TileToPlayerPos(_danceTiles[_tileIndex + 1].position))
            {
                PlayerMissedPath();
                return;
            }
            //increasing path index if path is followed
            if (playerPos == TileToPlayerPos(_danceTiles[_tileIndex + 1].position))
            {
                _tileIndex += 1;
            }

            //resolving pirouettes
            if(_pirouetteCoroutine == null)
            {
                for (int i = 0; i < 5; i++)
                {
                    if(_tileIndex == _pirouetteDanceMoveIndexes[i] && !_pirouettesDone[i])
                    {
                        _pirouetteCoroutine = StartCoroutine(PirouetteCoroutine(i));
                        _pirouettesDone[i] = true;
                    }
                }
            }
        }
    }

    IEnumerator PirouetteCoroutine(int whichOne)
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

        if(whichOne == _pirouetteDanceMoveIndexes.Length - 1)
        {
            _wallToOpen.Interaction();
            Destroy(this);
        }
        _pirouetteCoroutine = null;
    }

    void PlayerMissedPath()
    {
        _isOnPath = false;
        _tileIndex = 0;

        _pirouettesDone = new bool[] { false, false, false, false, false};

        if (_pirouetteCoroutine != null) 
        {
            StopCoroutine(_pirouetteCoroutine);
            _pirouetteCoroutine = null;
        }
    }

    Vector3 TileToPlayerPos(Vector3 tilePos)
    {
        Vector3 playerPos = tilePos;
        playerPos.y = PlayerParams.Objects.player.transform.position.y;
        return playerPos;
    }
}
