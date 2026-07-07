using UnityEngine;

public class BoxDeadTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Hold>(out Hold hold) && other.TryGetComponent<Box>(out Box bx))
        {
            
            CargoCoreManager.instance.takeDamage();
            Destroy(other.gameObject);
        }
    }
}