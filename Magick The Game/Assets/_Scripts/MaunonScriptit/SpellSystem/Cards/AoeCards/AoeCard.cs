using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AoeCardName", menuName = "Card/Aoe")]
public class AoeCard : Card
{

    public List<AoeModifierSO> aoeModifiers = new List<AoeModifierSO>();

}
