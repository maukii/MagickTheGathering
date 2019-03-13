using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowAuraSO : AoeModifierSO
{
    public override void AddSpellComponent(GameObject go)
    {
        go.AddComponent<AoeSlow>();
    }
}
