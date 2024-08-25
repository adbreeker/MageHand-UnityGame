using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreePillarsPuzzle : MonoBehaviour
{
    [SerializeField] OpenWallPassage _wallToOpen;
    [SerializeField] List<Transform> _pillarsPuzzlePath = new List<Transform>();

    [SerializeField] Transform _firstSoundPlace;
    [SerializeField] Transform _secondSoundPlace;
    [SerializeField] Transform _thirdSoundPlace;

    int _tileIndex = 0;
    bool _isOnPath = false;
    bool _soundMade = false;

    void Update()
    {
        Transform playerPos = PlayerParams.Controllers.playerMovement.currentTile;

        if (!_isOnPath)
        {
            if (playerPos == _pillarsPuzzlePath[_tileIndex])
            {
                _isOnPath = true;
            }
        }
        else
        {
            //resolving special situations - being on last tile of path
            if (_tileIndex == _pillarsPuzzlePath.Count - 1)
            {
                if(playerPos == _pillarsPuzzlePath[_tileIndex])
                {
                    _wallToOpen.Interaction();
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
            if (playerPos != _pillarsPuzzlePath[_tileIndex] && playerPos != _pillarsPuzzlePath[_tileIndex + 1])
            {
                PlayerMissedPath();
                return;
            }
            //increasing path index if path is followed
            if (playerPos == _pillarsPuzzlePath[_tileIndex + 1])
            {
                _tileIndex += 1;
            }

            //making sound when on specific tiles while following path
            if (playerPos == _firstSoundPlace
                || playerPos == _secondSoundPlace
                || playerPos == _thirdSoundPlace)
            {
                if (!_soundMade)
                {
                    _soundMade = true;
                    GameParams.Managers.audioManager.PlayOneShotSpatialized(GameParams.Managers.fmodEvents.SFX_ButtonPress, _wallToOpen.transform);
                }
            }
            else { _soundMade = false; }
        }
    }

    void PlayerMissedPath()
    {
        _isOnPath = false;
        _tileIndex = 0;
    }
}
