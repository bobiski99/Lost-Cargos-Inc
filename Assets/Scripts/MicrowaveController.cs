
using UnityEngine;

public class MicrowaveController : MonoBehaviour
{
    [SerializeField] private Transform rotatingPart;
    [SerializeField] private float speedPerClick = 500f;
    [SerializeField] private float maxSpeed = 5000f;
    private float currentSpeed = 0f;
    private bool playerNearby = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }

    private void Update()
    {
        if (rotatingPart != null)
        {
            rotatingPart.Rotate(Vector3.forward, currentSpeed * Time.deltaTime, Space.Self);
        }
        if (!playerNearby)
        {
            return;
        }
            

        if (Input.GetKeyDown(KeyCode.E))
        {
            currentSpeed += speedPerClick;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }

        
    }
}
