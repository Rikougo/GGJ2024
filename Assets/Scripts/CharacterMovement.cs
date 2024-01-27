using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Math = UnityEngine.ProBuilder.Math;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Scene references")] 
    [SerializeField] private GameController m_gameController;
    [SerializeField] private HandController m_handController;
    [SerializeField] private LineRenderer m_laserRenderer;
    [SerializeField] private Transform m_camera;
    [SerializeField] private Transform m_povController;
    [SerializeField] private CinemachineVirtualCamera m_virtualCamera;
    
    [Header("Configuration")]
    [SerializeField] private LayerMask m_fireMask;
    [SerializeField] private float m_laserTime = 0.2f;
    [SerializeField] private float m_laserCooldown = 0.25f;
    [SerializeField] private float m_speed = 5.0f;
    [SerializeField] private float m_cameraSensitivity = 90.0f;
    [SerializeField] private float m_xCameraDeadZone = 5.0f;
    [SerializeField] private float m_yCameraDeadZone = 5.0f;
    [SerializeField] private int m_health = 2;

    private CharacterController m_controller;
    private PlayerInput m_input;
    
    private Vector2 m_direction;
    private Vector2 m_xzCameraRotation;

    private int m_cleanStack;
    private float m_laserTimer;
    private bool m_laserActive;
    private float m_laserActivationTime;

    public CinemachineVirtualCamera VirtualCamera => m_virtualCamera;

    private void Awake()
    {
        m_controller = this.GetComponent<CharacterController>();
        m_input = this.GetComponent<PlayerInput>();

        m_cleanStack = 3;
        m_laserTimer = 0.0f;
        m_laserActive = false;
        m_laserActivationTime = Time.time;
    }

    private void OnEnable()
    {
        m_input.actions["Move"].started += this.OnMove;
        m_input.actions["Move"].performed += this.OnMove;
        m_input.actions["Move"].canceled += this.OnMove;

        m_input.actions["Heal"].started += this.OnHeal;

        m_input.actions["Reload"].started += (p_ctx) => this.ReloadGame();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        m_input.actions["Move"].started -= this.OnMove;
        m_input.actions["Move"].performed -= this.OnMove;
        m_input.actions["Move"].canceled -= this.OnMove;
    }

    private void Start()
    {
        m_handController.SetCurrentHealth(m_health);
    }
    
    private void OnMove(InputAction.CallbackContext p_context)
    {
        m_direction = p_context.canceled ? Vector2.zero : p_context.ReadValue<Vector2>();
    }

    private void OnHeal(InputAction.CallbackContext p_context)
    {
        if (m_handController.Busy || m_cleanStack > 0) return;

        m_health = Math.Clamp(m_health + 1, 0, 2);
        m_handController.Heal(m_health - 1, m_health);
    }

    private void FixedUpdate()
    {
        if (m_gameController.CurrentState != GameState.PLAYING) return;
        
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
        if (m_gameController.CurrentState != GameState.PLAYING) return;
        
        if (m_laserActive && Time.time - m_laserActivationTime > m_laserTime)
        {
            m_laserRenderer.enabled = false;
            m_laserActive = false;
        }

        if (!m_handController.Busy && m_laserTimer <= 0.0f && m_input.actions["Fire"].WasPressedThisFrame())
        {
            this.Fire();
        }

        if (m_laserTimer > 0.0f) m_laserTimer -= Time.deltaTime;
        
        Vector3 l_mouseDelta = m_input.actions["CameraMove"].ReadValue<Vector2>();
        Vector3 l_angleDelta = l_mouseDelta * m_cameraSensitivity;
        l_angleDelta.x = Mathf.Abs(l_angleDelta.x) < m_xCameraDeadZone ? 0.0f : l_angleDelta.x;
        l_angleDelta.y = Mathf.Abs(l_angleDelta.y) < m_yCameraDeadZone ? 0.0f : l_angleDelta.y;
        m_xzCameraRotation += new Vector2(l_angleDelta.x, -l_angleDelta.y);
        m_xzCameraRotation.y = Mathf.Clamp(m_xzCameraRotation.y, -60.0f, 90.0f);
        m_povController.localEulerAngles = new Vector3(m_xzCameraRotation.y, m_xzCameraRotation.x, 0.0f);
    }

    private void Fire()
    {
        if (!m_handController.HasGun) return;
        
        m_laserTimer = m_laserCooldown;
        m_handController.Fire();
        
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

    public void Hit()
    {
        m_health--;

        if (m_health < 0)
        {
            this.ReloadGame();
            return;
        }
        
        m_handController.SetCurrentHealth(m_health);
    }

    private void ReloadGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}