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
            smokeEffect.enabled = false;
    }

    public void Use()
    {
        moveTween?.Kill();

        // Çakmak sesi
        audioSource.PlayOneShot(lighterClip);

        // 1 saniye sonra
        DOVirtual.DelayedCall(1f, () =>
        {

            // Sigaray? öne getir
            moveTween = transform
                .DOLocalMoveZ(shownZ, 0.4f)
                .SetEase(Ease.Linear);

            // Duman? ba?lat
            if (smokeEffect != null)
            {
                smokeEffect.enabled = true;
                smokeEffect.Reinit();
                smokeEffect.Play();
            }
        });

        // 10 saniye sonra geri çek
        DOVirtual.DelayedCall(11f, () =>
        {
            moveTween?.Kill();

            moveTween = transform
                .DOLocalMoveZ(hiddenZ, 0.4f)
                .SetEase(Ease.Linear);

            // Duman? kapat
            if (smokeEffect != null)
            {
                smokeEffect.Stop();
                smokeEffect.enabled = false;
            }
        });
    }
}