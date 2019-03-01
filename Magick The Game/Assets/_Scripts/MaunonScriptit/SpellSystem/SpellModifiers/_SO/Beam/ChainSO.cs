using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainSO : BeamModifierSO
{
    public override void AddSpellComponent(GameObject go)
    {
        go.AddComponent<Chain>();
    }
}
