using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurpriseRunningWolves : MonoBehaviour
{
    [Header("Wolf spirale:")]
    [SerializeField] CaveWolfController _wolf1Controller;
    [SerializeField] List<Transform> _wolf1Path;
    [SerializeField] Transform _triggerTile1;

    [Header("Wolf room 9x9 connector:")]
    [SerializeField] CaveWolfController _wolf2Controller;
    [SerializeField] List<Transform> _wolf2Path;
    [SerializeField] Transform _triggerTile2;

    bool _wolf1Activated = false;
    bool _wolf2Activated = false;

    private void Update()
    {
        if(!_wolf1Activated && PlayerParams.Objects.player.transform.position == TileToPlayerPos(_triggerTile1.position))
        {
            _wolf1Activated=true;
            _wolf1Controller.SetWolfMovement(_wolf1Path, true, true);
        }

        if (!_wolf2Activated && PlayerParams.Objects.player.transform.position == TileToPlayerPos(_triggerTile2.position))
        {
            _wolf2Activated = true;
            _wolf2Controller.SetWolfMovement(_wolf2Path, true, true);
        }
    }

    Vector3 TileToPlayerPos(Vector3 tilePos)
    {
        Vector3 playerPos = tilePos;
        playerPos.y = PlayerParams.Objects.player.transform.position.y;
        return playerPos;
    }
}
