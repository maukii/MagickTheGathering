using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSelf : Spell
{

    public override void CastSpell(Spellbook spellbook, int spellIndex, Vector3 direction)
    {

        ///<summary>
        ///
        ///                                 SELF SPELLS ARE DIFFERENT
        /// 
        ///     • self spells affect players directly so there is no need to instansiate the prefab
        ///     • on self spells work in a few ways
        ///         • They take instant effect on player (teleport a little distance forwards, etc.)
        ///         • They modify player stats directly (health, movementspeed, etc.) // heal, haste
        ///         • Give player a triple jump until he uses all of them ????
        ///         • Fix this shit --> no need to create instance just apply effects directly to player
        /// 
        /// </summary>


        foreach (OnSelfCard card in spellbook.spells[spellIndex].cards)
        {
            foreach (OnSelfModifierSO mod in card.onSelfModifiers)
            {
                mod.AddSpellComponent(spellbook.gameObject);
            }
        }

        spellbook.StopCasting();
        spellbook.SetCooldown();

    }

}
