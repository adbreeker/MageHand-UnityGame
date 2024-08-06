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

    [Header("Save to journal")]
    public bool saveToJournal = true;

    [Header("Prefabs")]
    public GameObject notePrefab;

    public void Start()
    {
        itemName = '"' + title + '"';
    }

    public override void OnPickUp() //instantiate note prefab and open note on it
    {
        base.OnPickUp();
        Note note = Instantiate(notePrefab, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<Note>();
        note.titleText = title;
        note.contentText = content;
        note.saveToJournal = saveToJournal;

        note.GetComponent<Note>().OpenNote();

        if(destroy)
        {
            Destroy(gameObject);
        }
    }
}
