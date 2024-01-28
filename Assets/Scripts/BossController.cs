using System;
using BloodTribute.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossController : MonoBehaviour
{
    [SerializeField] private Sprite m_idleSprite;
    [SerializeField] private Sprite m_attackingSprite;
    [SerializeField] private Sprite[] m_reloadSprites;
    [SerializeField] private float m_reloadFramerate;
    [SerializeField] private SpriteAnimator m_explosionAnimator;
    [SerializeField] private AudioSource m_attackSource;
    [SerializeField] private LayerMask m_layerMask;
    
    [SerializeField] private LineRenderer[] m_laserPool;

    [SerializeField] private float m_lockTargetTime = 1.0f;
    [SerializeField] private float m_fireTime = 0.8f;
    [SerializeField] private float m_fireRate = 5.0f;
    [SerializeField] private float m_laserTime = 0.2f;
    [SerializeField] private int m_lives = 10;

    [SerializeField] private Transform m_player;
    
    private Vector3 m_lockDirection;
    private float m_lockTargetTimer;
    private float m_fireTimer;
    private float m_twoBulletTimer;
    private bool m_hasLock;

    private bool m_started;

    private int m_currentLaser;
    private bool[] m_laserActive;
    private float[] m_laserActivationTime;

    private int m_currentLaserIndex;

    public event Action OnDeath;

    private void Awake()
    {
        m_started = false;

        m_currentLaser = 0;
        m_laserActivationTime = new float[m_laserPool.Length];
        m_laserActive = new bool[m_laserPool.Length];
    }

    public void Init()
    {
        m_started = true;
        this.InitLock();
    }

    private void Update()
    {
        if (!m_started) return;
        
        this.LockAndFire();

        for (int l_index = 0; l_index < m_laserPool.Length; l_index++)
        {
            if (m_laserActive[l_index])
            {
                m_laserActivationTime[l_index] -= Time.deltaTime;

                if (m_laserActivationTime[l_index] <= 0.0f)
                {
                    m_laserActive[l_index] = false;
                    m_laserPool[l_index].enabled = false;
                }
            }
        }
    }
    
    private void InitLock()
    {
        m_attackSource.Stop();
        m_hasLock = false;
        m_lockTargetTimer = m_lockTargetTime + Random.Range(0.0f, 0.2f);
        m_lockDirection = (m_player.position - this.transform.position).normalized;
    }
    
    private void LockAndFire()
    {
        if (m_lockTargetTimer > 0.0f)
        {
            m_lockTargetTimer -= Time.deltaTime;
        }
        else if (!m_hasLock)
        {
            m_hasLock = true;
            m_fireTimer = m_fireTime;
            m_twoBulletTimer = 0.1f;
            m_attackSource.Play();
        }

        if (m_hasLock)
        {
            m_fireTimer -= Time.deltaTime;
            m_twoBulletTimer -= Time.deltaTime;

            if (m_twoBulletTimer <= 0.0f)
            {
                m_explosionAnimator.Play();
                if (Physics.Raycast(this.transform.position,
                        m_lockDirection,
                        out RaycastHit l_hit,
                        999.0f,
                        m_layerMask))
                {
                    if (l_hit.transform.TryGetComponent<CharacterMovement>(out var l_character))
                    {
                        l_character.Hit();
                    }
                    
                    this.ShowLaser(l_hit.point);
                }
                
                m_lockDirection = (m_player.position - this.transform.position).normalized;
                m_twoBulletTimer = 1.0f / m_fireRate;
            }

            if (m_fireTimer <= 0.0f)
            {
                this.InitLock();
            }
        }
    }
    
    private void ShowLaser(Vector3 p_hitPoint)
    {
        m_laserActive[m_currentLaser] = true;
        m_laserActivationTime[m_currentLaser] = m_laserTime;
        m_laserPool[m_currentLaser].enabled = true;
        m_laserPool[m_currentLaser].SetPosition(0, m_laserPool[m_currentLaser].transform.position); 
        m_laserPool[m_currentLaser].SetPosition(1, p_hitPoint);

        m_currentLaser = (m_currentLaser + 1) % m_laserPool.Length;
    }

    public void Hit()
    {
        m_lives--;

        if (m_lives <= 0)
        {
            this.OnDeath?.Invoke();
            Destroy(this);
        }
    }
}