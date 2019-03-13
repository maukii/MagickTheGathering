using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileCardName", menuName = "Card/Projectile")]
public class ProjectileCard : Card
{

                                                                                                // make this List<OnCastModifier>
    public List<ProjectileModifierSO> projectileModifiers = new List<ProjectileModifierSO>();   // make this List<OnCollisionModifier>
                                                                                                // make this List<PassiveModifiers> (homing etc.)

}
