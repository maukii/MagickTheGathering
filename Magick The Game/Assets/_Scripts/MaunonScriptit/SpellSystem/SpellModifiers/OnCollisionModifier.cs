using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnCollisionModifier : SpellModifier
{
    public bool ready = false;
    public virtual void OnCollision(Collision collision) { }
    public virtual void OnCollision(GameObject go) { }
}
