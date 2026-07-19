using DG.Tweening;
using UnityEngine;

public class CigarettePack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform lid;
    [SerializeField] private cigar cigar;

    [Header("Lid Animation")]
    [SerializeField] private float openAngle = -70f;
    [SerializeField] private float lidDuration = 0.25f;

    [Header("Pack Movement")]
    [SerializeField] private float hiddenX = 6.43f;
    [SerializeField] private float shownX = 5.30f;
    [SerializeField] private float moveDuration = 0.4f;
    [SerializeField] private float autoHideTime = 10f;

    private Vector3 startRotation;

    private Tween lidTween;
    private Tween moveTween;
    private Tween hideTween;

    private bool showing = false;

    private void Awake()
    {
        startRotation = lid.localEulerAngles;
    }

    // D??ar?dan ça?r?lacak
    public void ShowPack()
    {
        if (showing)
            return;

        showing = true;

        hideTween?.Kill();
        moveTween?.Kill();

        moveTween = transform.DOLocalMoveX(shownX, moveDuration).SetEase(Ease.Linear);

        hideTween = DOVirtual.DelayedCall(autoHideTime, HidePack);
    }

    public void HidePack()
    {
        showing = false;

        hideTween?.Kill();
        moveTween?.Kill();

        Close();

        moveTween = transform.DOLocalMoveX(hiddenX, moveDuration).SetEase(Ease.Linear);
    }

    private void OnMouseEnter()
    {
        if (!showing)
            return;

        Open();
    }

    private void OnMouseExit()
    {
        if (!showing)
            return;

        Close();
    }

    private void OnMouseDown()
    {
        if (!showing)
            return;

        cigar.Use();
        HidePack();
    }

    public void Open()
    {
        lidTween?.Kill();

        lidTween = lid.DOLocalRotate(new Vector3(openAngle, startRotation.y, startRotation.z),lidDuration).SetEase(Ease.OutCubic);
    }

    public void Close()
    {
        lidTween?.Kill();

        lidTween = lid.DOLocalRotate(startRotation,lidDuration).SetEase(Ease.OutCubic);
    }
}