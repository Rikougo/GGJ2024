using UnityEngine;
using UnityEngine.Events;

public class BossTrigger : MonoBehaviour
{
    public UnityEvent Triggered;
    
    private void OnTriggerEnter(Collider p_other)
    {
        if (p_other.CompareTag("Player"))
        {
            this.Triggered?.Invoke();
        }
    }
}
