using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBookBehavior : MonoBehaviour
{
    private Spellbook spellbook;
    private HandInteractions handInteractions;

    private void Awake()
    {
        spellbook = FindObjectOfType<Spellbook>();
        handInteractions = FindObjectOfType<HandInteractions>();
    }

    public void OnPickUp()
    {
        spellbook.bookOwned = true;

        foreach(GameObject spellScroll in GameObject.FindGameObjectsWithTag("SpellScroll"))
        {
            spellScroll.layer = LayerMask.NameToLayer("Item");
        }

        //destroying spellbook object
        handInteractions.inHand = null;
        Destroy(gameObject);
    }
}
