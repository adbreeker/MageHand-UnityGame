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

    public void EnlightItem(int enlighteningTime) //yellow englight - items
    {
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
    }

    public void EnlightInteraction(int enlighteningTime) //orange enlight - interactions
    {
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
    }
}
