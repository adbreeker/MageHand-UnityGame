using UnityEngine;

public class DualHallwaysSwapping : MonoBehaviour
{
    [Header("Switch Hallways")]
    [SerializeField] Transform _rightHallwayCheck;
    [SerializeField] Transform _rightHallwayTrigger;
    [SerializeField] Transform _leftHallwayCheck;
    [SerializeField] Transform _leftHallwayTrigger;

    Transform _prevTile = null;

    public void Update()
    {
        if (PlayerParams.Controllers.playerMovement.currentTile == _rightHallwayTrigger && _prevTile == _rightHallwayCheck)
        {
            Vector3 tpDest = _leftHallwayTrigger.position;
            tpDest.y = PlayerParams.Objects.player.transform.position.y;
            PlayerParams.Objects.player.transform.position = tpDest;
        }
        if (PlayerParams.Controllers.playerMovement.currentTile == _leftHallwayTrigger && _prevTile == _leftHallwayCheck)
        {
            Vector3 tpDest = _rightHallwayTrigger.position;
            tpDest.y = PlayerParams.Objects.player.transform.position.y;
            PlayerParams.Objects.player.transform.position = tpDest;
        }
        _prevTile = PlayerParams.Controllers.playerMovement.currentTile;
    }
}
