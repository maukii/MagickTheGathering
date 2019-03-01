using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Extra cast time ", menuName = "Balance/Casting/CastTime")]
public class CastTime : SpellBalance
{

    [SerializeField] private float amount = 3.0f;

    public override float GetCastingTime()
    {
        return amount;
    }
}
