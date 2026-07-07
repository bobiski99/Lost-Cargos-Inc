using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 rotationAxis;

    void Update()
    {
        transform.Rotate(rotationAxis * Time.deltaTime);

    }
}