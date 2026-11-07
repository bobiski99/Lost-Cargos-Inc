using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CargoCoreManager : MonoBehaviour
{
    public static CargoCoreManager instance;

    [SerializeField] private CameraSway cameraSway;

    public float BoxSpeed = 5f;
    public int Score;
    public float timer;
    public int healt = 3;
    public RawImage[] healtIcons;
    [SerializeField] private Color normalHealthColor = new Color32(0x3B, 0xFF, 0x40, 0xFF);
    [SerializeField] private Color criticalHealthColor = Color.red;
    [SerializeField] private CigarettePack cigarettePack;
    public bool Danger = false;
    public TextMeshProUGUI scoreText;
    private int _currentDisplayedScore = 0;
    private Tween _scoreTween;

    private Tween healthBlinkTween;
    [SerializeField] private TMP_Text damageReasonText;
    [SerializeField] private int blinkCount = 4;
    [SerializeField] private float blinkInterval = 0.1f;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float fadeOutDuration = 0.4f;
    private Queue<string> damageMessages = new Queue<string>();
    private bool isShowingDamageMessage = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {

            instance = this;
        }
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
        if (damageReasonText != null)
        {
            damageReasonText.alpha = 0f;
        }
        UpdateScore();
    }

    private void Update()
    {

        timer += Time.deltaTime;
    }
    public void ShowDamageReason(string reason)
    {
        if (damageReasonText == null)
        {
            return;
        }

        damageMessages.Enqueue(reason);

        if (!isShowingDamageMessage)
        {
            StartCoroutine(ProcessDamageMessages());
        }
    }

    private IEnumerator ProcessDamageMessages()
    {
        isShowingDamageMessage = true;

        while (damageMessages.Count > 0)
        {
            string message = damageMessages.Dequeue();

            damageReasonText.text = message;
            damageReasonText.alpha = 0f;
            for (int i = 0; i < blinkCount; i++)
            {
                damageReasonText.alpha = 1f;
                yield return new WaitForSecondsRealtime(blinkInterval);
                damageReasonText.alpha = 0f;
                yield return new WaitForSecondsRealtime(blinkInterval);
            }
            damageReasonText.alpha = 1f;
            yield return new WaitForSecondsRealtime(displayDuration);
            float elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                damageReasonText.alpha = Mathf.Lerp(1f,0f,elapsed / fadeOutDuration);
                yield return null;
            }
            damageReasonText.alpha = 0f;
        }

        isShowingDamageMessage = false;
    }
    public void takeDamage(int damage = 1, string reason = "BOX IN THE OVEN!")
    {
        if (combomanager.Instance != null)
        {
            combomanager.Instance.ResetCombo();
        }
        if (healt == 0) return;
        ShowDamageReason(reason);
        for (int i = 0; i < damage; i++)
        {
            if (healt == 0)
            {
                break;
            }
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
            PauseManager.Instance.PauseGame();
            Debug.Log("GAME OVER - Score: " + Score);
        }
    }

    public void Heal(int amount = 1)
    {

        for (int i = 0; i < amount; i++)
        {
            if (healt >= healtIcons.Length)
            {

                break;
            }
            

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
        float multiplier = 1f;

        if (combomanager.Instance != null)
        {

            multiplier = combomanager.Instance.Multiplier;
        }

        int finalPoint = Mathf.RoundToInt(point * multiplier);

        Score += finalPoint;
        UpdateScore();
    }

    public void UpdateScore()
    {
        if (ToastyCat.Instance != null)
        {
            ToastyCat.Instance.CheckScore(Score);
        }
        _scoreTween?.Kill();

        _scoreTween = DOTween.To(() => _currentDisplayedScore,  x => {_currentDisplayedScore = x;
                scoreText.text = _currentDisplayedScore.ToString();},Score,1.5f).SetEase(Ease.OutQuad);
    }

    public void PlayFlickerAndDestroy(RawImage img)
    {

        img.DOFade(0, 0.2f).SetLoops(6, LoopType.Yoyo).OnComplete(() =>{img.gameObject.SetActive(false);});
    }
}