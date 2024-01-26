using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField] private Vector3 m_initialHandPos;
    [SerializeField] private Vector3 m_downHandPos;
    [SerializeField] private float m_animationTime;

    private Vector3 m_targetPos;
    private bool m_isDown;

    private Vector3 m_animationVelocity;

    private void Awake()
    {
        m_isDown = false;
        this.transform.localPosition = m_initialHandPos;
        m_targetPos = this.transform.localPosition;
    }

    public void ShowHands(bool p_show)
    {
        m_targetPos = p_show ? m_initialHandPos : m_downHandPos;
    }

    private void Update()
    {
        if (Vector3.SqrMagnitude(m_targetPos - this.transform.position) > 0.5f)
        {
            this.transform.localPosition = Vector3.SmoothDamp(m_targetPos, this.transform.localPosition, ref m_animationVelocity, m_animationTime);
        }
    }
}
