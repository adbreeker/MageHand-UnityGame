using FMODUnity;
using UnityEngine;

public class PushPlayerEffect : MonoBehaviour
{
    PlayerMovement _pm = null;
    Vector3 _destination;
    float _pushForce;
    bool _isPushed = false;

    public void Initialize(Vector3 destination, float pushForce)
    {
        _pm = PlayerParams.Controllers.playerMovement;
        _pm.stopMovement = true;
        _pm.StopCurrentMovement();

        _destination = destination;
        _pushForce = pushForce;
    }

    void LateUpdate()
    {
        if( _pm != null ) 
        {
            if (!_isPushed && CanMove())
            {
                _isPushed = true;
            }

            if (_isPushed)
            {
                transform.position = Vector3.MoveTowards(transform.position, _destination, _pushForce * Time.unscaledDeltaTime * (PlayerParams.Controllers.pauseMenu.freezeTime ? 0 : 1));

                if (transform.position == _destination)
                {
                    _pm.currentTile = _pm.RaycastCurrentTile();
                    _pm.currentOnTilePos = transform.position;

                    _pm.stopMovement = false;
                    Destroy(this);
                }
                else if (!CanMove())
                {
                    _destination = _pm.currentOnTilePos; //back to previous tile if collide with something
                }
            }
        }
    }

    bool CanMove()
    {
        if (_pm.ghostmodeActive) //if ghostmode is active then allways can move
        {
            return true;
        }

        //get obstacles near player
        Collider[] colliders = Physics.OverlapSphere(new Vector3(transform.position.x, 1.25f, transform.position.z), 0.8f, _pm.obstacleMask, QueryTriggerInteraction.Ignore);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "Wall" || collider.gameObject.tag == "Obstacle")
            {
                //if obstacle near player then can't move
                RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.SFX_PlayerCollision);
                //Destroy(collisionSound.gameObject, collisionSound.clip.length);

                return false;
            }
        }
        return true;
    }
}
