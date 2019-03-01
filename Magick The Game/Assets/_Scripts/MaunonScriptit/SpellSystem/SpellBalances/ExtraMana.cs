using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExtraMana ", menuName = "Balance/Cost/Mana")]
public class ExtraMana : SpellBalance
{

    [SerializeField] private float amount = 10.0f;

    public override void Cost(Character character)
    {
        character.Mana.Modify(-amount);
    }
}
