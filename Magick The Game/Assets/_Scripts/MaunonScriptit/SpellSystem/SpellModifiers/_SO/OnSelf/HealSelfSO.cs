using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSelfSO : OnSelfModifierSO
{
    [SerializeField] private float amount = 10.0f;

    public override void AddSpellComponent(GameObject go)
    {
        HealSelf healSelf = go.AddComponent<HealSelf>();
        healSelf.Amount = amount;
    }
}
