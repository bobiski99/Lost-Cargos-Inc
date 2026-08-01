using DG.Tweening;
using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.GlobalIllumination;

public class burning_light : MonoBehaviour
{
    [SerializeField] private Light pointLight;
    private Light _light;

    private Coroutine routine;
    public float minIntensity = 0.7f;
    public float maxIntensity = 1.3f;

    private float defaultMin;
    private float defaultMax;

    public float minDuration = 0.05f;
    public float maxDuration = 0.2f;

    void Start()
    {
        defaultMax = maxIntensity;
        defaultMin = minIntensity;
        _light = GetComponent<Light>();
        if (_light == null)
        {
            return;
        }
        pointLight.enabled = false;
        Flicker();

    }
    public void TurnOnForSeconds(float seconds)
    {
        pointLight.enabled = true;

        minIntensity = defaultMin;
        maxIntensity = defaultMax;
        DOVirtual.DelayedCall(seconds, () =>
        {
            DOTween.To(() => minIntensity,x => minIntensity = x,0f,1f);
            DOTween.To(() => maxIntensity,x => maxIntensity = x,0f,1f).OnComplete(() =>
            {
                pointLight.enabled = false;
            });
        });
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