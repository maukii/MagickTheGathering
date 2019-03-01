using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileCardName", menuName = "Card/Projectile")]
public class ProjectileCard : Card
{

    public List<ProjectileModifierSO> projectileModifiers = new List<ProjectileModifierSO>();

}
