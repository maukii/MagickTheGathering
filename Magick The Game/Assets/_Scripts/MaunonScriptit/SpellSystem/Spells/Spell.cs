using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{

    [Header("-- Spell --")]
    //[SerializeField] protected float value = 10.0f;
    [SerializeField] protected float cooldown = 5.0f;
    [SerializeField] protected float castTime = 5.0f;
    [SerializeField] protected float manaCost = 5.0f;

    public float Cooldown
    {
        get { return cooldown; }
        set { cooldown = value; }
    }
    public float CastTime
    {
        get { return castTime; }
        set { castTime = value; }
    }
    public float ManaCost
    {
        get { return manaCost; }
        set { manaCost = value; }
    }


    // most spells override this cause they require different logic
    public virtual void CastSpell(Spellbook spellbook, int spellIndex)
    {
        // generic spell logic

        // spellbook checks if spell can be casted and applies all balances to the spells

        // create spell instance here and get all card modifiers and add them to the spell
        Spell spell = Instantiate(spellbook.spells[spellIndex].spell, spellbook.spellPos.position, spellbook.transform.rotation);
        foreach (Card card in spellbook.spells[spellIndex].cards)
        {
            foreach (SpellBalance balance in card.balances)
            {
                balance.Apply(spell, spellbook);
            }
        }

    }

}
