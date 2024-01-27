using System;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Vector3 m_targetPosition;
    [SerializeField] private float m_animationTime = 2.0f;

    private bool m_open = false;
    private Vector3 m_velocity;
    
    private void Update()
    {
        if (m_open)
        {
            this.transform.position = Vector3.SmoothDamp(this.transform.position, m_targetPosition, ref m_velocity, m_animationTime);
        }
    }

    private void OnTriggerEnter(Collider p_other)
    {
        if (p_other.CompareTag("Player"))
        {
            this.Open();
        }
    }

    private void Open()
    {
        m_open = true;
        m_targetPosition = new Vector3(this.transform.position.x, m_targetPosition.y, this.transform.position.z);
    }
}
