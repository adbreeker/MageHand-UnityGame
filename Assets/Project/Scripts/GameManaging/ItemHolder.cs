using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    [Header("Items")]
    public List<GameObject> items;

    public GameObject GiveItem(string itemName)
    {
        foreach(GameObject item in items)
        {
            if(item.GetComponent<ItemParameters>().itemName == itemName)
            {
                return Instantiate(item, new Vector3(0,10,0), Quaternion.identity);
            }
        }
        return null;
    }
}
