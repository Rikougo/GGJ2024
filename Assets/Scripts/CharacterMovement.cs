using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private Transform m_camera;
    [SerializeField] private float m_speed = 5.0f;
    
    private CharacterController m_controller;
    private PlayerInput m_input;

    private Vector2 m_direction;

    private float m_cameraXRotation = 0.0f;
    
    private void Awake()
    {
        m_controller = this.GetComponent<CharacterController>();
        m_input = this.GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        m_input.actions["Move"].started += this.OnMove;
        m_input.actions["Move"].performed += this.OnMove;
        m_input.actions["Move"].canceled += this.OnMove;

        m_input.actions["Fire"].started += this.OnFire;
        
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

    private void OnFire(InputAction.CallbackContext p_context)
    {
        if (Physics.Raycast(m_camera.position, m_camera.forward, out RaycastHit l_hit, 50.0f, 1 << LayerMask.NameToLayer("Enemy"),
                QueryTriggerInteraction.Ignore))
        {
            Destroy(l_hit.transform.gameObject);
        }
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
}
