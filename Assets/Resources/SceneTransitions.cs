using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class SceneTransitions : MonoBehaviour
{
    public static SceneTransitions Instance;

    [Header("Shutter Settings")]
    public RectTransform shutterImage;
    public float shutterDuration = 1.2f;

    [Header("Stamp Settings")]
    public Image darkBackgroundPanel; // Gamma yerine siyah/koyu renkli bir Image kullanýyoruz
    public RectTransform stampImage;
    public TextMeshProUGUI stampText;
    public TextMeshProUGUI openingInfoText;

    [Header("Transfer Data")]
    public int scoreData;
    public float timeData;
    public string textData;

    // Verileri tek seferde set etmek için yardýmcý bir metod
    public void SetSceneData(int score, float fValue, string text)
    {
        scoreData = score;
        timeData = fValue;
        textData = text;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Animation 1: Kepenk (Shutter)

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Yeni sahne yüklendiðinde otomatik açýlma animasyonunu baþlat
        PlayShutterOpen();
    }

    public void PlayShutterOpen()
    {
        // 1. Yazýyý Hazýrla
        if (!string.IsNullOrEmpty(textData))
        {
            openingInfoText.text = textData;
            openingInfoText.gameObject.SetActive(true);
            // Yazýnýn rengini ve alphasýný (görünürlüðünü) sýfýrla/tazele
            Color c = openingInfoText.color;
            openingInfoText.color = new Color(c.r, c.g, c.b, 1f);
        }

        // 2. Kepengi Hazýrla
        shutterImage.gameObject.SetActive(true);
        shutterImage.anchoredPosition = Vector2.zero;
        float endY = shutterImage.rect.height + 100f;

        Sequence openSeq = DOTween.Sequence();

        // Önce kilit sarsýntýsý, sonra yukarý fýrlama
        openSeq.Append(shutterImage.DOAnchorPosY(-20, 0.15f).SetEase(Ease.OutQuad))
            .Append(shutterImage.DOAnchorPosY(endY, shutterDuration).SetEase(Ease.InBack))

            // Kepenk tamamen yukarý çýktýktan sonra 2 saniye bekle
            .AppendInterval(1f)

            // Yazýyý 1 saniyede yavaþça yok et
            .Append(openingInfoText.DOFade(0, 1f))

            .OnComplete(() => {
                shutterImage.gameObject.SetActive(false);
                openingInfoText.gameObject.SetActive(false);
                textData = "";
            });
    }

    public void PlayShutterTransition(string sceneName)
    {
        float startY = shutterImage.rect.height + 100f;
        shutterImage.gameObject.SetActive(true);

        // Baþlangýç pozisyonunu ekranýn çok daha yukarýsýna çekiyoruz
        shutterImage.anchoredPosition = new Vector2(0, startY);
        shutterImage.rotation = Quaternion.identity;

        Sequence shutterSeq = DOTween.Sequence();

        float stuckPoint = 500f;

        shutterSeq.Append(shutterImage.DOAnchorPosY(stuckPoint, shutterDuration * 0.4f).SetEase(Ease.InCubic))
            .Join(shutterImage.DORotate(new Vector3(0, 0, 3f), shutterDuration * 0.2f)).SetEase(Ease.OutBounce)

            // 2. ADIM: Takýlma ve kurtulma çabasý (Hafif yukarý-aþaðý sarsýntý)
            .Append(shutterImage.DOAnchorPosY(stuckPoint + 40f, 0.1f)) // Hafif geri sekme
            .Append(shutterImage.DORotate(Vector3.zero, 0.15f)).SetEase(Ease.OutBack)

            .Append(shutterImage.DOAnchorPosY(0, 0.4f).SetEase(Ease.OutBounce))

            .OnComplete(() => SceneManager.LoadScene(sceneName));
    }
    #endregion

    #region Animation 2: Damga (Stamp)
    public void PlayStampTransition(string sceneName, Sprite _image, string _text)
    {
        // Hazýrlýk: Paneli aç ve tamamen saydam yap
        darkBackgroundPanel.gameObject.SetActive(true);
        darkBackgroundPanel.color = new Color(0, 0, 0, 0); // RGB = Siyah, Alpha = 0

        // Damga hazýrlýðý
        stampImage.localScale = Vector3.one * 5f;
        stampImage.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        stampImage.GetComponent<Image>().sprite = _image;
        stampText.text = "";
        string fullText = _text;

        Sequence stampSeq = DOTween.Sequence();

        // 1. Arka Plan Kararmasý
        stampSeq.Append(darkBackgroundPanel.DOFade(1f, 0.15f));

        // 2. Damga Vurma
        stampSeq.Append(stampImage.DOScale(1f, 0.3f).SetEase(Ease.InQuint))
            .Join(stampImage.GetComponent<Image>().DOFade(1f, 0.2f))
            .AppendCallback(() => {
                // Kamera yerine damganýn kendisine hafif bir sarsýntý veriyoruz (Daha garanti)
                stampImage.DOShakePosition(0.2f, 15f);
            });

        // 3. Harf Harf Yazý (DOTween.To kullanarak Pro gereksinimini bypass ediyoruz)
        stampSeq.Append(DOTween.To(() => stampText.text, x => stampText.text = x, fullText, 1.5f).SetEase(Ease.Linear));

        // 4. Sahne Deðiþimi
        stampSeq.AppendInterval(1f)
            .OnComplete(() => {
                SceneManager.LoadScene(sceneName);
                ResetStampUI();
            });
    }

    private void ResetStampUI()
    {
        // Yeni sahne yüklendiðinde UI'ý eski haline getir
        darkBackgroundPanel.color = new Color(0, 0, 0, 0);
        darkBackgroundPanel.gameObject.SetActive(false);

        stampImage.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        stampText.text = "";
    }
    #endregion
}