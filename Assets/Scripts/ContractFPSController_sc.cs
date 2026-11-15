using UnityEngine;

public class ContractFPSController_sc : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 85f;
    public bool controllerEnabled = false;
    private CharacterController characterController;
    private float verticalVelocity;
    private float cameraPitch;
    private bool lastEnabledState;
    [SerializeField] private GameObject customCursor;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }
        SetControllerEnabled(controllerEnabled);
    }

    private void Update()
    {
        if (controllerEnabled != lastEnabledState)
        {
            SetControllerEnabled(controllerEnabled);
        }
        if (!controllerEnabled)
        {
            return;
        }
        HandleMovement();
        HandleLook();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 move = transform.right * horizontal +transform.forward * vertical;
        move = Vector3.ClampMagnitude(move, 1f);
        if (characterController.isGrounded && verticalVelocity < 0f)verticalVelocity = -2f;
        verticalVelocity += gravity * Time.deltaTime;
        Vector3 velocity = move * moveSpeed;
        velocity.y = verticalVelocity;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch,-maxLookAngle,maxLookAngle);
        playerCamera.transform.localRotation =Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    public void SetControllerEnabled(bool enabled)
    {
        controllerEnabled = enabled;
        Cursor.lockState = enabled? CursorLockMode.Locked: CursorLockMode.None;
        Cursor.visible = !enabled;
        if (customCursor != null)
        {
            customCursor.SetActive(!enabled);
        }
    }
    public void SetCameraPitch(float pitch)
    {
        cameraPitch = pitch;
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
    public bool IsControllerEnabled()
    {
        return controllerEnabled;
    }
}