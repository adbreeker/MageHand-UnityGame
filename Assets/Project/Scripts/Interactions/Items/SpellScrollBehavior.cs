using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellScrollBehavior : MonoBehaviour
{
    public string spellName;
    public int manaCost;
    public string spellDescription;
    public Texture spellPicture;

    private Spellbook spellbook;
    private HandInteractions handInteractions;

    private void Start()
    {
        spellbook = FindObjectOfType<Spellbook>();
        handInteractions = FindObjectOfType<HandInteractions>();
    }

    void Update()
    {
        if (spellbook.bookOwned)
        {
            gameObject.layer = LayerMask.NameToLayer("Item");
        }

        if (handInteractions.inHand == gameObject)
        {
            spellbook.AddSpellFromScroll(gameObject);
        }
    }
}
