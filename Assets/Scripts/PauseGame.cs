using DG.Tweening;
using TMPro;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TMP_Text pauseTitle;

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioClip pauseSound;
    [SerializeField] private AudioClip resumeSound;

    [SerializeField] private float fadeDuration = .35f;

    public bool IsPaused { get; private set; }

    private Tween blinkTween;


    public bool CanPause { get; set; } = true;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        pausePanel.SetActive(false);

        if (sfxSource != null)
            sfxSource.ignoreListenerPause = true;
    }

    private void Update()
    {
        if (!CanPause)
            return;

       
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (IsPaused)
            return;

        IsPaused = true;

        pausePanel.SetActive(true);

        if (pauseTitle != null)
        {
            pauseTitle.alpha = 1f;

            blinkTween?.Kill();

            blinkTween = pauseTitle.DOFade(.2f, .6f).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
        }

        if (pauseSound != null)
            sfxSource.PlayOneShot(pauseSound);

        FadeOutAndPause(musicSource);
        FadeOutAndPause(ambienceSource);

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (!IsPaused)
            return;

        IsPaused = false;

        blinkTween?.Kill();

        if (pauseTitle != null)
            pauseTitle.alpha = 1f;

        if (resumeSound != null)
            sfxSource.PlayOneShot(resumeSound);

        Time.timeScale = 1f;

        ResumeWithFade(musicSource);
        ResumeWithFade(ambienceSource);

        pausePanel.SetActive(false);
    }

    void FadeOutAndPause(AudioSource source)
    {
        if (source == null)
            return;

        source.DOKill();

        source.DOFade(0f, fadeDuration).SetUpdate(true).OnComplete(() =>
            {
                source.Pause();
            });
    }

    void ResumeWithFade(AudioSource source)
    {
        if (source == null)
            return;

        source.DOKill();

        source.UnPause();
        source.volume = 0f;

        source.DOFade(1f, fadeDuration).SetUpdate(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}