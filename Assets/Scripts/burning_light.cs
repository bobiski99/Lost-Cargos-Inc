using DG.Tweening;
using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.GlobalIllumination;

public class burning_light : MonoBehaviour
{
    [SerializeField] private Light pointLight;
    private Light _light;

    private Coroutine routine;

    [Header("Şiddet Ayarları")]
    public float minIntensity = 0.7f;
    public float maxIntensity = 1.3f;

    private float defaultMin;
    private float defaultMax;

    [Header("Hız Ayarları")]
    public float minDuration = 0.05f;
    public float maxDuration = 0.2f;

    void Start()
    {
        defaultMax = maxIntensity;
        defaultMin = minIntensity;
        _light = GetComponent<Light>();
        if (_light == null)
        {
            Debug.LogError("Bu objede bir Light bileşeni bulunamadı!");
            return;
        }
        pointLight.enabled = false;
        // İlk titremeyi başlat
        Flicker();

    }
    public void TurnOnForSeconds(float seconds)
    {
        pointLight.enabled = true;

        minIntensity = defaultMin;
        maxIntensity = defaultMax;

        // 3 saniye sonra sönmeye başla
        DOVirtual.DelayedCall(seconds, () =>
        {
            DOTween.To(() => minIntensity,
                x => minIntensity = x,
                0f,
                1f);

            DOTween.To(() => maxIntensity,
                x => maxIntensity = x,
                0f,
                1f)
            .OnComplete(() =>
            {
                pointLight.enabled = false;
            });
        });
    }

    void Flicker()
    {
        // Rastgele bir hedef şiddet ve süre seç
        float targetIntensity = Random.Range(minIntensity, maxIntensity);
        float duration = Random.Range(minDuration, maxDuration);

        // DOTween ile yumuşak geçiş yap
        _light.DOIntensity(targetIntensity, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(Flicker); // Animasyon bitince kendini tekrar çağır
    }

    private void OnDestroy()
    {
        // Obje yok edilirse tween'i durdur (bellek yönetimi için)
        _light.DOKill();
    }
}