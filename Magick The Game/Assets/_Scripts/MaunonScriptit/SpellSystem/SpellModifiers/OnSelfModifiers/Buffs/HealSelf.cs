using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSelf : OnSelfModifier
{

    private float amount = 10.0f;
    public float Amount
    {
        get { return amount; }
        set { amount = value; }
    }

    public override void AddSpellModifier(GameObject go)
    {
        HealSelf heal = go.AddComponent<HealSelf>();
    }

    private void Start()
    {
        HealthComponent health = GetComponent<HealthComponent>();
        health.Modify(amount);
        Destroy(this);
    }

}
