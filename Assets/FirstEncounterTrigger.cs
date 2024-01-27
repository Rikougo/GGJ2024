using UnityEngine;

public class FirstEncounterTrigger : MonoBehaviour
{
    [SerializeField] private GameController m_gameController;
    
    private void OnTriggerEnter(Collider p_other)
    {
        if (p_other.CompareTag("Player"))
        {
            m_gameController.OnFirstEncounterDialog(this.transform);
            Destroy(this.gameObject);
        }
    }
}
