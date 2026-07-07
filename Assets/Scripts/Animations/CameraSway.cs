using UnityEngine;

public class CameraSway : MonoBehaviour
{
    [Header("Mouse Sway Settings")]
    public float positionSwayAmount = 0.05f;
    public float rotationSwayAmount = 2f;
    public float swaySmoothness = 5f;

    [Header("Idle Noise Sway Settings")]
    public float idlePositionAmount = 0.02f;
    public float idleRotationAmount = 1f;
    public float idleSpeed = 0.5f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private float timeCounter;

    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        timeCounter += Time.deltaTime * idleSpeed;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // === Mouse tabanlý pozisyon hedefi ===
        Vector3 mouseOffset = new Vector3(-mouseX, -mouseY, 0) * positionSwayAmount;

        // === Perlin noise ile idle pozisyon ===
        Vector3 idleOffset = new Vector3(
            (Mathf.PerlinNoise(timeCounter, 0f) - 0.5f),
            (Mathf.PerlinNoise(0f, timeCounter) - 0.5f),
            0f
        ) * idlePositionAmount;

        Vector3 targetPosition = initialPosition + mouseOffset + idleOffset;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * swaySmoothness);

        // === Mouse tabanlý rotasyon hedefi ===
        Vector3 mouseRot = new Vector3(
            mouseY * rotationSwayAmount,
            -mouseX * rotationSwayAmount,
            mouseX * rotationSwayAmount * 0.5f
        );

        // === Perlin noise ile idle rotasyon ===
        Vector3 idleRot = new Vector3(
            (Mathf.PerlinNoise(timeCounter + 10, 0f) - 0.5f),
            (Mathf.PerlinNoise(0f, timeCounter + 10) - 0.5f),
            (Mathf.PerlinNoise(timeCounter + 5, timeCounter + 5) - 0.5f)
        ) * idleRotationAmount;

        Quaternion targetRotation = Quaternion.Euler(initialRotation.eulerAngles + mouseRot + idleRot);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * swaySmoothness);
    }
}
