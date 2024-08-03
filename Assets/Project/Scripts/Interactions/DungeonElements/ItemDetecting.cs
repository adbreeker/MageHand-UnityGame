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

    private void Start()
    {
        Collider[] colliders = Physics.OverlapBox(_boundaries.bounds.center, _boundaries.bounds.extents, _boundaries.transform.rotation);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject == itemToDetect)
            {
                isItemInBoundaries = true;
                break;
            }
            isItemInBoundaries = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == itemToDetect && PlayerParams.Controllers.handInteractions.inHand != itemToDetect) 
        {
            isItemInBoundaries=true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == itemToDetect && PlayerParams.Controllers.handInteractions.inHand != itemToDetect)
        {
            isItemInBoundaries = true;
        }
        if (other.gameObject == itemToDetect && PlayerParams.Controllers.handInteractions.inHand == itemToDetect)
        {
            isItemInBoundaries = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == itemToDetect) 
        {
            isItemInBoundaries=false;
        }
    }
}
