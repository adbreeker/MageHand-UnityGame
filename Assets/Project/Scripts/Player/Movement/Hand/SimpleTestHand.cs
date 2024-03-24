using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTestHand : MonoBehaviour //relic of old times, before gesture recognition
{
    public Camera mainCamera;
    public float handDistance = 1.5f;
    public float catchingDistance = 2.5f;

    public GameObject currentlyPointing;


    private void Start()
    {
        //hide cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        HandMovement();
        HandClick();
    }

    void HandMovement() //moving hand object with mouse
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        Vector3 handPos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, handDistance));

        this.transform.position = handPos;

        if (Physics.Raycast(ray, out RaycastHit hit, catchingDistance))
        {
            currentlyPointing = hit.collider.gameObject;
            EnlightObject(currentlyPointing);

            // Visualize the raycast by drawing a line from the cursor position to the hit point
            Debug.DrawLine(ray.origin, hit.point, Color.green);
        }
        else
        {
            currentlyPointing = null;
            // Visualize the raycast by drawing a line from the cursor position to the maximum distance
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * catchingDistance, Color.red);
        }
    }

    void HandClick() //clicking on pointed object
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(LayerMask.LayerToName(currentlyPointing.layer) == "Interaction")
            {
                currentlyPointing.SendMessage("OnClick");
            }
        }
    }

    void EnlightObject(GameObject pointingAt) //enlightening pointed object
    {
        if(pointingAt.layer == LayerMask.NameToLayer("Item") && pointingAt != GetComponent<HandInteractions>().inHand)
        {
            if (pointingAt.GetComponent<EnlightObject>() != null)
            {
                pointingAt.GetComponent<EnlightObject>().enlightenTime = 10;
            }
            else
            {
                pointingAt.AddComponent<EnlightObject>();
            }
        }
    }
}
