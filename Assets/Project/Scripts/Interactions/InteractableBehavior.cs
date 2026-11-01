using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBehavior : MonoBehaviour
{
    public bool isInteractable = true;
    [SerializeField] List<GameObject> _materialOwners = new();

    void Start()
    {
        if (_materialOwners.Count == 0)
        {
            _materialOwners.Add(this.gameObject);
        }
    }

    public bool IsTheSameObject(GameObject otherObject)
    {
        return _materialOwners.Contains(otherObject);
    }

    public void EnlightItem(int enlighteningTime, GameObject gestureIconPrefab, Texture iconGrab)
    {
        //enlight
        foreach(GameObject materialOwner in _materialOwners)
        {
            if (materialOwner.GetComponent<EnlightObject>() != null)
            {
                materialOwner.GetComponent<EnlightObject>().enlightenTime = enlighteningTime;
            }
            else
            {
                materialOwner.AddComponent<EnlightObject>().materialType = MaterialsAndEffectsHolder.Materials.enlightenItem;
            }
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
            PlayerParams.Objects.hand.GetComponent<ShowGestureIcon>().icon = iconGrab;
        }
    }

    public void EnlightInteraction(int enlighteningTime, GameObject gestureIconPrefab, Texture iconInteract)
    {
        //enlight
        foreach(GameObject materialOwner in _materialOwners)
        {
            if (materialOwner.GetComponent<EnlightObject>() != null)
            {
                materialOwner.GetComponent<EnlightObject>().enlightenTime = enlighteningTime;
            }
            else
            {
                materialOwner.AddComponent<EnlightObject>().materialType = MaterialsAndEffectsHolder.Materials.enlightenInteraction;
            }
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
