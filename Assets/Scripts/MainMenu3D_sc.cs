using UnityEngine;
using DG.Tweening;

public class MainMenu3D_sc : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera menuCamera;
    [SerializeField] private Transform door;
    [SerializeField] private ContractFPSController_sc fpsController;

    [Header("Door")]
    [SerializeField] private float doorTargetZ = 157f;
    [SerializeField] private float doorOpenDuration = 1f;
    [SerializeField] private Ease doorEase = Ease.InOutSine;

    [Header("Raycast")]
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private LayerMask buttonLayer = ~0;

    private bool isBusy;

    private void Awake()
    {
        if (menuCamera == null)
            menuCamera = Camera.main;
    }

    private void Update()
    {
        if (isBusy)
            return;

        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = menuCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, buttonLayer))
        {
            physical_buttons button = hit.collider.GetComponentInParent<physical_buttons>();

            if (button != null)
            {
                HandleButton(button.buttonType);
            }
        }
    }

    private void HandleButton(MenuButtonType type)
    {
        switch (type)
        {
            case MenuButtonType.Play:
                OnPlay();
                break;

            case MenuButtonType.Settings:
                Settings();
                break;

            case MenuButtonType.Credits:
                Credits();
                break;

            case MenuButtonType.Exit:
                OnExit();
                break;
        }
    }

    private void OnPlay()
    {
        isBusy = true;

        Vector3 targetRotation = door.localEulerAngles;
        targetRotation.z = doorTargetZ;

        door.DOLocalRotate(targetRotation, doorOpenDuration).SetEase(doorEase).OnComplete(() =>{fpsController.SetControllerEnabled(true);});
    }
    private void Credits()
    {

        Debug.Log("Credits açıldı");
    }
    private void Settings()
    {

        Debug.Log("Settings açıldı");
    }
    private void OnExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}