using System.Collections.Generic;
using BloodTribute.Utils;
using UnityEngine;

public class HandController : MonoBehaviour
{
    
    [Header("Hand gun")] 
    [SerializeField] private SpriteRenderer m_handGunRenderer;
    [SerializeField] private Sprite m_idleLaser;
    [SerializeField] private Sprite m_shootLaser;
    [SerializeField] private AudioClip m_laserClip;

    [SerializeField] private float m_shootTime = 0.15f;

    [Header("Clapio")] 
    [SerializeField] private SpriteRenderer m_handClapioRenderer;
    [SerializeField] private SpriteAnimator m_handCleaningAnimator;
    [SerializeField] private SpriteAnimator m_dropletAnimator;
    [SerializeField] private List<Sprite> m_clapioStates;
    [SerializeField] private List<Sprite> m_clapioHeals;
    [SerializeField] private float m_healTime;
    [SerializeField] private AudioClip m_sprayClip;
    
    [Header("Configuration")]
    [SerializeField] private Vector3 m_initialHandPos;
    [SerializeField] private Vector3 m_downHandPos;
    [SerializeField] private float m_animationTime;

    private AudioSource m_audioSource;
    
    private Vector3 m_targetPos;
    private Vector3 m_animationVelocity = Vector3.zero;

    private bool m_shooting;
    private float m_shootTimer;

    private bool m_healing;
    private Sprite m_targetClapioSprite;
    private float m_healTimer;

    public bool Busy => m_healing || m_shooting;

    private void Awake()
    {
        m_healing = false;
        m_healTimer = 0.0f;
        m_targetClapioSprite = m_clapioStates[0];

        m_shooting = false;
        m_shootTimer = 0.0f;
        
        this.transform.localPosition = m_initialHandPos;
        m_targetPos = m_initialHandPos;
        
        m_audioSource = this.GetComponent<AudioSource>();
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
        else if (m_shooting)
        {
            m_shooting = false;
            m_handGunRenderer.sprite = m_idleLaser;
        }

        if (m_healTimer > 0.0f)
        {
            m_healTimer -= Time.deltaTime;
        }
        else if (m_healing)
        {
            m_healing = false;
            m_handClapioRenderer.sprite = m_targetClapioSprite;
            m_handGunRenderer.enabled = true;
            m_handCleaningAnimator.Hide();
        }
    }

    public void Fire()
    {
        this.PlaySound(m_laserClip);
        m_shooting = true;
        m_shootTimer = m_shootTime;
        m_handGunRenderer.sprite = m_shootLaser;
    }

    public void SetCurrentHealth(int p_state)
    {
        Debug.Log(p_state);
        m_handClapioRenderer.sprite = m_clapioStates[p_state];
    }

    public void Heal(int p_previousState, int p_newState)
    {
        m_healing = true;
        m_handGunRenderer.enabled = false;
        m_targetClapioSprite = m_clapioStates[p_newState];
        m_healTimer = m_healTime;

        float l_halfTime = m_healTime / 2.0f;
        float l_frameRate = 1.0f / (l_halfTime / 3.0f);
        
        m_handCleaningAnimator.FrameRate = l_frameRate;
        m_handCleaningAnimator.AnimationEnded += () =>
        {
            this.PlaySound(m_sprayClip);
            m_handClapioRenderer.sprite = m_clapioHeals[p_previousState];
            m_dropletAnimator.FrameRate = l_frameRate;
            m_dropletAnimator.AnimationEnded += () => m_dropletAnimator.Hide();
            m_dropletAnimator.Play();
        };
        m_handCleaningAnimator.Play();
        
        
    }

    public void TakeDamage(int p_newState)
    {
        m_handClapioRenderer.sprite = m_clapioStates[p_newState];
    }
    
    
    private void PlaySound(AudioClip p_clip)
    {
        m_audioSource.loop = false;
        m_audioSource.clip = p_clip;
        m_audioSource.Play();
    }
}
