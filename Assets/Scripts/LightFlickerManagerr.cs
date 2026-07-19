using DG.Tweening;
using UnityEngine;

public class LightFlickerManager : MonoBehaviour
{
    public static LightFlickerManager Instance;

    [SerializeField] private Light[] lights;

    private void Awake()
    {
        Instance = this;

        // ?stersen otomatik bulsun
        // lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
    }

    public void Flicker()
    {
        foreach (Light light in lights)
        {
            if (light == null) continue;

            light.DOKill();

            float start = light.intensity;

            Sequence seq = DOTween.Sequence();

            // Her ???k farkl? anda ba?las?n
            seq.PrependInterval(Random.Range(0f, 0.08f));

            seq.Append(light.DOIntensity(start * 0.15f, 0.15f).SetEase(Ease.Linear));
            seq.Append(light.DOIntensity(start, 0.12f).SetEase(Ease.Linear));

            seq.Append(light.DOIntensity(start * 0.05f, 0.10f).SetEase(Ease.Linear));
            seq.Append(light.DOIntensity(start * 1.10f, 0.13f).SetEase(Ease.Linear));

            seq.Append(light.DOIntensity(start * 0.30f, 0.12f).SetEase(Ease.Linear));
            seq.Append(light.DOIntensity(start * 0.90f, 0.15f).SetEase(Ease.Linear));

            seq.Append(light.DOIntensity(start * 0.10f, 0.08f).SetEase(Ease.Linear));
            seq.Append(light.DOIntensity(start, 0.15f).SetEase(Ease.Linear));
        }
    }
}