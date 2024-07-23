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
        Vector3 playerPos = PlayerParams.Controllers.playerMovement.currentTilePos;

        if (!_isOnPath)
        {
            if (playerPos == TileToPlayerPos(_pillarsPuzzlePath[_tileIndex].position) || playerPos == TileToPlayerPos(_pillarsPuzzlePath[_pillarsPuzzlePath.Count - 1].position))
            {
                _isOnPath = true;
            }
        }
        else
        {
            //resolving special situations - being on last tile of path
            if (_tileIndex == _pillarsPuzzlePath.Count - 1)
            {
                if(playerPos == TileToPlayerPos(_pillarsPuzzlePath[_tileIndex].position))
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
            if (playerPos != TileToPlayerPos(_pillarsPuzzlePath[_tileIndex].position) && playerPos != TileToPlayerPos(_pillarsPuzzlePath[_tileIndex + 1].position))
            {
                PlayerMissedPath();
                return;
            }
            //increasing path index if path is followed
            if (playerPos == TileToPlayerPos(_pillarsPuzzlePath[_tileIndex + 1].position))
            {
                _tileIndex += 1;
            }

            //making sound when on specific tiles while following path
            if (playerPos == TileToPlayerPos(_firstSoundPlace.position)
                || playerPos == TileToPlayerPos(_secondSoundPlace.position)
                || playerPos == TileToPlayerPos(_thirdSoundPlace.position))
            {
                if (!_soundMade)
                {
                    _soundMade = true;
                    AudioSource puzzleSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_Button);
                    puzzleSound.Play();
                    Destroy(puzzleSound, puzzleSound.clip.length);
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

    Vector3 TileToPlayerPos(Vector3 tilePos)
    {
        Vector3 playerPos = tilePos;
        playerPos.y = PlayerParams.Objects.player.transform.position.y;
        return playerPos;
    }
}
