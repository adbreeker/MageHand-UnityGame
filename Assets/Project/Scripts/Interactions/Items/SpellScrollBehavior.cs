using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellScrollBehavior : MonoBehaviour
{
    public string spellName;
    public int manaCost;
    public string spellDescription;
    public GameObject spellIcon;

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
            spellbook.spells.Add(gameObject);
            // tip.GetComponent<ReadableNote>().OpenNote();
            playerHand.inHand = null;
            gameObject.SetActive(false);
        }
    }
}
