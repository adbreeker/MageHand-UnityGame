using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfEncounter : MonoBehaviour
{
    [Header("Wolf Controlling:")]
    [SerializeField] CaveWolfController _wolf;
    [SerializeField] List<Transform> _runningInPath;
    [SerializeField] float waitingTime;
    [SerializeField] List<Transform> _walkingOutPath;
    [SerializeField] List<Transform> _runningOutBlinded;

    [Header("Triggers")]
    [SerializeField] Transform _wolfEnterTrigger;
    [SerializeField] BoxCollider _closeWallTrigger;
    [SerializeField] BoxCollider _closeWallSecretTrigger;
    [SerializeField] Transform _afterTeleportTrigger;
    bool _wolfTriggered = false;
    bool _wallTriggered = false;
    bool _wolfIsBlinded = false;
    bool _wolfWalkingPeacefully = false;

    [Header("Wall to close for wolf")]
    [SerializeField] OpenWallPassage _wall;
    [SerializeField] OpenWallPassage _wallSecret;

    private void Start()
    {
        _wolf.OnWolfHitted += WolfHittedInteraction;
        PlayerParams.Controllers.pointsManager.maxPlotPoints += 4;
        PlayerParams.Controllers.pointsManager.minPlotPoints -= 4;
    }

    void Update()
    {
        if(!_wolfTriggered
            && PlayerParams.Controllers.playerMovement.currentTilePos.x == _wolfEnterTrigger.position.x 
            && PlayerParams.Controllers.playerMovement.currentTilePos.z == _wolfEnterTrigger.position.z)
        {
            _wolfTriggered = true;
            _wolf.SetWolfMovement(_runningInPath);
            StartCoroutine(WaitingForWolf());
        }

        if (_wolf != null && !_wallTriggered && _closeWallTrigger.bounds.Contains(_wolf.transform.position))
        {
            _wallTriggered = true;
            _wall.Interaction();
        }

        if (_wolf != null && !_wallTriggered && _closeWallSecretTrigger.bounds.Contains(_wolf.transform.position))
        {
            _wallTriggered = true;
            _wallSecret.Interaction();
        }

        if (PlayerParams.Controllers.playerMovement.currentTilePos.x == _afterTeleportTrigger.position.x
            && PlayerParams.Controllers.playerMovement.currentTilePos.z == _afterTeleportTrigger.position.z)
        {
            if(_wolf == null) { PlayerParams.Controllers.pointsManager.plotPoints -= 4; Debug.Log("-4pkt"); }
            else if(_wolfWalkingPeacefully) { PlayerParams.Controllers.pointsManager.plotPoints += 4; Debug.Log("4pkt"); }
            else if(_wolfIsBlinded) { PlayerParams.Controllers.pointsManager.plotPoints -= 2; Debug.Log("-2pkt"); }
            Destroy(this);
        }
    }

    IEnumerator WaitingForWolf()
    {
        yield return new WaitForSeconds(waitingTime);
        if(_wolf != null && !_wolfIsBlinded)
        {
            _wolfWalkingPeacefully = true;
            _wolf.SetWolfMovement(_walkingOutPath, 3.0f, 180f);
        }
    }

    void WolfHittedInteraction(GameObject hittedBy)
    {
        if (hittedBy.layer == LayerMask.NameToLayer("Item") || hittedBy.layer == LayerMask.NameToLayer("Spell"))
        {
            if (_wolfWalkingPeacefully)
            {
                _wolf.movementSpeed = 8.0f;
                _wolf.rotationSpeed = 330.0f;
            }
            else
            {
                if (hittedBy.GetComponent<LightSpellBehavior>() != null)
                {
                    _wolfIsBlinded = true;
                    _wolf.SetWolfMovement(_runningOutBlinded, 6.0f, 300.0f);
                }
            }
        }
    }
}
