using UnityEngine;

public class WashoPickup : MonoBehaviour
{
    [SerializeField] private float m_delta = 0.5f;
    
    private float m_yStart;

    private void Start()
    {
        m_yStart = this.transform.position.y;
    }

    private void Update()
    {
        Vector3 l_pos = this.transform.position;
        Vector3 l_rot = this.transform.eulerAngles;
        l_pos.y = m_yStart + Mathf.Sin(Time.time) * m_delta;
        l_rot.y = Mathf.Sin(Time.time) * 180.0f;

        this.transform.position = l_pos;
        this.transform.eulerAngles = l_rot;
    }

    private void OnTriggerEnter(Collider p_other)
    {
        if (p_other.CompareTag("Player"))
        {
            if (p_other.transform.TryGetComponent<CharacterMovement>(out var l_character))
            {
                l_character.GainStack(3);
                Destroy(this.gameObject);
            }  
        }
    }
}
