using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetObjectsNearHand : MonoBehaviour
{
    private GameObject magicPointer; // relic of old times :)
    public LayerMask objectsMask;
    public Transform wristPoint, indexFingerKnucklePoint, smallFingerKnucklePoint;

    public float catchingDistance = 2.5f;

    public GameObject currentlyPointing;


    private void Update()
    {
        if(GetComponent<HandInteractions>().inHand == null)
        {
            CheckSphere();
        }
    }

    void CheckSphere()
    {
        Vector3 middlePoint = (wristPoint.position + indexFingerKnucklePoint.position + smallFingerKnucklePoint.position) / 3f;

        Collider[] colliders;
        if (transform.parent.parent.GetComponent<PlayerMovement>().uiActive)
        {
            colliders = Physics.OverlapSphere(middlePoint, 0.2f, objectsMask);
        }
        else
        {
            colliders = Physics.OverlapSphere(middlePoint, 0.7f, objectsMask);
        }


        if(colliders.Length > 0)
        {
            currentlyPointing = colliders[0].gameObject;
            EnlightObject(currentlyPointing);
        }
        else
        {
            currentlyPointing = null;
        }
    }

    void EnlightObject(GameObject pointingAt)
    {
        if ((pointingAt.layer == LayerMask.NameToLayer("Item") || pointingAt.layer == LayerMask.NameToLayer("Switch") || pointingAt.layer == LayerMask.NameToLayer("UI")) && pointingAt != GetComponent<HandInteractions>().inHand)
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
    }


    //relics of old system

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
