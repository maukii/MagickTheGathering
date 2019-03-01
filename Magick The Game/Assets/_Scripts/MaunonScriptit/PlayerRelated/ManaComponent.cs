using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaComponent : MonoBehaviour
{
    public float amount = 100.0f;
    public float regenRate = 0.5f;
    public float maxAmount = 100.0f;

    /// <summary>
    /// amount will be added to current amount
    /// </summary>
    public void Modify(float amount)
    {
        print("Mana was modified");

        this.amount += amount;

        if (this.amount > maxAmount) this.amount = maxAmount;
        if (this.amount <= 0)
        {
            this.amount = 0;
        }
    }

    private void Update()
    {
        this.amount = Mathf.Clamp(amount + Time.deltaTime * regenRate, 0, maxAmount);
    }
}
