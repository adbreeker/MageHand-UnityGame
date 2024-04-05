using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LockBehavior : MonoBehaviour
{
    public enum LockType
    {
        Rusty,
        Silver,
        Gold
    }

    [Header("Lock materials:")]
    [SerializeField] Material _rusty;
    [SerializeField] Material _silver;
    [SerializeField] Material _gold;

    [Header("Lock type")]
    public LockType lockType;

    public void OpenLock()
    {
        foreach (SwitchInteraction switchInteraction in GetComponents<SwitchInteraction>())
        {
            switchInteraction.Interact();
        }
        gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        switch(lockType)
        {
            case LockType.Rusty:
                GetComponent<MeshRenderer>().material = _rusty;
                break;
            case LockType.Silver:
                GetComponent<MeshRenderer>().material = _silver;
                break;
            case LockType.Gold:
                GetComponent<MeshRenderer>().material = _gold;
                break;
        }
    }
}
