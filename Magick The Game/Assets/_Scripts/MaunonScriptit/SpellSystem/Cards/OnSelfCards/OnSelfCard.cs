using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OnSelfCardName", menuName = "Card/OnSelf")]
public class OnSelfCard : Card
{

    public List<OnSelfModifierSO> onSelfModifiers = new List<OnSelfModifierSO>();

}
