using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchmanRoomTraps : MonoBehaviour
{
    [Header("Cages:")]
    [SerializeField] Transform _cage1;
    [SerializeField] Transform _cage2;

    [Header("Bars:")]
    [SerializeField] OpenBarsPassage _bars1;
    [SerializeField] OpenBarsPassage _bars2;

    bool _trap1Triggered = false;
    bool _trap2Triggered = false;

    bool IsPlayerOnPosition(Transform position)
    {
        if (position.position.x == PlayerParams.Controllers.playerMovement.currentTilePos.x
            && position.position.z == PlayerParams.Controllers.playerMovement.currentTilePos.z)
        {
            return true;
        }
        return false;
    }

    void Update()
    {
        if(!_trap1Triggered) 
        {
            if(IsPlayerOnPosition(_cage1))
            {
                _trap1Triggered=true;
                _bars1.Interaction();
            }
        }

        if (!_trap2Triggered)
        {
            if(IsPlayerOnPosition(_cage2))
            {
                _trap2Triggered=true;
                _bars2.Interaction();
            }
        }
    }
}
