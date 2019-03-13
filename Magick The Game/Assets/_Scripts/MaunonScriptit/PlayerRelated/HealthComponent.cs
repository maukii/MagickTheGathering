using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public float amount = 100.0f;
    public float maxAmount = 100.0f;

    /// <summary>
    /// amount will be added to current amount
    /// </summary>
    public void Modify(float amount)
    {
        this.amount += amount;

        if (this.amount > maxAmount) this.amount = maxAmount;
        if (this.amount <= 0)
        {
            print("Player died");
        }
    }
}
