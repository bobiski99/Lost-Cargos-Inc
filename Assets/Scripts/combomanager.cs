
using DG.Tweening;
using TMPro;
using UnityEngine;
using System.Globalization;

public class combomanager : MonoBehaviour
{
    public static combomanager Instance;
    [SerializeField] private int comboStartCount = 3;
    [SerializeField] private float multiplierStep = 0.1f;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private float animationDuration = 0.25f;
    [SerializeField] private float baseRotationSpeed = 0.65f;
    [SerializeField] private float rotationSpeedIncrease = 0.10f;
    [SerializeField] private float baseRotationAngle = 1f;
    [SerializeField] private float rotationAngleIncrease = 3f;
    [SerializeField] private float rainbowSpeed = 1.2f;
    public int SuccessfulDrops 
    {
        get; 
        private set; 
    }
    public float Multiplier { get; private set; } = 1f;
    private Tween rotationTween;
    private Tween rainbowTween;
    private Tween scaleTween;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private float rainbowHue;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (comboText != null)
        {
            originalScale = comboText.transform.localScale;
            originalRotation = comboText.transform.localRotation;



            comboText.gameObject.SetActive(false);
        }
    }

    public void RegisterSuccessfulDelivery()
    {
        SuccessfulDrops++;


        if (SuccessfulDrops < comboStartCount)
        {
            return;
        }

        Multiplier = 1f +(SuccessfulDrops - comboStartCount + 1) * multiplierStep;

        ShowCombo();
    }

    private void ShowCombo()
    {
        if (comboText == null) 
        {
            return;
        } 
        KillAnimations();

        comboText.gameObject.SetActive(true);

        comboText.text = "COMBO x"+Multiplier.ToString("0.0", CultureInfo.InvariantCulture);










        comboText.transform.localScale = Vector3.zero;
        comboText.transform.localRotation = originalRotation;

        UpdateComboColor();
        StartRotationEffect();
        scaleTween = comboText.transform.DOScale(originalScale * 1.5f, animationDuration).SetEase(Ease.OutBack).OnComplete(() =>
            {
                scaleTween = comboText.transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutElastic);
            });
    }

    private void UpdateComboColor()
    {
        if (comboText == null) return;

        rainbowTween?.Kill();
        rainbowTween = null;

        float colorStage = Mathf.Floor((Multiplier - 1.1f) / 0.5f);
        if (Multiplier < 1.5f)
        {
            comboText.color = new Color(0.15f, 0.55f, 1f);
        }
        else if (Multiplier < 2f)
        {
            comboText.color = new Color(1f, 0.85f, 0.1f);
        }
        else if (Multiplier < 2.5f)
        {
            comboText.color = new Color(1f, 0.38f, 0.05f);
        }
        else if (Multiplier < 3f)
        {
            comboText.color = new Color(1f, 0.08f, 0.08f);
        }
        else if (Multiplier < 3.5f)
        {
            comboText.color = new Color(0.65f, 0.2f, 1f);
        }
        else
        {
            StartRainbowEffect();
        }
    }

    private void StartRainbowEffect()
    {
        rainbowHue = 0f;

        rainbowTween = DOTween.To(() => rainbowHue,x =>
            {
                rainbowHue = x;
                comboText.color = Color.HSVToRGB(x, 0.9f, 1f);
            }
            ,1f,rainbowSpeed).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }

    private void StartRotationEffect()
    {
        float stage = Mathf.Max(0f,Mathf.Floor((Multiplier - 1.1f) / 0.5f));
        float rotationDuration = Mathf.Max(0.12f,baseRotationSpeed - stage * rotationSpeedIncrease);
        float rotationAngle = baseRotationAngle +stage * rotationAngleIncrease;

        float originalZ = originalRotation.eulerAngles.z;

        rotationTween = comboText.transform.DOLocalRotate(new Vector3(originalRotation.eulerAngles.x,originalRotation.eulerAngles.y,originalZ + rotationAngle)
            ,rotationDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public void ResetCombo()
    {
        if (SuccessfulDrops == 0)
        {
            return;
        }

        bool hadCombo = SuccessfulDrops >= comboStartCount;

        SuccessfulDrops = 0;
        Multiplier = 1f;

        if (!hadCombo || comboText == null)
        {
            return;
        }

        KillAnimations();

        comboText.text = "COMBO LOST";
        comboText.color = Color.red;

        comboText.transform.localRotation = originalRotation;

        scaleTween = comboText.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InBack).OnComplete(() =>
            {
                comboText.gameObject.SetActive(false);
                comboText.transform.localScale = originalScale;
                comboText.transform.localRotation = originalRotation;
            });
    }

    private void KillAnimations()
    {
        rotationTween?.Kill();
        rainbowTween?.Kill();
        scaleTween?.Kill();

        if (comboText != null)
        {

            comboText.transform.DOKill();
        }
    }

    private void OnDestroy()
    {
        KillAnimations();

        if (Instance == this)
        {
            Instance = null;
        }
    }
}
