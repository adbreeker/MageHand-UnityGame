using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDetecting : MonoBehaviour
{
    [Header("Item detecting:")]
    public bool isItemInBoundaries = false;
    public GameObject itemToDetect;

    [Header("Boundaries")]
    [SerializeField] BoxCollider _boundaries;

    private void Update()
    {
        Collider[] colliders = Physics.OverlapBox(
            _boundaries.bounds.center,
            _boundaries.bounds.extents,
            _boundaries.transform.rotation,
            LayerMask.GetMask("Item"));

        bool itemState = false;
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject == itemToDetect && PlayerParams.Controllers.handInteractions.inHand != collider.gameObject)
            {
                itemState = true;
                break;
            }
        }
        isItemInBoundaries = itemState;
    }
}
