using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private Transform m_camera;
    [SerializeField] private HandController m_handController;
    [SerializeField] private LineRenderer m_laserRenderer;
    [SerializeField] private LayerMask m_fireMask;
    [SerializeField] private float m_laserTime = 0.2f;
    [SerializeField] private float m_laserCooldown = 0.25f;
    [SerializeField] private float m_speed = 5.0f;

    private CharacterController m_controller;
    private PlayerInput m_input;

    private Vector2 m_direction;

    private float m_laserTimer;
    private bool m_laserActive;
    private float m_laserActivationTime;

    private void Awake()
    {
        m_controller = this.GetComponent<CharacterController>();
        m_input = this.GetComponent<PlayerInput>();

        m_laserCooldown = 0.0f;
        m_laserActive = false;
        m_laserActivationTime = Time.time;
    }

    private void OnEnable()
    {
        m_input.actions["Move"].started += this.OnMove;
        m_input.actions["Move"].performed += this.OnMove;
        m_input.actions["Move"].canceled += this.OnMove;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        m_input.actions["Move"].started -= this.OnMove;
        m_input.actions["Move"].performed -= this.OnMove;
        m_input.actions["Move"].canceled -= this.OnMove;
    }

    private void OnMove(InputAction.CallbackContext p_context)
    {
        m_direction = p_context.canceled ? Vector2.zero : p_context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 l_rawForward = new Vector3(m_camera.forward.x, 0.0f, m_camera.forward.z).normalized;
        Vector3 l_rawRight = new Vector3(m_camera.right.x, 0.0f, m_camera.right.z).normalized;
        Vector3 l_forward = l_rawForward * m_direction.y;
        Vector3 l_right = l_rawRight * m_direction.x;
        Vector3 l_direction = l_forward + l_right;
        Vector3 l_velocity = l_direction.normalized * m_speed;
        m_controller.Move(l_velocity * Time.fixedDeltaTime);

        
    }

    private void Update()
    {
        if (m_laserActive && Time.time - m_laserActivationTime > m_laserTime)
        {
            m_laserRenderer.enabled = false;
            m_laserActive = false;
        }

        if (m_laserTimer <= 0.0f && m_input.actions["Fire"].WasPressedThisFrame())
        {
            m_handController.ShowHands(false);
            // this.Fire();
        }

        if (m_laserTimer > 0.0f) m_laserTimer -= Time.deltaTime;
    }

    private void Fire()
    {
        m_laserTimer = m_laserCooldown;
        
        if (Physics.Raycast(m_camera.position, m_camera.forward, out RaycastHit l_hit, 50.0f, m_fireMask))
        {
            if (l_hit.transform.TryGetComponent(out EnemyController l_enemy))
            {
                l_enemy.Hit();
            }

            this.ShowLaser(l_hit.point);
        }
    }

    private void ShowLaser(Vector3 p_hitPoint)
    {
        m_laserActive = true;
        m_laserActivationTime = Time.time;
        m_laserRenderer.enabled = true;
        m_laserRenderer.SetPosition(1, m_laserRenderer.transform.InverseTransformPoint(p_hitPoint));
    }
}