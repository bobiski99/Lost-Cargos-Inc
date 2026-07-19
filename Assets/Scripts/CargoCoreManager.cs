using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CargoCoreManager : MonoBehaviour
{
    public static CargoCoreManager instance;

    [SerializeField] private CameraSway cameraSway;

    public float BoxSpeed = 5f;
    public int Score;
    public float timer;

    [Header("Health")]
    public int healt = 3;
    public RawImage[] healtIcons;

    [SerializeField] private Color normalHealthColor = new Color32(0x3B, 0xFF, 0x40, 0xFF);
    [SerializeField] private Color criticalHealthColor = Color.red;
    [SerializeField] private CigarettePack cigarettePack;
    public bool Danger = false;
    public bool pause = false;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    private int _currentDisplayedScore = 0;
    private Tween _scoreTween;

    private Tween healthBlinkTween;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        if (SceneTransitions.Instance != null)
        {
            Score = SceneTransitions.Instance.scoreData;
            timer = SceneTransitions.Instance.timeData;
        }
        else
        {
            Score = 0;
            timer = 0;
        }

        UpdateScore();
    }

    private void Update()
    {
        if (pause) return;

        timer += Time.deltaTime;
    }

    public void takeDamage(int damage = 1)
    {
        if (pause) return;
        if (healt == 0) return;

        for (int i = 0; i < damage; i++)
        {
            if (healt == 0)
                break;

            healt--;

            PlayFlickerAndDestroy(healtIcons[healt]);

            LightFlickerManager.Instance.Flicker();
            cameraSway.DamageShake();

            if (healt == 1)
            {
                StartCriticalHealthEffect();
                cigarettePack.ShowPack();
            }

        }

        if (healt == 0)
        {
            pause = true;
            Debug.Log("GAME OVER - Score: " + Score);
        }
    }
    
    public void Heal(int amount = 1)
    {
        if (pause) return;

        for (int i = 0; i < amount; i++)
        {
            if (healt >= healtIcons.Length)
                break;

            healtIcons[healt].gameObject.SetActive(true);
            healt++;
        }

        if (healt > 1)
        {
            StopCriticalHealthEffect();
        }
    }

    void StartCriticalHealthEffect()
    {
        healthBlinkTween?.Kill();

        healtIcons[0].color = criticalHealthColor;

        healthBlinkTween = healtIcons[0].DOColor(normalHealthColor, 0.18f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }

    void StopCriticalHealthEffect()
    {
        healthBlinkTween?.Kill();

        healtIcons[0].color = normalHealthColor;
    }


    public void GivePoint(int point)
    {
        Score += point;
        UpdateScore();
    }

    public void UpdateScore()
    {
        _scoreTween?.Kill();

        _scoreTween = DOTween.To(
            () => _currentDisplayedScore,
            x =>
            {
                _currentDisplayedScore = x;
                scoreText.text = _currentDisplayedScore.ToString();
            },
            Score,
            1.5f
        ).SetEase(Ease.OutQuad);
    }

    public void PlayFlickerAndDestroy(RawImage img)
    {
        if (pause) return;

        img.DOFade(0, 0.2f)
            .SetLoops(6, LoopType.Yoyo)
            .OnComplete(() =>
            {
                img.gameObject.SetActive(false);
            });
    }
}