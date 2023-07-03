using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassageOpen_Light : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;

    [Header("Wall")]
    public GameObject passage;

    [Header("Player position to start listening spell cast")]
    public float locX, locZ;

    private SpellCasting spellCastingController;

    private void Awake() //get necessary components on awake
    {
        spellCastingController = player.GetComponentInChildren<SpellCasting>();
    }

    void Update() //open passage if player casted light on specified position and then destroy this script
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
