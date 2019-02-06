//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [SerializeField] private ThirdPersonCamera tpCamera = null;
    [SerializeField] private GameObject projectile = null;
    [SerializeField] private LayerMask physicsLayerMask;

    private new Transform camera = null;

    void Start()
    {
        camera = tpCamera.GetCameraPivot().GetChild(0);
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (projectile != null && tpCamera != null)
            {
                RaycastHit hit;
                if (Physics.Raycast(
                    camera.position,
                    camera.forward,
                    out hit,
                    Mathf.Infinity,
                    physicsLayerMask
                    ))
                {
                    Vector3 direction = Vector3.Normalize(transform.position + Vector3.up * 1.0f - hit.point);
                    Instantiate(projectile).GetComponent<Projectile>().Initialize(transform.position + Vector3.up * 1.0f, -direction);
                }
                else
                {
                    Vector3 direction = Vector3.Normalize(transform.position + Vector3.up * 1.0f - (camera.position + camera.forward * 5000.0f));
                    Instantiate(projectile).GetComponent<Projectile>().Initialize(transform.position + Vector3.up * 1.0f, -direction);
                }
            }
        }
    }
}
