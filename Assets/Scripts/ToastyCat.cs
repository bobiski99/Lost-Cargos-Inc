using DG.Tweening;
using UnityEngine;

public class ToastyCat : MonoBehaviour
{
    public static ToastyCat Instance;

    [SerializeField] private Transform cat;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip toastyClip;
    [SerializeField] private int scoreThreshold = 500;
    [SerializeField] private bool repeatEveryThreshold = false;
    [SerializeField] private float hiddenY = -1.44f;
    [SerializeField] private float shownY = 1.159f;
    [SerializeField] private float slideTime = 0.3f;
    [SerializeField] private float stayTime = 2f;

    private int _nextThreshold;
    private bool _playing;

    void Awake()
    {
        Instance = this;
        _nextThreshold = scoreThreshold;

        if (cat != null)
        {
            Vector3 p = cat.position;
            p.y = hiddenY;
            cat.position = p;
        }
    }

    public void CheckScore(int score)
    {
        if (_playing || score < _nextThreshold) return;

        _nextThreshold = repeatEveryThreshold ? _nextThreshold + scoreThreshold : int.MaxValue;







        Play();
    }

    void Play()
    {
        _playing = true;

        if (audioSource != null && toastyClip != null)
        {

            audioSource.PlayOneShot(toastyClip);
        }

        cat.DOKill();
        DOTween.Sequence().Append(cat.DOMoveY(shownY, slideTime)).AppendInterval(stayTime).Append(cat.DOMoveY(hiddenY, slideTime)).OnComplete(() => _playing = false);
    }
}