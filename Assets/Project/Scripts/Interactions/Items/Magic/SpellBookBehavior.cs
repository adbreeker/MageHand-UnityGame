using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBookBehavior : MonoBehaviour
{
    private Spellbook spellbook;
    private HandInteractions handInteractions;

    private void Awake() //get necessary objects on awake
    {
        spellbook = FindObjectOfType<Spellbook>();
        handInteractions = FindObjectOfType<HandInteractions>();
    }

    public void OnPickUp() //on pick up toggle spellbook on spellbook ui script and make all spell scrolls pickable again
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
