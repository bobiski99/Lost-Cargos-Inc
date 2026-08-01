using DG.Tweening;
using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.GlobalIllumination;

public class FireFlicker : MonoBehaviour
{
    private Light _light;


    [Header("Intensity Settings")]
    public float minIntensity = 0.7f;
    public float maxIntensity = 1.3f;

    [Header("Speed Settings")]
    public float minDuration = 0.05f;
    public float maxDuration = 0.2f;

    void Start()
    {
        _light = GetComponent<Light>();
        if (_light == null)
        {
            return;
        }
        Flicker();
    }
    void Flicker()
    {
        float targetIntensity = Random.Range(minIntensity, maxIntensity);
        float duration = Random.Range(minDuration, maxDuration);

        _light.DOIntensity(targetIntensity, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(Flicker);
    }

    private void OnDestroy()
    {
        _light.DOKill();
    }
}