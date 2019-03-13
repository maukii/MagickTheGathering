using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeSlow : AoeModifier
{

    [SerializeField] private float slowAmount = 0.8f; // this is in percents

    public override void AddSpellModifier(GameObject go)
    {
        go.AddComponent<AoeSlow>();
    }

    public override void Apply(Character character)
    {

        character.speed -= slowAmount * Time.deltaTime;

    }


}
