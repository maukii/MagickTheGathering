using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : OnCollisionModifier
{

    [SerializeField] private int bounceCount = 2;

    private void Start()
    {
        ready = false;
    }

    public override void OnCollision(Collision collision)
    {
        if (!ready)
        {
            print("BOUNCE");

            transform.rotation = Quaternion.FromToRotation(transform.right, collision.contacts[0].normal);
            bounceCount--;

            if (bounceCount <= 0)
            {
                ready = true;
            }
        }
    }
}
