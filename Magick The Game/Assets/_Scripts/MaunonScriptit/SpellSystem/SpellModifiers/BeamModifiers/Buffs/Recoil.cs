using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : BeamModifier
{
    // recoil power

    private void Update()
    {
        // recoil logic here
    }


    public override void AddSpellModifier(GameObject go)
    {
        Recoil recoil = go.AddComponent<Recoil>();
    }

}
