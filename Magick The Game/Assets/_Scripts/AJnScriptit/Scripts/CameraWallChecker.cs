﻿//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class CameraWallChecker : MonoBehaviour
{
    [SerializeField] private Transform cameraOriginalPos = null;
    [SerializeField] private LayerMask physicsLayerMask;

    void Update()
    {
        if (cameraOriginalPos != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(
                cameraOriginalPos.position + transform.forward * 4.0f,
                -transform.forward,
                out hit,
                4.0f,
                physicsLayerMask
                ))
            {
                transform.position = hit.point;
            }
            else
            {
                transform.position = cameraOriginalPos.position;
            }
        }
    }
}