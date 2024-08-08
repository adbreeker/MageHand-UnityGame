using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetObjectsNearHand : MonoBehaviour
{
    [Header("Layer masks for objects, UI and covers")]
    public LayerMask objectsMask;
    public LayerMask uiMask;
    public LayerMask visibleMask;

    [Header("Points to calculate middle point")]
    public Transform wristPoint;
    public Transform indexFingerKnucklePoint; 
    public Transform smallFingerKnucklePoint;

    [Header("Currently pointing object")]
    public GameObject currentlyPointing;

    [Header("Enlightening")]
    public int enlighteningTime = 5;
    public Texture iconInteract;
    public Texture iconGrab;
    public GameObject gestureIconPrefab;

    private void Update() //check objects near middle point every update
    {
        if(PlayerParams.Controllers.handInteractions.inHand == null)
        {
            if(PlayerParams.Variables.uiActive)
            {
                CheckObjectsUI();
            }
            else
            {
                CheckObjects();
            }
        }
    }

    void CheckObjects()
    {
        //calculate middle point
        Vector3 middlePoint = (wristPoint.position + indexFingerKnucklePoint.position + smallFingerKnucklePoint.position) / 3f;

        List<Collider> interactableObjects = new List<Collider>();
        Collider[] colliders = Physics.OverlapSphere(middlePoint, 0.7f, objectsMask);

        foreach (Collider collider in colliders)
        {
            if (IsObjectInteractable(collider))
            {
                interactableObjects.Add(collider);
            }
        }


        if (interactableObjects.Count > 0) //first found object becomes currently pointed
        {
            interactableObjects.Sort((collider1, collider2) =>
            Vector3.Distance(collider1.transform.position, middlePoint).CompareTo(
            Vector3.Distance(collider2.transform.position, middlePoint)));

            foreach(Collider collider in interactableObjects)
            {
                if(IsObjectVisible(collider))
                {
                    currentlyPointing = collider.gameObject;
                    EnlightObject(currentlyPointing);
                    return;
                }
            }
            currentlyPointing = null;
        }
        else //if no objects then currently pointed is null
        {
            currentlyPointing = null;
        }
    }

    void CheckObjectsUI()
    {
        //calculate middle point
        Vector3 middlePoint = (wristPoint.position + indexFingerKnucklePoint.position + smallFingerKnucklePoint.position) / 3f;

        Collider[] colliders = Physics.OverlapSphere(middlePoint, 0.2f, uiMask);

        if (colliders.Length > 0) //first found object becomes currently pointed
        {
            System.Array.Sort(colliders, (collider1, collider2) =>
            Vector3.Distance(collider1.transform.position, middlePoint).CompareTo(
            Vector3.Distance(collider2.transform.position, middlePoint)));

            currentlyPointing = colliders[0].gameObject;
            EnlightObject(currentlyPointing);
        }
        else //if no objects then currently pointed is null
        {
            currentlyPointing = null;
        }
    }

    bool IsObjectInteractable(Collider collider)
    {
        if (collider.GetComponent<InteractableBehavior>() != null)
        {
            if (collider.GetComponent<InteractableBehavior>().isInteractable) { return true; }
        }
        return false;
    }

    bool IsObjectVisible(Collider collider)
    {
        Camera cam = PlayerParams.Objects.playerCamera;
        MeshRenderer objectRenderer = collider.GetComponentInChildren<MeshRenderer>();

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(PlayerParams.Objects.playerCamera);
        if (!GeometryUtility.TestPlanesAABB(planes, objectRenderer.bounds))
        {
            return false;
        }

        // Perform detailed visibility check using raycasts
        Bounds bounds = objectRenderer.bounds;
        Vector3 centerPoint = bounds.center;

        // Calculate adjusted corner points
        Vector3[] checkPoints = new Vector3[17];
        checkPoints[0] = centerPoint;

        Vector3[] corners = new Vector3[8]
        {
            bounds.min,
            new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.max.y, bounds.min.z),
            bounds.max
        };

        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 direction = (corners[i] - centerPoint).normalized;
            checkPoints[i + 1] = centerPoint + direction * (bounds.extents.magnitude * 0.5f);
        }
        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 direction = (corners[i] - centerPoint).normalized;
            checkPoints[i + 9] = centerPoint + direction * (bounds.extents.magnitude * 0.9f);
        }

        //foreach (Vector3 point in checkPoints)
        //{
        //    Vector3 screenPoint = cam.WorldToViewportPoint(point);
        //    Ray ray = cam.ViewportPointToRay(screenPoint);
        //    Debug.DrawRay(ray.origin, ray.direction * 4.0f, Color.red);
        //}

        foreach (Vector3 point in checkPoints)
        {
            Vector3 screenPoint = cam.WorldToViewportPoint(point);
            Ray ray = cam.ViewportPointToRay(screenPoint);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 4.0f, visibleMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.gameObject == collider.gameObject 
                    || (collider.tag == "Chest" && hit.transform.parent.gameObject == collider.gameObject))
                {
                    //Debug.DrawRay(ray.origin, ray.direction * 4.0f, Color.green);
                    return true; // At least one point is visible
                }
            }
        }

        return false; // None of the points are visible
    }

    void EnlightObject(GameObject pointingAt) //enlightening pointed objects and showing icons
    {
        if(pointingAt != PlayerParams.Controllers.handInteractions.inHand) //only if no object currently in hand
        {
            //if pointing on item, switch or UI then enlightening only this item and showing proper icon
            if (pointingAt.layer == LayerMask.NameToLayer("Item") || pointingAt.layer == LayerMask.NameToLayer("UI"))
            {
                //enlight
                if (pointingAt.GetComponent<EnlightObject>() != null)
                {
                    pointingAt.GetComponent<EnlightObject>().enlightenTime = enlighteningTime;
                }
                else
                {
                    pointingAt.AddComponent<EnlightObject>().materialType = MaterialsAndEffectsHolder.Materials.enlightenItem;
                }

                //show icon
                if (GameSettings.gestureHints)
                {
                    if (PlayerParams.Objects.hand.GetComponent<ShowGestureIcon>() != null)
                    {
                        PlayerParams.Objects.hand.GetComponent<ShowGestureIcon>().iconTime = enlighteningTime;
                    }
                    else
                    {
                        PlayerParams.Objects.hand.AddComponent<ShowGestureIcon>().gestureIconPrefab = gestureIconPrefab;
                    }
                    if (pointingAt.GetComponent<SpellIcon>() != null) PlayerParams.Objects.hand.GetComponent<ShowGestureIcon>().icon = iconInteract;
                    else PlayerParams.Objects.hand.GetComponent<ShowGestureIcon>().icon = iconGrab;
                }
            }

            if(pointingAt.layer == LayerMask.NameToLayer("Interaction") && pointingAt.tag!="Chest")
            {
                //enlight
                if (pointingAt.GetComponent<EnlightObject>() != null)
                {
                    pointingAt.GetComponent<EnlightObject>().enlightenTime = enlighteningTime;
                }
                else
                {
                    pointingAt.AddComponent<EnlightObject>().materialType = MaterialsAndEffectsHolder.Materials.enlightenInteraction;
                }

                //show icon
                if (GameSettings.gestureHints)
                {
                    if (PlayerParams.Objects.hand.GetComponent<ShowGestureIcon>() != null)
                    {
                        PlayerParams.Objects.hand.GetComponent<ShowGestureIcon>().iconTime = enlighteningTime;
                    }
                    else
                    {
                        PlayerParams.Objects.hand.AddComponent<ShowGestureIcon>().gestureIconPrefab = gestureIconPrefab;
                    }
                    PlayerParams.Objects.hand.GetComponent<ShowGestureIcon>().icon = iconInteract;
                }
            }

            //if pointing on chest then enlightening all child objects
            if(pointingAt.layer == LayerMask.NameToLayer("Interaction") && pointingAt.tag == "Chest")
            {
                //enlight core
                if(pointingAt.GetComponent<ChestBehavior>().chestCore.GetComponent<EnlightObject>() != null)
                {
                    pointingAt.GetComponent<ChestBehavior>().chestCore.GetComponent<EnlightObject>().enlightenTime = enlighteningTime;
                }
                else
                {
                    pointingAt.GetComponent<ChestBehavior>().chestCore.AddComponent<EnlightObject>().materialType = MaterialsAndEffectsHolder.Materials.enlightenInteraction;
                }

                //enlight lid
                if (pointingAt.GetComponent<ChestBehavior>().chestLid.GetComponent<EnlightObject>() != null)
                {
                    pointingAt.GetComponent<ChestBehavior>().chestLid.GetComponent<EnlightObject>().enlightenTime = enlighteningTime;
                }
                else
                {
                    pointingAt.GetComponent<ChestBehavior>().chestLid.AddComponent<EnlightObject>().materialType = MaterialsAndEffectsHolder.Materials.enlightenInteraction;
                }

                //show icon
                if (GameSettings.gestureHints)
                {
                    if (PlayerParams.Objects.hand.GetComponent<ShowGestureIcon>() != null)
                    {
                        PlayerParams.Objects.hand.GetComponent<ShowGestureIcon>().iconTime = enlighteningTime;
                    }
                    else
                    {
                        PlayerParams.Objects.hand.AddComponent<ShowGestureIcon>().gestureIconPrefab = gestureIconPrefab;
                    }
                    PlayerParams.Objects.hand.GetComponent<ShowGestureIcon>().icon = iconInteract;
                }
            }
        }
    }
}
