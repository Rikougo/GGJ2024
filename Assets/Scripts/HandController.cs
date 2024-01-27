using UnityEngine;

public class HandController : MonoBehaviour
{
    [Header("Laser sprites")] 
    [SerializeField] private SpriteRenderer m_handGunRenderer;
    [SerializeField] private Sprite m_idleLaser;
    [SerializeField] private Sprite m_shootLaser;

    [SerializeField] private float m_shootTime = 0.15f;
    
    [Header("Configuration")]
    [SerializeField] private Vector3 m_initialHandPos;
    [SerializeField] private Vector3 m_downHandPos;
    [SerializeField] private float m_animationTime;

    private Vector3 m_targetPos;
    private Vector3 m_animationVelocity = Vector3.zero;

    private float m_shootTimer;

    private void Awake()
    {
        this.transform.localPosition = m_initialHandPos;
        m_targetPos = m_initialHandPos;
    }

    public void ShowHands(bool p_show)
    {
        m_targetPos = p_show ? m_initialHandPos : m_downHandPos;
        m_animationVelocity = Vector3.zero;
    }

    private void Update()
    {
        if (Vector3.SqrMagnitude(m_targetPos - this.transform.position) > 0.5f)
        {
            this.transform.localPosition = Vector3.SmoothDamp(this.transform.localPosition, m_targetPos, ref m_animationVelocity, m_animationTime);
        }

        if (m_shootTimer > 0.0f)
        {
            m_shootTimer -= Time.deltaTime;
        }
        else
        {
            m_handGunRenderer.sprite = m_idleLaser;
        }
    }

    public void Fire()
    {
        m_shootTimer = m_shootTime;
        m_handGunRenderer.sprite = m_shootLaser;
    }
}
