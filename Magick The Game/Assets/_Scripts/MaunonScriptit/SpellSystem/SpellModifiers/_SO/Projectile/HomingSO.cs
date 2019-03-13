using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingSO : ProjectileModifierSO
{
    public override void AddSpellComponent(GameObject go)
    {
        go.AddComponent<Homing>();
    }
}
