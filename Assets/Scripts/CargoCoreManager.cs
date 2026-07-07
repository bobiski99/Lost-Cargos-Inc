using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CargoCoreManager : MonoBehaviour
{
    public static CargoCoreManager instance;

    public float BoxSpeed = 5f;
    public int Score;
    public float timer;

    public int healt = 3;
    public RawImage[] healtIcons;
    public DeliverButton deliverbtn;

    public bool Danger = false;
    public bool pause = false;


    public Sprite standarDead;

    public TextMeshProUGUI scoreText;
    private int _currentDisplayedScore = 0;
    private Tween _scoreTween;



    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else instance = this;

       

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

    public void takeDamage()
    {
        if (pause) return;
        if (healt == 0) return;
        healt--;
        PlayFlickerAndDestroy(healtIcons[healt]);

        if (healt == 0)
        {
            pause = true;
            Debug.Log("GAME OVER - Score: " + Score);// editor'da test için
        }
    }

    public void GoDanger()
    {
        deliverbtn.ActivateObject();
    }

    public void GivePoint(int point)
    {
        Score += point;
        UpdateScore();
    }

    public void UpdateScore()
    {
        if (_scoreTween != null) _scoreTween.Kill();

        _scoreTween = DOTween.To(() => _currentDisplayedScore, x => {
            _currentDisplayedScore = x;
            scoreText.text = _currentDisplayedScore.ToString();
        }, Score, 1.5f).SetEase(Ease.OutQuad);
    }

    public void PlayFlickerAndDestroy(RawImage _img)
    {
        if (pause) return;
        _img.DOFade(0, 0.2f).SetLoops(6, LoopType.Yoyo).OnComplete(() =>
        {
            Destroy(_img.gameObject);
        });
    }
}
