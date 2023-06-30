using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SpellScrollInfo
{
    public string spellName;
    public int manaCost;
    public string spellDescription;
    public Texture spellPicture;
}

public class SpellScrollBehavior : MonoBehaviour
{
    [Header("Spell informations")]
    public string spellName;
    public int manaCost;
    public string spellDescription;
    public Texture spellPicture;

    private Spellbook spellbook;

    private void Start()
    {
        spellbook = FindObjectOfType<Spellbook>();

        if(!spellbook.bookOwned)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    public void OnPickUp()
    {
        spellbook.AddSpell(GetSpellScrollInfo());
        Destroy(gameObject);
    }

    public SpellScrollInfo GetSpellScrollInfo()
    {
        SpellScrollInfo info;
        info.spellName = spellName;
        info.manaCost = manaCost;
        info.spellDescription = spellDescription;
        info.spellPicture = spellPicture;

        return info;
    }
}
