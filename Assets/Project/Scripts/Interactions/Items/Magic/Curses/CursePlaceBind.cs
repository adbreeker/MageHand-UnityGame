using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursePlaceBind : MonoBehaviour
{
    Vector3 _startingPos;
    Quaternion _startingRot;

    [SerializeField] Collider _bindBounds;

    Coroutine _afterThrow;

    private void Start()
    {
        _startingPos = transform.position;
        _startingRot = transform.rotation;
        PlayerParams.Controllers.playerMovement.StartCoroutine(BackToLocationPlayerMovement());
    }

    void OnThrow()
    {
        _afterThrow = StartCoroutine(BackToLocationAfterThrow());
    }

    void OnPickUp()
    {
        if(_afterThrow != null)
        {
            StopCoroutine(_afterThrow);
            _afterThrow = null;
        }
    }

    IEnumerator BackToLocationAfterThrow()
    {
        yield return new WaitForSeconds(4.0f);
        TeleportBackToLocation();
    }

    IEnumerator BackToLocationPlayerMovement()
    {
        while(true) 
        {
            yield return new WaitForFixedUpdate();
            if (transform.position != _startingPos && !_bindBounds.bounds.Contains(PlayerParams.Objects.player.transform.position))
            {
                TeleportBackToLocation();
            }
        }
    }

    void ChangeLayer(GameObject obj, LayerMask layer)
    {
        if (obj != null)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                ChangeLayer(child.gameObject, layer);
            }
        }
    }

    void TeleportBackToLocation()
    {
        transform.SetParent(null);
        if (GetComponent<Rigidbody>() != null)
        {
            Destroy(GetComponent<Rigidbody>());
        }

        if(PlayerParams.Controllers.handInteractions.inHand == gameObject) { PlayerParams.Controllers.handInteractions.inHand = null; }

        GameObject tpEffect = GameParams.Holders.materialsAndEffectsHolder.GetEffect(MaterialsAndEffectsHolder.Effects.teleportationObject);
        Instantiate(tpEffect, transform.position, Quaternion.identity);

        ChangeLayer(gameObject, LayerMask.NameToLayer("Item"));

        GetComponent<ItemBehavior>().TeleportTo(_startingPos, 0f, null);
        transform.rotation = _startingRot;
    }
}
