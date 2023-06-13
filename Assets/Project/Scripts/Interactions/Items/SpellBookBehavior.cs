using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBookBehavior : MonoBehaviour
{
    public GameObject player;
    public GameObject tip;
    void Update()
    {
        if (player.transform.Find("Main Camera").Find("Hand").GetComponent<HandInteractions>().inHand == gameObject)
        {
            player.GetComponent<Spellbook>().ableToOpen = true;
            tip.GetComponent<ReadableNote>().OpenNote();
            Destroy(gameObject);
            player.transform.Find("Main Camera").Find("Hand").GetComponent<HandInteractions>().inHand = null;
        }
    }
}
