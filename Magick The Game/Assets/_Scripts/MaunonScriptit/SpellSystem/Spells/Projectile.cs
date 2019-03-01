using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Spell
{

    [Header("-- Projectile --")]
    [SerializeField] protected float range = 1000.0f;
    [SerializeField] protected float speed = 15.0f;
 
    private Vector3 lastPos = Vector3.zero;
    private float distanceTravelled = 0.0f;
    private Vector3 direction = Vector3.zero;

    void Start()
    {
        lastPos = transform.position;
    }

    void FixedUpdate()
    {

        distanceTravelled += Vector3.Distance(transform.position, lastPos);
        lastPos = transform.position;

        if(distanceTravelled < range)
        {
            transform.position += direction * speed * Time.deltaTime;
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
        if(collMods.Length > 0)
        {
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
        print("No collision modifiers...destroying");
        Destroy(gameObject);
    }

    public override void CastSpell(Spellbook spellbook, int spellIndex, Vector3 direction)
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

        direction = spellbook.GetDirection2();
        Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);
        Projectile proj = Instantiate(this, spellbook.spellPos.position, rot);
        proj.direction = direction;
        
        foreach (ProjectileCard card in spellbook.spells[spellIndex].cards)
        {
            // add all modifiers to spell
            foreach (ProjectileModifierSO mod in card.projectileModifiers)
            {
                mod.AddSpellComponent(proj.gameObject);
            }
        }

    }

}
