using UnityEngine;

public class LifterController : MonoBehaviour
{
    [SerializeField] private Transform m_model;
    [SerializeField] private Transform m_target;

    private void OnTriggerEnter(Collider p_other)
    {
        if (p_other.CompareTag("Player"))
        {
            Vector3 l_oldPos = m_model.InverseTransformPoint(p_other.transform.position);
            Quaternion l_oldRot = p_other.transform.rotation;

            Vector3 l_newPos = m_target.TransformPoint(l_oldPos);

            p_other.GetComponent<CharacterController>().enabled = false;
            p_other.transform.position = l_newPos;
            p_other.GetComponent<CharacterController>().enabled = true;
        }
    }
}
