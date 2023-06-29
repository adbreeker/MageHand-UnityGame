using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadableBehavior : MonoBehaviour
{
    [Header("Note content")]
    public string title;
    public string content;

    [Header("Prefabs")]
    public GameObject notePrefab, noteSmallPrefab;

    public void OnPickUp()
    {
        GameObject note = Instantiate(notePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        note.GetComponent<ReadableNote>().titleText = title;
        note.GetComponent<ReadableNote>().contentText = content;
        note.GetComponent<ReadableNote>().OpenNote();
    }
}
