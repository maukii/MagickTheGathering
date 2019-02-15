using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSpike : MonoBehaviour
{
    Transform trap;

    [SerializeField]
    private Vector3 direction;

    void Start()
    {
        trap = GetComponent<Transform>();
    }

    void Update()
    {
        
    }

    public void ActivateTrap(TrapGeneric trapGeneric)
    {

    }
}
