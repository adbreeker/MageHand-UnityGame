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
                CheckObjectsUI(1f, 0.1f);
            }
            else
            {
                if(PlayerParams.Controllers.playerMovement.isLeaning) { CheckObjects(3.0f, 0.3f); }
                else { CheckObjects(2.0f, 0.3f); }
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
        Vector3 axis = endPoint - startPoint;
        if (axis.sqrMagnitude <= Mathf.Epsilon)
        {
            for (int i = 0; i < segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;
                float nextAngle = (i + 1) * Mathf.PI * 2f / segments;
                Debug.DrawLine(startPoint + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius,
                    startPoint + new Vector3(Mathf.Cos(nextAngle), Mathf.Sin(nextAngle), 0f) * radius, Color.blue);
                Debug.DrawLine(startPoint + new Vector3(0f, Mathf.Cos(angle), Mathf.Sin(angle)) * radius,
                    startPoint + new Vector3(0f, Mathf.Cos(nextAngle), Mathf.Sin(nextAngle)) * radius, Color.blue);
                Debug.DrawLine(startPoint + new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * radius,
                    startPoint + new Vector3(Mathf.Sin(nextAngle), 0f, Mathf.Cos(nextAngle)) * radius, Color.blue);
            }
            return;
        }

        Vector3 axisNormalized = axis.normalized;
        Vector3 perpendicularVector = Vector3.Cross(axisNormalized, Vector3.up);
        if (perpendicularVector.sqrMagnitude <= Mathf.Epsilon)
        {
            perpendicularVector = Vector3.Cross(axisNormalized, Vector3.right);
        }
        perpendicularVector.Normalize();
        Vector3 secondPerpendicular = Vector3.Cross(axisNormalized, perpendicularVector).normalized;

        Debug.DrawLine(startPoint, endPoint, Color.cyan);
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            float nextAngle = (i + 1) * Mathf.PI * 2f / segments;
            Vector3 radial = perpendicularVector * Mathf.Cos(angle) + secondPerpendicular * Mathf.Sin(angle);
            Vector3 nextRadial = perpendicularVector * Mathf.Cos(nextAngle) + secondPerpendicular * Mathf.Sin(nextAngle);
            Vector3 startRingPoint = startPoint + radial * radius;
            Vector3 nextStartRingPoint = startPoint + nextRadial * radius;
            Vector3 endRingPoint = endPoint + radial * radius;
            Vector3 nextEndRingPoint = endPoint + nextRadial * radius;

            Debug.DrawLine(startRingPoint, nextStartRingPoint, Color.blue);
            Debug.DrawLine(endRingPoint, nextEndRingPoint, Color.blue);
            Debug.DrawLine(startRingPoint, endRingPoint, Color.blue);

            Vector3 previousStartCapPoint = startRingPoint;
            Vector3 previousEndCapPoint = endRingPoint;
            for (int capSegment = 1; capSegment <= segments / 4; capSegment++)
            {
                float capAngle = (Mathf.PI * 0.5f) * capSegment / (segments / 4f);
                Vector3 startCapPoint = startPoint + radial * (Mathf.Cos(capAngle) * radius)
                    - axisNormalized * (Mathf.Sin(capAngle) * radius);
                Vector3 endCapPoint = endPoint + radial * (Mathf.Cos(capAngle) * radius)
                    + axisNormalized * (Mathf.Sin(capAngle) * radius);
                Debug.DrawLine(previousStartCapPoint, startCapPoint, Color.blue);
                Debug.DrawLine(previousEndCapPoint, endCapPoint, Color.blue);
                previousStartCapPoint = startCapPoint;
                previousEndCapPoint = endCapPoint;
            }
        }
    }

    void CheckObjectsUI(float distance,float radius)
    {
        Ray ray = PlayerParams.Objects.playerCamera.ViewportPointToRay(PlayerParams.Objects.playerCamera.WorldToViewportPoint(_middlePoint));

        Vector3 startPoint = ray.origin;
        Vector3 endPoint = ray.origin + ray.direction * distance;

        Collider[] colliders = Physics.OverlapCapsule(startPoint, endPoint, radius, uiMask, QueryTriggerInteraction.Collide);
#if UNITY_EDITOR
        DrawDebugCylinder(startPoint, endPoint, radius);
#endif

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

#if UNITY_EDITOR
        foreach (Vector3 point in checkPoints)
        {
            Vector3 screenPoint = cam.WorldToViewportPoint(point);
            Ray ray = cam.ViewportPointToRay(screenPoint);
            Debug.DrawRay(ray.origin, ray.direction * 4.0f, Color.red);
        }
#endif

        foreach (Vector3 point in checkPoints)
        {
            Vector3 screenPoint = cam.WorldToViewportPoint(point);
            Ray ray = cam.ViewportPointToRay(screenPoint);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 4.0f, visibleMask, QueryTriggerInteraction.Ignore))
            {
                if (collider.gameObject.GetComponent<InteractableBehavior>().IsTheSameObject(hit.collider.gameObject))
                {
#if UNITY_EDITOR
                    Debug.DrawRay(ray.origin, ray.direction * 4.0f, Color.green);
#endif
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
            if (pointingAt.layer == LayerMask.NameToLayer("Item") 
                || (pointingAt.layer == LayerMask.NameToLayer("UI") && pointingAt.tag == "UI_ItemCell"))
            {
                pointingAt.GetComponent<InteractableBehavior>().EnlightItem(enlighteningTime);

                //show gesture hint icon
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
                    PlayerParams.Objects.hand.GetComponent<ShowGestureIcon>().icon = iconGrab;
                }
            }

            if (pointingAt.layer == LayerMask.NameToLayer("Interaction")
                || (pointingAt.layer == LayerMask.NameToLayer("UI") && pointingAt.tag == "UI_SpellCell"))
            {
                pointingAt.GetComponent<InteractableBehavior>().EnlightInteraction(enlighteningTime);

                //show gesture hint icon
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
