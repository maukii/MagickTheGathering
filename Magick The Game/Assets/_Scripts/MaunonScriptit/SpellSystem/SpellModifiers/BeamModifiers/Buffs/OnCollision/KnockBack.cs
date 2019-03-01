using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBack : OnCollisionModifier
{


    public override void OnCollision(Collision collision)
    {
        // push enemies back here
    }

    public override void AddSpellModifier(GameObject go)
    {
        KnockBack knockback = go.AddComponent<KnockBack>();
    }
}
