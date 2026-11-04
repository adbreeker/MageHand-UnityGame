using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeThrowingPortal : MonoBehaviour
{
    [SerializeField] Transform _tileToPortal1;
    [SerializeField] Transform _tileToPortal2;

    [SerializeField] Collider _colliderOfPortal1;
    [SerializeField] Collider _colliderOfPortal2;

    void Update()
    {
        if(PlayerParams.Controllers.playerMovement.currentTile == _tileToPortal1)
        {
            _colliderOfPortal1.enabled = true;
            _colliderOfPortal2.enabled = false;
        }
        if (PlayerParams.Controllers.playerMovement.currentTile == _tileToPortal2)
        {
            _colliderOfPortal1.enabled = false;
            _colliderOfPortal2.enabled = true;
        }
    }
}
