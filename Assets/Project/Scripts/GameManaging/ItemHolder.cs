using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    //holder for item prefabs, necessary for saves loading and dev console
    [Header("Items")]
    public List<GameObject> items;

    public GameObject GiveItem(string itemName) //returning requested item
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
