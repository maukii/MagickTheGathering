using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilSO : BeamModifierSO
{
    public override void AddSpellComponent(GameObject go)
    {
        go.AddComponent<Recoil>();
    }
}
