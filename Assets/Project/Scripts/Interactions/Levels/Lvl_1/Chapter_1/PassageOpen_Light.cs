using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassageOpen_Light : MonoBehaviour
{
    public GameObject player;
    public GameObject passage;

    public float locX, locZ;

    private SpellCasting spellCastingController;

    private void Awake()
    {
        spellCastingController = player.GetComponentInChildren<SpellCasting>();
    }

    void Update()
    {
        if(player.transform.position.x == locX && player.transform.position.z == locZ)
        {
            if(spellCastingController.currentSpell == "Light")
            {
                passage.SendMessage("Interaction");
                Destroy(this);
            }
        }
    }
}
