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
        if(!_wolf1Activated && PlayerParams.Controllers.playerMovement.currentTile == _triggerTile1)
        {
            _wolf1Activated=true;
            _wolf1Controller.SetWolfMovement(_wolf1Path, true, true);
        }

        if (!_wolf2Activated && PlayerParams.Controllers.playerMovement.currentTile == _triggerTile2)
        {
            _wolf2Activated = true;
            _wolf2Controller.SetWolfMovement(_wolf2Path, true, true);
        }
    }
}
