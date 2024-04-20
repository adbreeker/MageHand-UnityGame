using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBookBehavior : ItemBehavior
{
    private Spellbook spellbook;
    private HandInteractions handInteractions;

    private void Start() //get necessary objects on awake
    {
        spellbook = PlayerParams.Controllers.spellbook;
        handInteractions = PlayerParams.Controllers.handInteractions;
    }

    public override void OnPickUp() //on pick up toggle spellbook on spellbook ui script and make all spell scrolls pickable again
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
