using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBookBehavior : MonoBehaviour
{
    public GameObject tip;

    private Spellbook spellbook;
    private HandInteractions handInteractions;

    private void Start()
    {
        spellbook = FindObjectOfType<Spellbook>();
        handInteractions = FindObjectOfType<HandInteractions>();
    }

    void Update()
    {
        if (handInteractions.inHand == gameObject)
        {
            spellbook.bookOwned = true;
            tip.GetComponent<ReadableNote>().OpenNote();
            handInteractions.inHand = null;
            Destroy(gameObject);
        }
    }
}
