using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : Spell
{

    [Header("-- Beam --")]
    [SerializeField] private float range = 150.0f;

    private RaycastHit hit;

    public override void CastSpell(Spellbook spellbook, int spellIndex)
    {

        Beam beam = Instantiate(this, spellbook.spellPos.position, spellbook.transform.rotation);
        beam.transform.parent = spellbook.transform;

        // add all modifiers from cards
        foreach (BeamCard card in spellbook.spells[spellIndex].cards)
        {
            foreach (BeamModifierSO mod in card.beamModifiers)
            {
                mod.AddSpellComponent(beam.gameObject);
            }
        }

        beam.StartCoroutine(CastBeam(beam.gameObject, spellbook, spellIndex));

    }

    IEnumerator CastBeam(GameObject self, Spellbook spellbook, int spellIndex)
    {
        Camera cam = Camera.main;

        while (true)
        {
            print("Casting beam");

            // beam logic here
            Ray ray = new Ray(spellbook.spellPos.position, spellbook.transform.forward * range);
            Debug.DrawRay(spellbook.spellPos.position, spellbook.transform.forward * range);

            // if beam hits something do this
            if(Physics.Raycast(ray, out hit))
            {
                print("Beam hits something");

                // apply beam effects here to target we hit
                if(hit.transform.GetComponent<Rigidbody>() != null)
                {
                    OnCollisionModifier[] collisionMods = GetComponents<OnCollisionModifier>();
                    foreach (OnCollisionModifier mod in collisionMods)
                    {
                        mod.OnCollision(hit.transform.gameObject);
                    }
                }
            }

            if(Input.GetKeyUp((spellIndex + 1).ToString()) || !Input.GetKey((spellIndex + 1).ToString()))
            {
                print("Beam cast ended");
                break;
            }

            yield return null;
        }

        spellbook.StopCasting();
        spellbook.SetCooldown();
        Destroy(self);
    }

}
