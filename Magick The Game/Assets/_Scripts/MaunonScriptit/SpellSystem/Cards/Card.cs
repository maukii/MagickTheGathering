using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : ScriptableObject
{
    // base for all cards

    public List<SpellBalance> balances = new List<SpellBalance>();
    public List<CastRequirement> castRequirements = new List<CastRequirement>();

}
