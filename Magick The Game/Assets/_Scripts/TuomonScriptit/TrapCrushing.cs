using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapCrushing : MonoBehaviour
{
    //put this to the pressure plate or whatever trigger you are using

    [SerializeField]
    private Rigidbody rb;   //falling object here

    void Start()
    {
        rb.isKinematic = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            rb.isKinematic = false;
            Destroy(gameObject);
        }
    }
}