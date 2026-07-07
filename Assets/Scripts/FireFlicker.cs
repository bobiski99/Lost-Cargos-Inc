using DG.Tweening;
using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.GlobalIllumination;

public class FireFlicker : MonoBehaviour
{
    private Light _light;


    [Header("Şiddet Ayarları")]
    public float minIntensity = 0.7f;
    public float maxIntensity = 1.3f;

    [Header("Hız Ayarları")]
    public float minDuration = 0.05f;
    public float maxDuration = 0.2f;

    void Start()
    {
        _light = GetComponent<Light>();
        if (_light == null)
        {
            Debug.LogError("Bu objede bir Light bileşeni bulunamadı!");
            return;
        }
        // İlk titremeyi başlat
        Flicker();
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