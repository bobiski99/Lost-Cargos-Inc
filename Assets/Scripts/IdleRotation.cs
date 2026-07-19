using UnityEngine;

public class IdleRotation : MonoBehaviour
{
    [SerializeField] private float maxAngle = 2f;
    [SerializeField] private float speed = 0.4f;

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void Update()
    {
        float x = (Mathf.PerlinNoise(Time.time * speed, 0f) - 0.5f) * 2f * maxAngle;
        float y = (Mathf.PerlinNoise(0f, Time.time * speed) - 0.5f) * 2f * maxAngle;
        float z = (Mathf.PerlinNoise(Time.time * speed, Time.time * speed) - 0.5f) * 2f * maxAngle;

        transform.localRotation = startRotation * Quaternion.Euler(x, y, z);
    }
}