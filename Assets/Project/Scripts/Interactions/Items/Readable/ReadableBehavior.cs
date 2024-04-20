using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadableBehavior : ItemBehavior
{
    [Header("Note content")]
    public string title;
    [TextArea(3, 10)]
    public string content;

    [Header("Destroy on pick up")]
    public bool destroy = false;

    [Header("Prefabs")]
    public GameObject notePrefab; 

    public override void OnPickUp() //instantiate note prefab and open note on it
    {
        base.OnPickUp();
        GameObject note = Instantiate(notePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        note.GetComponent<Note>().titleText = title;
        note.GetComponent<Note>().contentText = content;
        note.GetComponent<Note>().OpenNote();

        if(destroy)
        {
            Destroy(gameObject);
        }
    }
}
