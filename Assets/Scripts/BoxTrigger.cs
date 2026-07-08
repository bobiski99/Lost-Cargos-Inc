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
            Destroy(other.GetComponent<BoxSlide>());
            bx.CanBeScanned = true;
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
