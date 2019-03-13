using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportForwardSO : OnSelfModifierSO
{
    public override void AddSpellComponent(GameObject go)
    {
        TeleportForward teleport = go.AddComponent<TeleportForward>();
    }
}
