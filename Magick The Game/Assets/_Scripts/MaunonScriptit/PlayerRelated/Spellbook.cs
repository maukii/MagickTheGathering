using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpellData 
{
    public Spell spell;
    public List<Card> cards;
}

[RequireComponent(typeof(Character))]
public class Spellbook : MonoBehaviour
{

    [SerializeField] public bool isCasting = false;
    public Transform spellPos;

    public SpellData[] spells = new SpellData[4];
    private float[] cooldowns = new float[4];

    public Character character { get; private set; }

    private void Start()
    {
        character = GetComponent<Character>();

        for (int i = 0; i < cooldowns.Length; i++)
        {
            cooldowns[i] = 0.0f;
        }

        isCasting = false;
    }

    // casting done here --> make it optimized
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1) && CanCast(0))
        {
            float castingTime = GetCastingTime(0);
            StartCoroutine(StartCastingSpell(0, castingTime));            
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && CanCast(1))
        {
            float castingTime = GetCastingTime(1);
            StartCoroutine(StartCastingSpell(1, castingTime));
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && CanCast(2))
        {
            float castingTime = GetCastingTime(2);
            StartCoroutine(StartCastingSpell(2, castingTime));
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) && CanCast(3))
        {
            float castingTime = GetCastingTime(3);
            StartCoroutine(StartCastingSpell(3, castingTime));
        }
    }

    private bool CanCast(int spellIndex)
    {
        // loop through all cards
        foreach (Card card in spells[spellIndex].cards)
        {
            // check that every cards requirements are met before doing anything
            foreach (CastRequirement requirement in card.castRequirements)
            {
                if(!requirement.isMet(this))
                {
                    print(requirement.name + " was not met");
                    return false;
                }
            }
        }

        // check is spell on cooldown
        if(cooldowns[spellIndex] > Time.time)
        {
            print("Spell is on cooldown");
            return false;
        }

        // check if player is already casting something
        if(isCasting)
        {
            return false;
        }

        return true;
    }

    private float GetCastingTime(int spellIndex)
    {
        float castingTime = spells[spellIndex].spell.CastTime;

        foreach (Card card in spells[spellIndex].cards)
        {
            // get all balances that effect the cast time of the spell
            for (int i = 0; i < card.balances.Count; i++)
            {
                if (card.balances[i].GetType() == typeof(CastTime))
                {
                    castingTime += card.balances[i].GetCastingTime();
                    print("New casting time is: " + castingTime);
                }
            }
        }
        return castingTime;
    }

    private IEnumerator StartCastingSpell(int spellIndex, float castingTime)
    {
        isCasting = true;

        // check if something modifies speed etc. while casting
        yield return new WaitForSeconds(castingTime);

        // take spell cost here
        foreach (Card card in spells[spellIndex].cards)
        {
            foreach (SpellBalance balance in card.balances)
            {
                balance.Cost(character); // cost all here hp/mana/etc.
            }
        }

        spells[spellIndex].spell.CastSpell(this, spellIndex);
        lastCastedSpell = spellIndex;
    }

    public void StopCasting()
    {
        isCasting = false;
    }

    int lastCastedSpell;
    public void SetCooldown()
    {
        cooldowns[lastCastedSpell] = Time.time + spells[lastCastedSpell].spell.Cooldown; // check if some spells card has extra cooldown and add it here
        isCasting = false;
    }
}
