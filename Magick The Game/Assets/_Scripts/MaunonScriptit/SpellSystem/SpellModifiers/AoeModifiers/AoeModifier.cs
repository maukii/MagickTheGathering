﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AoeModifier : SpellModifier
{
    // base class
    public abstract void Apply(Character character);
}
