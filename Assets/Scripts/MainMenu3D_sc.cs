
using UnityEngine;
using DG.Tweening;

public class MainMenu3D_sc : MonoBehaviour
{
    [SerializeField] private Camera menuCamera;
    [SerializeField] private Transform door;
    [SerializeField] private ContractFPSController_sc fpsController;
    [SerializeField] private float doorOpenDuration = 1f;
    [SerializeField] private Ease doorEase = Ease.InOutSine;
    [SerializeField] private float doorTargetZ = -220.74f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private LayerMask buttonLayer = ~0;
    private bool isBusy;
    private physical_buttons currentHoveredButton;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject exitPanel;
    private void Awake()
    {
        if (menuCamera == null)
        {
            menuCamera = Camera.main;
        }
        creditsPanel.SetActive(false);
        exitPanel.SetActive(false);
    }

    private void Update()
    {
        if (isBusy)
        {
            return;
        }

        Ray ray = menuCamera.ScreenPointToRay(Input.mousePosition);

        physical_buttons detectedButton = null;

        if (Physics.Raycast(ray, out RaycastHit hit,
            maxDistance, buttonLayer))
        {
            detectedButton =
                hit.collider.GetComponentInParent<physical_buttons>();
        }

        UpdateHover(detectedButton);

        if (Input.GetMouseButtonDown(0) && detectedButton != null)
        {
            HandleButton(detectedButton.buttonType);
        }
    }

    private void UpdateHover(physical_buttons newButton)
    {
        if (currentHoveredButton == newButton)
            return;

        if (currentHoveredButton != null)
            currentHoveredButton.SetHovered(false);

        currentHoveredButton = newButton;

        if (currentHoveredButton != null)
            currentHoveredButton.SetHovered(true);
    }

    private void HandleButton(MenuButtonType type)
    {
        switch (type)
        {
            case MenuButtonType.Play:
                OnPlay();
                break;

            case MenuButtonType.Credits:
                OpenCredits();
                break;

            case MenuButtonType.Exit:
                OpenExitConfirmation();
                break;
        }
    }

    private void OnPlay()
    {
        isBusy = true;

        if (currentHoveredButton != null)
        {
            currentHoveredButton.SetHovered(false);
            currentHoveredButton = null;
        }

        Vector3 targetRotation = door.localEulerAngles;
        targetRotation.z = doorTargetZ;

        door.DOLocalRotate(
            targetRotation,
            doorOpenDuration
        )
        .SetEase(doorEase)
        .OnComplete(() =>
        {
            fpsController.SetControllerEnabled(true);
        });
    }
    public void OpenCredits()
    {
        isBusy=true;
        creditsPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void CloseCredits()
    {
        isBusy = false;
        creditsPanel.SetActive(false);
    }
    public void OpenExitConfirmation()
    {
        isBusy = true;
        exitPanel.SetActive(true);
        creditsPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void CloseExitConfirmation()
    {
        isBusy = false;
        exitPanel.SetActive(false);
    }
    public void ConfirmExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
