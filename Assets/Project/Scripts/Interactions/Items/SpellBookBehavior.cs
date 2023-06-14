using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBookBehavior : MonoBehaviour
{
    public GameObject tip;

    private Spellbook spellbook;
    private HandInteractions playerHand;

    private void Start()
    {
        spellbook = FindObjectOfType<Spellbook>();
        playerHand = FindObjectOfType<HandInteractions>();
    }

    void Update()
    {
        if (playerHand.inHand == gameObject)
        {
            spellbook.ableToOpen = true;
            // tip.GetComponent<ReadableNote>().OpenNote();
            playerHand.inHand = null;
            Destroy(gameObject);
        }
    }
}
