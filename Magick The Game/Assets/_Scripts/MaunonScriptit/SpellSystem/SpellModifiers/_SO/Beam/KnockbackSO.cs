using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackSO : BeamModifierSO
{
    public override void AddSpellComponent(GameObject go)
    {
        go.AddComponent<KnockBack>();
    }
}
