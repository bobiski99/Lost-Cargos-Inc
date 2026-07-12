using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class discoball_sc : MonoBehaviour
{
    [SerializeField] private Light[] lights;
    [SerializeField] private AudioClip disco_music;

    private AudioSource musicSource;

    private Tween currentTween;
    private void Awake()
    {
        musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.playOnAwake = false;
        musicSource.clip = disco_music;
    }
    public void discotime()
    {
        musicSource.Stop();
        currentTween?.Kill();
        transform.DOKill();
        transform.DOMoveY(3.58f, 1.9f);
        DOVirtual.DelayedCall(2f, () =>
        {
            musicSource.volume = 1f;
            musicSource.Play();
            foreach (Light l in lights)
            {
                l.enabled = true;
                l.intensity = 18f;
            }
        });
        DOVirtual.DelayedCall(11f, () =>
        {
            transform.DOMoveY(5.214f, 1f).SetEase(Ease.OutBack);
            
        });
        DOVirtual.DelayedCall(12f, () =>
        {
            foreach (Light l in lights)
            {
                l.DOIntensity(0f, 0.5f);
            }
            musicSource.DOFade(0f, 1f).OnComplete(() =>
            {
                musicSource.Stop();
            });
        });
    }
}
