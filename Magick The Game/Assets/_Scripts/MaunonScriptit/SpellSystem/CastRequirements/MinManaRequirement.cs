using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MinMana ", menuName = "CastRequirement/MinManaRequirement")]
public class MinManaRequirement : CastRequirement
{
    [SerializeField] private float amount = 10.0f;

    public override bool isMet(Spellbook spellbook)
    {
        if(spellbook.character.Mana.amount >= amount)
        {
            return true;
        }
        return false;
    }
}
