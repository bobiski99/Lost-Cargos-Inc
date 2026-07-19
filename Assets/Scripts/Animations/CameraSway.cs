using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Camera))]
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

    [Header("Damage Shake")]
    public float damageDuration = 0.4f;
    public float damagePositionStrength = 0.18f;
    public float damageRotationStrength = 2f;
    public float damageFOV = 2.5f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private float timeCounter;

    private Camera cam;
    private float defaultFov;

    private Sequence damageSequence;

    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;

        cam = GetComponent<Camera>();
        defaultFov = cam.fieldOfView;
    }

    void Update()
    {
        timeCounter += Time.deltaTime * idleSpeed;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Mouse Position
        Vector3 mouseOffset = new Vector3(-mouseX, -mouseY, 0) * positionSwayAmount;

        // Idle Position
        Vector3 idleOffset = new Vector3(
            Mathf.PerlinNoise(timeCounter, 0f) - 0.5f,
            Mathf.PerlinNoise(0f, timeCounter) - 0.5f,
            0f
        ) * idlePositionAmount;

        Vector3 targetPosition = initialPosition + mouseOffset + idleOffset;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            Time.deltaTime * swaySmoothness
        );

        // Mouse Rotation
        Vector3 mouseRot = new Vector3(
            mouseY * rotationSwayAmount,
            -mouseX * rotationSwayAmount,
            mouseX * rotationSwayAmount * 0.5f
        );

        // Idle Rotation
        Vector3 idleRot = new Vector3(
            Mathf.PerlinNoise(timeCounter + 10f, 0f) - 0.5f,
            Mathf.PerlinNoise(0f, timeCounter + 10f) - 0.5f,
            Mathf.PerlinNoise(timeCounter + 5f, timeCounter + 5f) - 0.5f
        ) * idleRotationAmount;

        Quaternion targetRotation =
            Quaternion.Euler(initialRotation.eulerAngles + mouseRot + idleRot);

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            Time.deltaTime * swaySmoothness
        );
    }

    public void DamageShake()
    {
        damageSequence?.Kill();

        transform.DOKill();
        cam.DOKill();

        damageSequence = DOTween.Sequence();

        damageSequence.Append(transform.DOShakePosition(damageDuration,damagePositionStrength,45,90,false,true).SetEase(Ease.Linear));

        damageSequence.Join(transform.DOShakeRotation(damageDuration,damageRotationStrength,35,90,true).SetEase(Ease.Linear));

        damageSequence.Join(DOTween.To(() => cam.fieldOfView,x => cam.fieldOfView = x,defaultFov + damageFOV,0.08f).SetEase(Ease.OutQuad));

        damageSequence.Append(DOTween.To(() => cam.fieldOfView,x => cam.fieldOfView = x,defaultFov,0.18f).SetEase(Ease.InQuad));
    }
}