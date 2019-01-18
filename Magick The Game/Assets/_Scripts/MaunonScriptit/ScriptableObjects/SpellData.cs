using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellData")]
public class SpellData : ScriptableObject
{
    public Sprite icon;
    public string spellName;

    [Range(0, 100)]
    public float baseDamage,
                 baseCooldown;

    public List<Element> elements = new List<Element>();
    public List<Type> types = new List<Type>();
}
