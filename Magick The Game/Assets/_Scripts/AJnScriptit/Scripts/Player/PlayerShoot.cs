﻿using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    #region VARIABLES

    public GameObject projectile                        = null;
    [SerializeField] private GameObject reticleBlocked  = null;
    [SerializeField] private LineRenderer line          = null;

    private LayerMask physicsLayerMask                  = 1;
    private new Transform camera                        = null;
    private Vector3 charPositionOffset                  = Vector3.up * 1.0f;
    private Vector3 hitPoint                            = Vector3.zero;

    #endregion

    #region UNITY_DEFAULT_METHODS

    void Start()
    {
        physicsLayerMask    = GetComponent<PlayerCore>().PhysicsLayerMask;
        camera              = Camera.main.transform;
    }

    void Update()
    {
        if (camera != null)
        {
            RaycastHit hitFromCamera;
            RaycastHit hitFromPlayer;

            if (!Physics.Raycast(
                camera.position,
                camera.forward,
                out hitFromCamera,
                Mathf.Infinity,
                physicsLayerMask
                ))
            {
                hitFromCamera.point = transform.position + charPositionOffset + (camera.position + camera.forward * 5000.0f);
            }

            if (reticleBlocked != null && line != null)
            {
                if (Physics.Raycast(
                transform.position + charPositionOffset,
                -Vector3.Normalize(transform.position + charPositionOffset - hitFromCamera.point),
                out hitFromPlayer,
                Mathf.Infinity,
                physicsLayerMask
                ))
                {
                    hitPoint = hitFromPlayer.point;
                    if (AlmostEqual(hitFromPlayer.point, hitFromCamera.point, 0.05f))
                    {
                        reticleBlocked.SetActive(false);
                        line.enabled = false;
                    }
                    else
                    {
                        reticleBlocked.SetActive(true);
                        reticleBlocked.transform.position = hitFromPlayer.point + hitFromPlayer.normal * 0.01f;
                        reticleBlocked.transform.rotation = Quaternion.LookRotation(hitFromPlayer.normal, Vector3.up);

                        line.enabled = true;
                        line.SetPosition(0, transform.position + charPositionOffset);
                        line.SetPosition(1, hitFromPlayer.point);
                    }
                }
                else
                {
                    hitPoint = hitFromCamera.point;
                    reticleBlocked.SetActive(false);
                    line.enabled = false;
                }
            }
        }
    }

    #endregion

    #region CUSTOM_METHODS

    public void ShootProjectile()
    {
        if (projectile != null)
        {
            Vector3 direction = -Vector3.Normalize(transform.position + charPositionOffset - hitPoint);
            Instantiate(projectile).GetComponent<Projectile>().Initialize(transform.position + Vector3.up * 1.0f, direction);
        }
    }

    bool AlmostEqual(Vector3 v1, Vector3 v2, float precision)
    {
        bool equal = true;
        if (Mathf.Abs(Vector3.Angle(v1, v2)) > precision) { equal = false; }
        return equal;
    }

    #endregion
}