using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalPathsPuzzle : MonoBehaviour
{
    [Header("Path of Shark:")]
    [SerializeField] Transform _sharkPathEnter;
    [SerializeField] List<Transform> _sharkPath;
    [SerializeField] Transform _sharkPathExit;
    [SerializeField] float _sharkPathTpDeley;
    [SerializeField] LeverBehavior _sharkPathLever;
    [SerializeField] OpenBarsPassage _sharkPathPassage;

    [Header("Path of Tortoise:")]
    [SerializeField] Transform _tortoisePathEnter;
    [SerializeField] List<Transform> _tortoisePath;
    [SerializeField] Transform _tortoisePathExit;
    [SerializeField] float _tortoisePathMovementDeley;

    [Header("Path of Crab:")]
    [SerializeField] Transform _crabPathEnter;
    [SerializeField] List<Transform> _crabPath;
    [SerializeField] Transform _crabPathExit;


    PlayerMovement _pm; //player movement script
    Coroutine _pathCoroutine = null; // current path (shark/tortoise/crab) puzzle coroutine 

    private void Start()
    {
        _pm = PlayerParams.Controllers.playerMovement;
    }

    private void Update()
    {
        if(_pathCoroutine == null)
        {
            if(_pm.currentTile == _sharkPathEnter || _pm.currentTile == _sharkPathExit)
            {
                List<Transform> path = new(_sharkPath);
                if(_pm.currentTile == _sharkPathEnter) 
                { 
                    path.Insert(0, _sharkPathEnter); 
                    _sharkPathLever.ForceLeverUp();
                    if(_sharkPathPassage.passageOpen) { _sharkPathPassage.Interaction(); }
                }
                else 
                {
                    path.Add(_sharkPathExit);
                    path.Reverse();
                }

                _pathCoroutine = StartCoroutine(SharkPathPuzzle(path));
                return;
            }
            if(_pm.currentTile == _tortoisePathEnter || _pm.currentTile == _tortoisePathExit)
            {
                List<Transform> path = new(_tortoisePath);
                if (_pm.currentTile == _tortoisePathEnter) { path.Insert(0, _tortoisePathEnter); }
                else
                {
                    path.Add(_tortoisePathExit);
                    path.Reverse();
                }

                _pathCoroutine = StartCoroutine(TortoisePathPuzzle(path));
                return;
            }
            if(_pm.currentTile == _crabPathEnter || _pm.currentTile == _crabPathExit)
            {
                List<Transform> path = new(_crabPath);
                if (_pm.currentTile == _crabPathEnter) { path.Insert(0, _crabPathEnter); }
                else
                {
                    path.Add(_crabPathExit);
                    path.Reverse();
                }

                _pathCoroutine = StartCoroutine(CrabPathPuzzle(path));
                return;
            }
        }
    }

    IEnumerator SharkPathPuzzle(List<Transform> path) // moving fast foward - "shark which does not swim, drowns"
    {
        int currentIndex = 0;
        float elapsedTime = 0f;

        while (true)
        {
            yield return null;

            int index = path.IndexOf(_pm.currentTile);
            if (index == -1) // left puzzle
            {
                _pathCoroutine = null;
                yield break;
            }

            if (currentIndex > 0) { elapsedTime += Time.deltaTime; }

            if (index < currentIndex || elapsedTime >= _sharkPathTpDeley) // failed puzzle
            {
                TpPlayer(path);
                _pathCoroutine = null;
                yield break;
            }

            if(index > currentIndex) // progression
            {
                currentIndex = index;
                elapsedTime = 0f;
            }
        }
    }

    IEnumerator TortoisePathPuzzle(List<Transform> path) // moving slowly forward
    {
        int currentIndex = 0;
        float elapsedTime = _tortoisePathMovementDeley;

        while (true)
        {
            yield return null;

            int index = path.IndexOf(_pm.currentTile);
            if (index == -1) // left puzzle
            {
                _pathCoroutine = null;
                yield break;
            }

            elapsedTime += Time.deltaTime;

            if (index < currentIndex) // failed puzzle - index
            {
                TpPlayer(path);
                _pathCoroutine = null;
                yield break;
            }

            if (index > currentIndex) //progression
            {
                if (elapsedTime < _tortoisePathMovementDeley) // failed puzzle - too fast
                {
                    TpPlayer(path);
                    _pathCoroutine = null;
                    yield break;
                }
                currentIndex = index;
                elapsedTime = 0f;
            }
        }
    }

    IEnumerator CrabPathPuzzle(List<Transform> path) // moving only sideways
    {
        Vector3 lastPosition = _pm.transform.position;

        while (true)
        {
            yield return null;

            int index = path.IndexOf(_pm.currentTile);
            if (index == -1) // left puzzle
            {
                _pathCoroutine = null;
                yield break;
            }

            Vector3 currentPosition = _pm.transform.position;
            if(currentPosition != lastPosition // failed puzzle - moved along the look vector of the player
                && Mathf.Abs(Vector3.Dot((currentPosition - lastPosition).normalized, _pm.transform.forward)) > 0.25f)
            {
                TpPlayer(path);
                _pathCoroutine = null;
                yield break;
            }
            lastPosition = currentPosition;
        }
    }

    void TpPlayer(List<Transform> path)
    {
        if (_pm.currentTile != path[0])
        {
            // fixing position if case of tiles bigger than 1x1
            Vector3 tpPos = path[0].position;
            if (Vector3.Distance(tpPos, path[1].position) > 4.5f)
            {
                Vector3 diff = tpPos - path[1].position;

                Vector3 snapDir;
                if (Mathf.Abs(diff.x) > Mathf.Abs(diff.z)) { snapDir = new Vector3(Mathf.Sign(diff.x), 0f, 0f); }
                else { snapDir = new Vector3(0f, 0f, Mathf.Sign(diff.z)); }
                
                tpPos = path[1].position + snapDir * 4f;
            }

            // teleportation on path starting tile and facing path
            Vector3 tpDest = new Vector3(tpPos.x, _pm.transform.position.y, tpPos.z);
            float tpRot = Mathf.Round(Quaternion.LookRotation(path[1].position - tpPos).eulerAngles.y / 90f) * 90f;
            _pm.TeleportTo(tpDest, tpRot, null);
        }
    }
}
