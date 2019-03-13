using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : OnCollisionModifier
{

    [SerializeField] private float maxDistance = 25.0f;

    public override void OnCollision(Collision collision)
    {
        // chain logic here
    }

    public override void AddSpellModifier(GameObject go)
    {
        Chain chain = go.AddComponent<Chain>();
    }
}
