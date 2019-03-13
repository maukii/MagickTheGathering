using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthComponent), typeof(ManaComponent))]
public class Character : MonoBehaviour
{
    public HealthComponent Health
    {
        get { return health; }
    }
    public ManaComponent Mana
    {
        get { return mana; }
    }

    private HealthComponent health;
    private ManaComponent mana;
    public float speed = 100.0f;

    // keep a list of affecting effects here
    public Dictionary<string, bool> effects = new Dictionary<string, bool>();

    void Start()
    {
        health = GetComponent<HealthComponent>();
        mana = GetComponent<ManaComponent>();

        string[] effectCount = System.Enum.GetNames(typeof(Effect));
        for (int i = 0; i < effectCount.Length; i++)
        {
            effects.Add(effectCount[i], false);
        }
    }

    private void Update()
    {
        // make effects apply here and remove when duration is over
    }

}

public enum Effect
{
    SLOW,
    POISON,
    BURN,
    MOIST,
};
