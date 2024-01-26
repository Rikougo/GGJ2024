using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform m_camera;

    private void OnEnable()
    {
        m_camera = Camera.main.transform;
    }

    private void OnDisable()
    {
        m_camera = null;
    }

    private void Update()
    {
        Vector3 l_uiToPlayerDirection = (transform.position - m_camera.position).normalized;
        l_uiToPlayerDirection.y = 0;
        l_uiToPlayerDirection.Normalize();

        transform.rotation = Quaternion.LookRotation(l_uiToPlayerDirection);
    }
}