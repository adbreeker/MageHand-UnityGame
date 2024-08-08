using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    Vector3 _middlePoint;

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
            _middlePoint = (wristPoint.position + indexFingerKnucklePoint.position + smallFingerKnucklePoint.position) / 3f;

            if (PlayerParams.Variables.uiActive)
            {
                CheckObjectsUI(0.2f);
            }
            else
            {
                if(PlayerParams.Controllers.playerMovement.isLeaning) { CheckObjects(2.0f, 0.3f); }
                else { }
            }
        }
    }

    void CheckObjects(float distance, float radius)
    {
        Ray ray = PlayerParams.Objects.playerCamera.ViewportPointToRay(PlayerParams.Objects.playerCamera.WorldToViewportPoint(_middlePoint));

        Vector3 startPoint = ray.origin;
        Vector3 endPoint = ray.origin + ray.direction * distance;

        Collider[] hitColliders = Physics.OverlapCapsule(startPoint, endPoint, radius, objectsMask, QueryTriggerInteraction.Collide);
#if UNITY_EDITOR
        DrawDebugCylinder(startPoint, endPoint, radius);
#endif
        if(GetInteractableSortedByDistance(ref hitColliders))
        {
            foreach (Collider collider in hitColliders)
            {
                if (IsObjectVisible(collider))
                {
                    currentlyPointing = collider.gameObject;
                    EnlightObject(currentlyPointing);
                    return;
                }
            }
            currentlyPointing = null;
        }
        else
        {
            currentlyPointing = null;
        }
    }

    public void DrawDebugCylinder(Vector3 startPoint, Vector3 endPoint, float radius, int segments = 20)
    {
        // Vector representing the axis of the cylinder
        Vector3 axis = endPoint - startPoint;
        Vector3 axisNormalized = axis.normalized;

        // Find any vector that is not parallel to the cylinder's axis
        Vector3 perpendicularVector = Vector3.Cross(axisNormalized, Vector3.up);
        if (perpendicularVector == Vector3.zero) // If the vector happened to be parallel to "up", choose a different vector
        {
            perpendicularVector = Vector3.Cross(axisNormalized, Vector3.right);
        }
        perpendicularVector.Normalize();

        // Draw the center axis of the cylinder
        Debug.DrawLine(startPoint, endPoint, Color.cyan);

        // Draw the cross-section of the cylinder
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments;
            float nextAngle = (i + 1) * Mathf.PI * 2 / segments;

            // Calculate points on the circle around startPoint and endPoint
            Vector3 point1 = startPoint + Quaternion.AngleAxis(angle * Mathf.Rad2Deg, axisNormalized) * perpendicularVector * radius;
            Vector3 point2 = startPoint + Quaternion.AngleAxis(nextAngle * Mathf.Rad2Deg, axisNormalized) * perpendicularVector * radius;

            Vector3 point3 = endPoint + Quaternion.AngleAxis(angle * Mathf.Rad2Deg, axisNormalized) * perpendicularVector * radius;
            Vector3 point4 = endPoint + Quaternion.AngleAxis(nextAngle * Mathf.Rad2Deg, axisNormalized) * perpendicularVector * radius;

            // Draw the cross-sections (base and top of the cylinder)
            Debug.DrawLine(point1, point2, Color.blue);
            Debug.DrawLine(point3, point4, Color.blue);

            // Draw lines connecting the two bases
            Debug.DrawLine(point1, point3, Color.blue);
        }
    }

    void CheckObjectsUI(float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(_middlePoint, radius, uiMask);

        if (colliders.Length > 0) //first found object becomes currently pointed
        {
            System.Array.Sort(colliders, (collider1, collider2) =>
            Vector3.Distance(collider1.transform.position, _middlePoint).CompareTo(
            Vector3.Distance(collider2.transform.position, _middlePoint)));

            currentlyPointing = colliders[0].gameObject;
            EnlightObject(currentlyPointing);
        }
        else //if no objects then currently pointed is null
        {
            currentlyPointing = null;
        }
    }

    bool GetInteractableSortedByDistance(ref Collider[] potentialObjects)
    {
        List<Collider> interactable = new List<Collider>();
        foreach (Collider col in potentialObjects)
        {
            if (IsObjectInteractable(col)) { interactable.Add(col); }
        }

        if (interactable.Count > 0)
        {
            Camera cam = PlayerParams.Objects.playerCamera;
            Vector3 middleScreenPoint = cam.WorldToScreenPoint(_middlePoint);

            // Sort the list of interactable colliders based on their distance from middleScreenPoint
            potentialObjects = interactable.OrderBy(collider =>
            {
                // Calculate the screen position of the collider's center
                Vector3 colliderScreenPoint = cam.WorldToScreenPoint(collider.bounds.center);

                // Calculate the 2D distance between the middleScreenPoint and the collider's screen point
                return Vector2.Distance(new Vector2(middleScreenPoint.x, middleScreenPoint.y),
                                        new Vector2(colliderScreenPoint.x, colliderScreenPoint.y));
            }).ToArray();

            return true;
        }

        return false;
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
