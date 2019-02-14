using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    public Transform parent;
    
    Rigidbody arrow;

    [SerializeField, Range(1.0f, 20f)]
    private float forceMultiplier;

    [SerializeField, Range(1.0f, 5.0f)]
    private float setTimer;

    [SerializeField]
    private float timer;

    [SerializeField]
    private Vector3 direction;

    void Start()
    {
        arrow = GetComponent<Rigidbody>();
        timer = setTimer;
        arrow.position = parent.position;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer > 0)
        {
            arrow.AddForceAtPosition(direction * forceMultiplier, arrow.transform.position);
        }

        if (timer < 0)
        {
            arrow.position = parent.position;
            arrow.velocity = Vector3.zero;
            timer = setTimer;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            arrow.position = parent.position;
            arrow.velocity = Vector3.zero;
            timer = setTimer;
        }
    }
}
