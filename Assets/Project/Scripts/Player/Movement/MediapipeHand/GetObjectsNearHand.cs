using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetObjectsNearHand : MonoBehaviour
{
    [Header("Layer masks for objects and UI")]
    public LayerMask objectsMask;
    public LayerMask uiMask;

    [Header("Points to calculate middle point")]
    public Transform wristPoint;
    public Transform indexFingerKnucklePoint; 
    public Transform smallFingerKnucklePoint;

    [Header("Currently pointing object")]
    public GameObject currentlyPointing;


    private void Update() //check objects near middle point every update
    {
        if(PlayerParams.Controllers.handInteractions.inHand == null)
        {
            CheckSphere();
        }
    }

    void CheckSphere()
    {
        //calculate middle point
        Vector3 middlePoint = (wristPoint.position + indexFingerKnucklePoint.position + smallFingerKnucklePoint.position) / 3f;

        Collider[] colliders;
        if (PlayerParams.Variables.uiActive) //if UI active then searching on UI layer with smaller range
        {
            colliders = Physics.OverlapSphere(middlePoint, 0.2f, uiMask);
        }
        else //else searching on objects layers with bigger range
        {
            colliders = Physics.OverlapSphere(middlePoint, 0.7f, objectsMask);
        }


        if(colliders.Length > 0) //first found object becomes currently pointed
        {
            currentlyPointing = colliders[0].gameObject;
            EnlightObject(currentlyPointing);
        }
        else //if no objects then currently pointed is null
        {
            currentlyPointing = null;
        }
    }

    void EnlightObject(GameObject pointingAt) //enlightening pointed objects
    {
        if(pointingAt != PlayerParams.Controllers.handInteractions.inHand) //only if no object currently in hand
        {
            //if pointing on item, switch or UI then enlightening only this item
            if (pointingAt.layer == LayerMask.NameToLayer("Item") || pointingAt.layer == LayerMask.NameToLayer("Switch") || pointingAt.layer == LayerMask.NameToLayer("UI"))
            {
                if (pointingAt.GetComponent<EnlightItem>() != null)
                {
                    pointingAt.GetComponent<EnlightItem>().enlightenTime = 10;
                }
                else
                {
                    pointingAt.AddComponent<EnlightItem>();
                }
            }

            //if pointing on chest then enlightening all child objects
            if(pointingAt.layer == LayerMask.NameToLayer("Chest"))
            {
                foreach(Transform child in pointingAt.transform)
                {
                    if (child.gameObject.GetComponent<MeshRenderer>() != null)
                    {
                        if (child.gameObject.GetComponent<EnlightItem>() != null)
                        {
                            child.gameObject.GetComponent<EnlightItem>().enlightenTime = 10;
                        }
                        else
                        {
                            child.gameObject.AddComponent<EnlightItem>();
                        }
                    }
                }
            }
        }
    }


    //relics of old system ------------------------------------------------------------------------------------------------------ relics of old system


    private GameObject magicPointer;
    private float catchingDistance = 2.5f;
    void MakeRayCast()
    {
        Vector3 middlePoint = (wristPoint.position + indexFingerKnucklePoint.position + smallFingerKnucklePoint.position) / 3f;
        Vector3 dir = -Vector3.Cross(indexFingerKnucklePoint.position - wristPoint.position, smallFingerKnucklePoint.position - wristPoint.position).normalized;
        Ray ray = new Ray(middlePoint, Quaternion.Euler(0f, 30f, 0f) * dir);


        if (Physics.Raycast(ray, out RaycastHit hit, catchingDistance))
        {
            currentlyPointing = hit.collider.gameObject;
            EnlightObject(currentlyPointing);

            // Visualize the raycast by drawing a line from the cursor position to the hit point
            Debug.DrawLine(ray.origin, hit.point, Color.green);
            DrawMagicPointer(middlePoint, hit.point);
        }
        else
        {
            currentlyPointing = null;

            // Visualize the raycast by drawing a line from the cursor position to the maximum distance
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * catchingDistance, Color.red);
            DrawMagicPointer(middlePoint, middlePoint + ray.direction * catchingDistance);
        }
    }

    

    void DrawMagicPointer(Vector3 startPoint, Vector3 endPoint)
    {
        magicPointer.GetComponent<LineRenderer>().SetPosition(0, startPoint);
        magicPointer.GetComponent<LineRenderer>().SetPosition(1, endPoint);
    }

}
