using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BeamCardName", menuName = "Card/Beam")]
public class BeamCard : Card
{

    public List<BeamModifierSO> beamModifiers = new List<BeamModifierSO>();

}
