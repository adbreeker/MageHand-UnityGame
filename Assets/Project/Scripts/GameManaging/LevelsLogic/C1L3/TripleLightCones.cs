using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleLightCones : MonoBehaviour
{
    [SerializeField] Transform[] _rowsOfTilesLeft = new Transform[11];
    [SerializeField] Transform[] _rowsOfTilesMiddle = new Transform[11];
    [SerializeField] Transform[] _rowsOfTilesRight = new Transform[11];

    [Header("Path:")]
    [SerializeField] Transform[] _path = new Transform[11];
    [SerializeField] int _currentRow = 0;

    [SerializeField] GameObject _lightCone;

    [SerializeField] Vector3 _failTpDestination;

    [SerializeField] Transform _triggerTile1, _triggerTile2;


    bool IsPlayerOnTile(Transform position)
    {
        if (position == PlayerParams.Controllers.playerMovement.currentTile)
        {
            return true;
        }
        return false;
    }

    bool IsPlayerOnRow(int row)
    {
        if (IsPlayerOnTile(_rowsOfTilesLeft[row]) || IsPlayerOnTile(_rowsOfTilesMiddle[row]) || IsPlayerOnTile(_rowsOfTilesRight[row]))
        {
            return true;
        }
        return false;
    }

    void ManageLightCone(int onRow)
    {
        if(onRow < 11)
        {
            _lightCone.transform.parent = _path[onRow];
            _lightCone.transform.localPosition = new Vector3(0, 3.5f, 0);
            _lightCone.SetActive(true);
        }
        else
        {
            _lightCone.SetActive(false);
        }
    }

    private void Update()
    {
        //special tiles
        if(IsPlayerOnTile(_triggerTile1))
        {
            _lightCone.SetActive(false);
            _currentRow = 6;
        }
        if(IsPlayerOnTile(_triggerTile2))
        {
            _lightCone.SetActive(false);
            _currentRow = 10;
        }
        if (PlayerParams.Controllers.playerMovement.currentOnTilePos.x <= 20)
        {
            _lightCone.SetActive(false);
            _currentRow = 0;
        }

        //managing light cone and path following
        if(PlayerParams.Controllers.spellCasting.currentSpell == "Light")
        {
            if(IsPlayerOnRow(_currentRow))
            {
                ManageLightCone(_currentRow);
            }
        }
        else
        {
            _lightCone.SetActive(false);
        }

        if(_currentRow < 10)
        {
            if (IsPlayerOnRow(_currentRow + 1))
            {
                if (IsPlayerOnTile(_path[_currentRow]))
                {
                    _currentRow += 1;
                }
                else
                {
                    PlayerParams.Controllers.playerMovement.TeleportTo(_failTpDestination, 90f, null);
                    _currentRow = 0;
                    _lightCone.SetActive(false);
                }
            }
        }

        if (_currentRow > 0)
        {
            if (IsPlayerOnRow(_currentRow - 1))
            {
                _currentRow -= 1;
            }
        }
    }
}
