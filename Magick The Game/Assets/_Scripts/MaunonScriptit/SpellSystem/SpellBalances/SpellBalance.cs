using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellBalance : ScriptableObject
{
    // base for all balances
    public virtual void Apply(Spell spell, Spellbook spellbook) { }
    public virtual float GetCastingTime() { return 0; }

    public virtual void Cost(Character character) { }
}
