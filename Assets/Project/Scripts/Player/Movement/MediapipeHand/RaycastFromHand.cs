using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastFromHand : MonoBehaviour
{
    public Transform wristPoint, indexFingerKnucklePoint, smallFingerKnucklePoint;

    public float catchingDistance = 2.5f;

    public GameObject currentlyPointing;


    private void Update()
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
        }
        else
        {

            // Visualize the raycast by drawing a line from the cursor position to the maximum distance
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * catchingDistance, Color.red);
        }
    }

    void EnlightObject(GameObject pointingAt)
    {
        if ((pointingAt.layer == LayerMask.NameToLayer("Item") || pointingAt.layer == LayerMask.NameToLayer("Switch")) && pointingAt != GetComponent<HandInteractions>().inHand)
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

}
