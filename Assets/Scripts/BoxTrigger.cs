using UnityEngine;

public class BoxTrigger : MonoBehaviour
{
    [SerializeField] private Transform boxPos;
    [SerializeField] private BoxCollider col;

    private void Start()
    {
        col = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Hold>(out Hold hold) && other.TryGetComponent<Box>(out Box bx))
        {
            if (bx.Danger) CargoCoreManager.instance.Danger = true;
            Destroy(other.GetComponent<BoxSlide>());

            other.transform.parent = boxPos;

            hold.enabled = true;
            hold.StarterPoint = boxPos.position;
        }
    }

    private void FixedUpdate()
    {
        col.enabled = boxPos.childCount == 0;
    }
}
