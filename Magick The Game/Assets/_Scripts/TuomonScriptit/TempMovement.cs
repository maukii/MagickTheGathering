using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempMovement : MonoBehaviour
{
    public float speed;

    Transform player, cube;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        player = GetComponent<Transform>();
        speed = 25f;
    }

    void Update()
    {
        transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * 7.5f, 0f, Input.GetAxis("Vertical") * Time.deltaTime * 7.5f, Space.Self);
        transform.Rotate(0f, Input.GetAxis("Mouse X") * Time.deltaTime * 30f, 0f, Space.World);
        transform.Rotate(-Input.GetAxis("Mouse Y") * Time.deltaTime * 30f, 0f, 0f, Space.Self);

        RaycastHit hit;

        Debug.DrawRay(player.position, Vector3.forward);

        if (Physics.Raycast(transform.position, transform.forward, out hit, 100f))
        {
            if (hit.collider.tag == "Throwable")
            {
                GameObject caught = hit.collider.gameObject;
                Rigidbody caughtRb = caught.GetComponent<Rigidbody>();

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    caughtRb.isKinematic = true;
                    cube = caught.transform;
                    cube.parent = transform;
                }

                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    caughtRb.isKinematic = false;

                    if (cube != null)
                    {
                        cube.parent = null;
                    }
                }

                if (caughtRb && Input.GetKeyUp(KeyCode.Mouse1))
                {
                    caughtRb.isKinematic = false;
                    caughtRb.AddForce(transform.forward * speed, ForceMode.Impulse);

                    cube.parent = null;
                }
            }
        }
    }
}