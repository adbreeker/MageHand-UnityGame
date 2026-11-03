using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeItemForKey : MonoBehaviour
{
    [SerializeField] BoxCollider _boundaries;
    [SerializeField] GameObject _key;

    Coroutine _tradeCoroutine = null;
    GameObject _tradingItem;

    private void Update()
    {
        Collider[] colliders = Physics.OverlapBox(
            _boundaries.bounds.center, 
            _boundaries.bounds.extents, 
            _boundaries.transform.rotation, 
            LayerMask.GetMask("Item"));

        if(_tradeCoroutine == null)
        {
            foreach(Collider collider in colliders) 
            {
                if(PlayerParams.Controllers.handInteractions.inHand != collider.gameObject)
                {
                    _tradeCoroutine = StartCoroutine(TradeCoroutine());
                    _tradingItem = collider.gameObject;
                    break;
                }
            }
        }
        else
        {
            bool itemStillHere = false;
            foreach (Collider collider in colliders)
            {
                if (PlayerParams.Controllers.handInteractions.inHand != collider.gameObject
                    && collider.gameObject == _tradingItem)
                {
                    itemStillHere = true; 
                    break;
                }
            }

            if(!itemStillHere)
            {
                StopCoroutine(_tradeCoroutine);
                _tradeCoroutine = null;
                _tradingItem = null;
            }
        }
    }

    IEnumerator TradeCoroutine()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(_tradingItem);
        _key.SetActive(true);
        _key.GetComponent<ItemBehavior>().TeleportTo(_key.transform.position, _key.transform.rotation.eulerAngles.y, null);
        this.enabled = false;
    }
}
