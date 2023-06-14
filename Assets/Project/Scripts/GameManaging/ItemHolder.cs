using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    public List<GameObject> items;

    public GameObject GiveItem(string itemName)
    {
        foreach(GameObject item in items)
        {
            if(item.name == itemName)
            {
                return item;
            }
        }
        return null;
    }
}
