using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 rotationAxis;

    void Update()
    {
        if (PauseManager.Instance.IsPaused)
            return;
        transform.Rotate(rotationAxis * Time.deltaTime);

    }
}