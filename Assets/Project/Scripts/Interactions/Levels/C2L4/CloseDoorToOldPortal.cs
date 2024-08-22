using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDoorToOldPortal : MonoBehaviour
{
    [SerializeField] BoxCollider _closeDoorTrigger;
    [SerializeField] OpenLockedDoorsPassage _door;

    private void Update()
    {
        if (_closeDoorTrigger.bounds.Contains(PlayerParams.Objects.player.transform.position))
        {
            _door.Interaction();
            Destroy(this);
        }
    }
}
