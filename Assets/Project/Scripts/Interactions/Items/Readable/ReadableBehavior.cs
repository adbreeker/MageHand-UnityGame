using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadableBehavior : MonoBehaviour
{
    [Header("Note content")]
    public string title;
    public string content;

    [Header("Destroy on pick up")]
    public bool destroy = false;

    [Header("Prefabs")]
    public GameObject notePrefab; 

    public void OnPickUp() //instantiate note prefab and open note on it
    {
        GameObject note = Instantiate(notePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        note.GetComponent<ReadableNote>().titleText = title;
        note.GetComponent<ReadableNote>().contentText = content;
        note.GetComponent<ReadableNote>().OpenNote();

        if(destroy)
        {
            Destroy(gameObject);
        }
    }
}
