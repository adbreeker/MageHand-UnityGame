using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlteredTethered : MonoBehaviour
{
    Vector3 _startingPos;
    Quaternion _startingRot;

    [Header("Tether settings:")]
    [SerializeField] float _teleportDelay = 5.0f;
    [SerializeField] Collider _bindBounds;

    Coroutine _afterThrow;

    private void Awake()
    {
        _startingPos = transform.position;
        _startingRot = transform.rotation;
    }

    private void Start()
    {
        PlayerParams.Controllers.playerMovement.StartCoroutine(BackToLocationAfterExitingBounds());
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
        yield return new WaitForSeconds(_teleportDelay);
        TeleportBackToLocation();
    }

    IEnumerator BackToLocationAfterExitingBounds()
    {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();
        Transform playerPos = PlayerParams.Objects.player.transform;
        Inventory inventory = PlayerParams.Controllers.inventory;

        while (true) 
        {
            yield return wait;

            if (gameObject.activeSelf)
            {
                if(!_bindBounds.bounds.Contains(gameObject.transform.position))
                {
                    TeleportBackToLocation();
                    Debug.Log($"Tethered {gameObject.name} has exited its bounds and was teleported back — ACTIVE");
                }
            }
            else if (transform.position != _startingPos 
                && inventory.inventory.Contains(gameObject)
                && !_bindBounds.bounds.Contains(playerPos.position))
            {
                TeleportBackToLocation();
                Debug.Log($"Tethered {gameObject.name} has exited its bounds and was teleported back — INVENTORY");
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

    public void TeleportBackToLocation(bool tpEffectAtOrigin = true)
    {
        if (_afterThrow != null)
        {
            StopCoroutine(_afterThrow);
            _afterThrow = null;
        }

        transform.SetParent(null);
        if (GetComponent<Rigidbody>() != null)
        {
            Destroy(GetComponent<Rigidbody>());
        }

        if(PlayerParams.Controllers.handInteractions.inHand == gameObject) { PlayerParams.Controllers.handInteractions.inHand = null; }

        if(tpEffectAtOrigin)
        {
            GameObject tpEffect = GameParams.Holders.materialsAndEffectsHolder.GetEffect(MaterialsAndEffectsHolder.Effects.teleportationObject);
            Instantiate(tpEffect, transform.position, Quaternion.identity);
        }

        ChangeLayer(gameObject, LayerMask.NameToLayer("Item"));

        GetComponent<ItemBehavior>().TeleportTo(_startingPos, 0f, null);
        transform.rotation = _startingRot;
    }
}
