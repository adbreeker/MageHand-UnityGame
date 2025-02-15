using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabirynthTraps : MonoBehaviour
{
    [SerializeField] Transform _leftTunnel, _rightTunnel;
    [SerializeField] Transform _cage1, _cage2, _cage3;

    [SerializeField] OpenWallPassage _wallLeft, _wallRight;
    [SerializeField] OpenBarsPassage _bars1, _bars2, _bars3;

    bool _onLeftTunnelTrigger = false;
    bool _onRightTunnelTrigger = false;
    bool _onCage1Trigger = false;
    bool _onCage2Trigger = false;
    bool _onCage3Trigger = false;

    private void Update()
    {
        CheckRightTunnel();
        CheckLeftTunnel();
        CheckCage1();
        CheckCage2();
        CheckCage3();
    }

    void CheckLeftTunnel()
    {
        if(PlayerParams.Controllers.playerMovement.currentTile == _leftTunnel && !_onLeftTunnelTrigger)
        {
            _onLeftTunnelTrigger = true;
            if(_wallLeft.passageOpen)
            {
                _wallLeft.Interaction();
            }
        }

        if(PlayerParams.Controllers.playerMovement.currentTile != _leftTunnel && _onLeftTunnelTrigger)
        {
            _onLeftTunnelTrigger = false;
        }
    }

    void CheckRightTunnel()
    {
        if (PlayerParams.Controllers.playerMovement.currentTile == _rightTunnel && !_onRightTunnelTrigger)
        {
            _onRightTunnelTrigger = true;
            if (_wallRight.passageOpen)
            {
                _wallRight.Interaction();
            }
        }

        if (PlayerParams.Controllers.playerMovement.currentTile != _rightTunnel && _onRightTunnelTrigger)
        {
            _onRightTunnelTrigger = false;
        }
    }

    void CheckCage1()
    {
        if (PlayerParams.Controllers.playerMovement.currentTile == _cage1 && !_onCage1Trigger)
        {
            _onCage1Trigger = true;
            if (_bars1.passageOpen)
            {
                _bars1.Interaction();
            }
        }

        if (PlayerParams.Controllers.playerMovement.currentTile != _cage1 && _onCage1Trigger)
        {
            _onCage1Trigger = false;
        }
    }

    void CheckCage2() 
    {
        if (PlayerParams.Controllers.playerMovement.currentTile == _cage2 && !_onCage2Trigger)
        {
            _onCage2Trigger = true;
            if (_bars2.passageOpen)
            {
                _bars2.Interaction();
            }
        }

        if (PlayerParams.Controllers.playerMovement.currentTile != _cage2 && _onCage2Trigger)
        {
            _onCage2Trigger = false;
        }
    }

    void CheckCage3()
    {
        if (PlayerParams.Controllers.playerMovement.currentTile == _cage3 && !_onCage3Trigger)
        {
            _onCage3Trigger = true;
            if (_bars3.passageOpen)
            {
                _bars3.Interaction();
            }
        }

        if (PlayerParams.Controllers.playerMovement.currentTile != _cage3 && _onCage3Trigger)
        {
            _onCage3Trigger = false;
        }
    }
}
