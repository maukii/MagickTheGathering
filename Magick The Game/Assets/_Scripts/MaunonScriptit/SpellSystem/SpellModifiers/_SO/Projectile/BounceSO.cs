using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceSO : ProjectileModifierSO
{
    public override void AddSpellComponent(GameObject go)
    {
        go.AddComponent<Bounce>();
    }
}
