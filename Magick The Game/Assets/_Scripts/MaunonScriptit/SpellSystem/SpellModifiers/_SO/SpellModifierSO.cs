using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellModifierSO : ScriptableObject
{
    public virtual void AddSpellComponent(GameObject go) { }
}
