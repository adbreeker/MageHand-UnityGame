using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageTutorialSpell : MonoBehaviour
{
    [SerializeField] SpellScrollBehavior _tutorialSpellScroll;
    void Start()
    {
        PlayerParams.Controllers.spellbook.bookOwned = true;
        PlayerParams.Controllers.spellbook.AddSpell(_tutorialSpellScroll.spellScrollInfo);
    }

    void Update()
    {
        PlayerParams.Controllers.spellbook.ableToInteract = false;
    }
}
