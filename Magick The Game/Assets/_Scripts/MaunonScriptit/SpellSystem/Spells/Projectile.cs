using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Spell
{

    [Header("-- Projectile --")]
    [SerializeField] protected float range = 1000.0f;
    [SerializeField] protected float speed = 15.0f;

    private Vector3 startPos = Vector3.zero;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, startPos) < range)
        {
            transform.Translate(transform.forward * speed * Time.deltaTime);
        }
        else
        {
            print("Out of range");
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollisionModifier[] collMods = GetComponents<OnCollisionModifier>();
        for (int i = 0; i < collMods.Length; i++)
        {
            if (!collMods[i].ready)
            {
                foreach (OnCollisionModifier mod in collMods)
                {
                    mod.OnCollision(collision);
                }
                break;
            }
            print("Collision modifiers ready, destroying...");
            Destroy(gameObject);
        }
    }

    public override void CastSpell(Spellbook spellbook, int spellIndex)
    {

        ///<summary>
        ///
        ///                                 PROJECTILE SPELLS
        /// 
        ///     • Projectile it self only moves forwards until it's reached maxRange
        ///     • Projectiles can have multiple types of effects
        ///         - Projectiles can home towards closest enemy target
        ///         - Projectiles can have OnCollision modifiers that take effect when projectile collides with something (Split, Bounce, etc.)
        /// 
        /// </summary>

        Projectile proj = Instantiate(this, spellbook.spellPos.position, spellbook.transform.rotation);

        foreach (ProjectileCard card in spellbook.spells[spellIndex].cards)
        {
            // apply all balances to spell
            foreach (SpellBalance balance in card.balances)
            {
                balance.Apply(proj, spellbook);
            }

            // add all modifiers to spell
            foreach (ProjectileModifierSO mod in card.projectileModifiers)
            {
                mod.AddSpellComponent(proj.gameObject);
            }
        }

        spellbook.SetCooldown();

    }
}
