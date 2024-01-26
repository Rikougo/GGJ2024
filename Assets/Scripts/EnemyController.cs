using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int m_lives = 3;
    
    public void Hit()
    {
        m_lives--;

        if (m_lives <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
