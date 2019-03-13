using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Split : OnCollisionModifier
{

    public override void AddSpellModifier(GameObject go)
    {
        Split split = go.AddComponent<Split>();
    }

    public override void OnCollision(Collision collision)
    {
        if (!ready)
        {
            print("SPLIT");

            // this rotates the original projectile
            transform.rotation = Quaternion.FromToRotation(transform.right, collision.contacts[0].normal);

            // debug
            Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.green, 10);

            // this creates a instance copy of the original and rotates is 
            Split copy = Instantiate(this, transform.position, Quaternion.identity);
            copy.transform.rotation = Quaternion.FromToRotation(transform.forward, collision.contacts[0].normal);

            ready = true;
            copy.ready = true;
        }
    }
}
