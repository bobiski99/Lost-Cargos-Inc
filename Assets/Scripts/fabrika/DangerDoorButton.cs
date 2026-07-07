using DG.Tweening;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DangerDoorButton : MonoBehaviour
{
    [SerializeField] private Transform dontbuton;
    [SerializeField] private Transform case1;
    [SerializeField] private Transform case2;
    public bool dangerOpen = false;
    private bool isAnimating = false;
    void Start()
    {

    }
    public void toggle_case()
    {
        if (isAnimating)
            return;

        isAnimating = true;

        Sequence seq = DOTween.Sequence();

        // Button animation
        seq.Append(dontbuton.DOLocalMoveY(0.267f, 0.1f).SetEase(Ease.OutQuad));
        seq.Append(dontbuton.DOLocalMoveY(0.3f, 0.3f).SetEase(Ease.OutBounce));

        if (!dangerOpen)
        {
            case1.DOLocalRotate(new Vector3(80, 0, 0), 0.4f, RotateMode.LocalAxisAdd).SetEase(Ease.OutBack);
            case2.DOLocalRotate(new Vector3(80, 0, 0), 0.4f, RotateMode.LocalAxisAdd).SetEase(Ease.OutBack);
        }
        else
        {
            case1.DOLocalRotate(new Vector3(-80, 0, 0), 0.4f, RotateMode.LocalAxisAdd).SetEase(Ease.OutBack);
            case2.DOLocalRotate(new Vector3(-80, 0, 0), 0.4f, RotateMode.LocalAxisAdd).SetEase(Ease.OutBack);
        }
        dangerOpen = !dangerOpen;
        seq.OnComplete(() =>
        {
            isAnimating = false;
        });
    }
    private void OnMouseDown()
    {
        toggle_case();
    }
    public void TryDangerBox(Box box)
    {
        if (!dangerOpen)
            return;

        if (box.Danger)
        {
            CargoCoreManager.instance.GivePoint(100);
        }
        else
        {
            CargoCoreManager.instance.takeDamage();
        }
    }
}