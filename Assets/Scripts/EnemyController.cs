using System;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    IDLE,
    FOLLOW,
    ATTACK,
    DEAD
}

public class EnemyController : MonoBehaviour
{
    [SerializeField] private LayerMask m_obstaclesLayers;
    [SerializeField] private int m_lives = 3;

    private EnemyState m_currentState;
    private NavMeshAgent m_agent;

    private GameController m_gameController;
    [SerializeField]
    private Transform m_player;

    private void Awake()
    {
        m_agent = this.GetComponent<NavMeshAgent>();
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

        this.UpdateState();

        switch (m_currentState)
        {
            case EnemyState.FOLLOW:
                m_agent.SetDestination(m_player.position);
                break;
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
                Debug.Log(l_checkPlayer);
                if (l_checkPlayer)
                {
                    m_currentState = EnemyState.FOLLOW;
                }
                break;
            default:
                break;
        }
    }

    private bool CheckPlayer()
    {
        Vector3 l_playerDirection = (m_player.position - this.transform.position).normalized;
        return !Physics.Raycast(this.transform.position, 
            l_playerDirection, 
            (m_player.position - this.transform.position).magnitude, 
            m_obstaclesLayers);
    }
    
    public void Hit()
    {
        m_lives--;

        if (m_lives <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(this.transform.position, 
            this.transform.position + (m_player.position - this.transform.position).normalized * (m_player.position - this.transform.position).magnitude);
    }
}
