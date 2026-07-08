using DG.Tweening;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class DeliverButton : MonoBehaviour
{
    [Header("Transform Ayarlarý")]
    [SerializeField] private Vector3 _targetPosition;
    [SerializeField] private Vector3 _targetRotation;
    [SerializeField] private Vector3 _targetPosition1;
    [SerializeField] private Vector3 _targetbase;
    [SerializeField] private Vector3 _targetrotbase;
    [SerializeField] private float _animationDuration = 1.5f;

    [SerializeField] private burning_light lightController;

    [Header("Bilesenler")]
    [SerializeField] private Collider _targetCollider;

    public void Awake()
    {
        if (_targetCollider != null)
            _targetCollider.enabled = false;
    }

    public void ActivateObject()
    {
        if (_targetCollider != null)
            _targetCollider.enabled = true;

        transform.DOMove(_targetPosition, _animationDuration).SetEase(Ease.OutElastic);
        transform.DORotate(_targetRotation, _animationDuration).SetEase(Ease.OutElastic);
    }

    private void OnMouseDown()
    {
        if (_targetCollider != null)
            _targetCollider.enabled = false;
        CargoCoreManager.instance.GivePoint(100);
        CargoCoreManager.instance.Heal();
        lightController.TurnOnForSeconds(3f);
        transform.DOMove(_targetPosition1, _animationDuration).SetEase(Ease.OutExpo);
        transform.DOMove(_targetbase, _animationDuration).SetDelay(2f);
        transform.DORotate(_targetrotbase, _animationDuration).SetDelay(2f);
    }
}
