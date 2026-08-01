using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(AudioSource))]
public class cigar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VisualEffect smokeEffect;

    [Header("Audio")]
    [SerializeField] private AudioClip lighterClip;

    [Header("Movement")]
    [SerializeField] private float hiddenZ = -6.618f;
    [SerializeField] private float shownZ = -5.45f;

    private AudioSource audioSource;
    private Tween moveTween;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (smokeEffect != null)
        {
            smokeEffect.enabled = false;
        }
    }

    public void Use()
    {
        moveTween?.Kill();

        audioSource.PlayOneShot(lighterClip);

        DOVirtual.DelayedCall(1f, () =>
        {

            moveTween = transform
                .DOLocalMoveZ(shownZ, 0.4f)
                .SetEase(Ease.Linear);

            if (smokeEffect != null)
            {
                smokeEffect.enabled = true;
                smokeEffect.Reinit();
                smokeEffect.Play();
            }
        });

        DOVirtual.DelayedCall(11f, () =>
        {
            moveTween?.Kill();

            moveTween = transform
                .DOLocalMoveZ(hiddenZ, 0.4f)
                .SetEase(Ease.Linear);

            if (smokeEffect != null)
            {
                smokeEffect.Stop();
                smokeEffect.enabled = false;
            }
        });
    }
}