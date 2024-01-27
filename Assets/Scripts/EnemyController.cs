using System;
using BloodTribute.Utils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum EnemyState
{
    IDLE,
    FOLLOW,
    ATTACK,
    DEAD
}

public class EnemyController : MonoBehaviour
{
    [Header("Visual configuration")]
    [SerializeField] private Sprite m_idleSprite;
    [SerializeField] private Sprite m_attackSprite;
    [SerializeField] private Sprite[] m_walkingSprite;
    
    [Header("Target following")]
    [SerializeField] private LayerMask m_obstaclesLayers;
    [SerializeField] private float m_targetDistanceArea = 2.0f;
    [SerializeField] private float m_targetDistance = 8.0f;
    
    [Header("Shoot configuration")]
    [SerializeField] private float m_lockTargetTime = 1.0f;
    [SerializeField] private float m_walkFramerate = 0.5f;
    [SerializeField] private SpriteAnimator m_explosionAnimator;
    [SerializeField] private AudioSource m_fireSource;
    
    [Header("Misc")]
    [SerializeField] private int m_lives = 3;

    private EnemyState m_currentState;
    private SpriteRenderer m_renderer;
    private NavMeshAgent m_agent;
    private GameController m_gameController;
    private Transform m_player;

    private float m_playerDistance;

    private Vector3 m_lockDirection;
    private float m_lockTargetTimer;
    private bool m_hasLock;

    private int m_currentWalkIndex;
    private float m_walkTimer;

    private void Awake()
    {
        m_agent = this.GetComponent<NavMeshAgent>();
        m_renderer = this.GetComponent<SpriteRenderer>();
        m_agent.stoppingDistance = m_targetDistance;
    }

    private void Start()
    {
        m_gameController = FindAnyObjectByType<GameController>();
        m_player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (m_gameController.CurrentState != GameState.PLAYING)
        {
            m_agent.SetDestination(transform.position);
            return;
        }

        m_playerDistance = (m_player.position - this.transform.position).magnitude;
        this.UpdateState();

        switch (m_currentState)
        {
            case EnemyState.FOLLOW:
                m_walkTimer -= Time.deltaTime;
                if (m_walkTimer <= 0.0f)
                {
                    m_currentWalkIndex = (m_currentWalkIndex + 1) % m_walkingSprite.Length;
                    m_renderer.sprite = m_walkingSprite[m_currentWalkIndex];
                    m_walkTimer = m_walkFramerate;
                }

                m_agent.SetDestination(m_player.position);
                break;
            case EnemyState.ATTACK:
                this.LockAndFire();
                break;
            // LOCK AND FIRE
            default:
                break;
        }
    }

    private void UpdateState()
    {
        switch (m_currentState)
        {
            case EnemyState.IDLE:
                bool l_checkPlayer = this.CheckPlayer();
                if (l_checkPlayer)
                {
                    this.SwitchToFollow();
                }

                break;
            case EnemyState.FOLLOW:
                if (m_playerDistance <= m_targetDistance)
                {
                    this.SwitchToAttack();
                }

                break;
            case EnemyState.ATTACK:
                if (m_playerDistance > (m_targetDistance + m_targetDistanceArea))
                {
                    this.SwitchToFollow();
                }

                break;
            default:
                break;
        }
    }

    private void SwitchToFollow()
    {
        m_currentState = EnemyState.FOLLOW;
        m_currentWalkIndex = 0;
        m_renderer.sprite = m_walkingSprite[m_currentWalkIndex];
        m_walkTimer = m_walkFramerate;
    }

    private void SwitchToAttack()
    {
        m_agent.SetDestination(transform.position);
        m_currentState = EnemyState.ATTACK;
        m_renderer.sprite = m_attackSprite;
        m_lockTargetTimer = m_lockTargetTime;
    }

    private bool CheckPlayer()
    {
        Vector3 l_position = this.transform.position;
        Vector3 l_playerDirection = (m_player.position - l_position).normalized;
        return !Physics.Raycast(l_position,
            l_playerDirection,
            m_playerDistance,
            m_obstaclesLayers);
    }

    private void InitLock()
    {
        m_hasLock = false;
        m_lockTargetTimer = m_lockTargetTime + Random.Range(0.2f, 1.0f);
        m_lockDirection = (m_player.position - this.transform.position).normalized;
    }
    
    private void LockAndFire()
    {
        if (m_lockTargetTimer > 0.0f)
        {
            m_lockTargetTimer -= Time.deltaTime;
        }
        else
        {
            m_hasLock = true;
        }

        if (m_hasLock)
        {
            Debug.Log($"[{DateTime.Now.ToLongTimeString()}] Fire");
            m_explosionAnimator.Play();
            m_fireSource.Play();
            if (Physics.Raycast(this.transform.position,
                    m_lockDirection,
                    out RaycastHit l_hit,
                    999.0f,
                    1 << LayerMask.NameToLayer("Player")))
            {
                if (l_hit.transform.TryGetComponent<CharacterMovement>(out var l_character))
                {
                    l_character.Hit();
                }
            }
            this.InitLock();
        }
    }

    public void Hit()
    {
        m_lives--;

        if (m_lives <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}