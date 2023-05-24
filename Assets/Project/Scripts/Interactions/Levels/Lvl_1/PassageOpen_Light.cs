using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassageOpen_Light : MonoBehaviour
{
    public GameObject player;
    public GameObject passage;

    public float locX, locZ;

    void Update()
    {
        if(player.transform.position.x == locX && player.transform.position.z == locZ)
        {
            if(player.GetComponent<SpellCasting>().currentSpell == "Light")
            {
                passage.SendMessage("Interaction");
                this.enabled = false;
            }
        }
    }
}
