using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitSO : ProjectileModifierSO
{
    public override void AddSpellComponent(GameObject go)
    {
        go.AddComponent<Split>();
    }
}
