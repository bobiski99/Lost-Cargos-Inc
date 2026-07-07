using UnityEngine;
using DG.Tweening;

public class Xray : MonoBehaviour
{
    [Header("Obje Referanslarý")]
    [SerializeField] private Transform _movingUnit;      // Aþaðý inip çýkan ilk obje
    [SerializeField] private GameObject _flashLight;     // Flaþ patlatacak ýþýk
    [SerializeField] private Transform _resultUnit;      // True gelirse yukarý çýkacak obje

    [Header("Pozisyon Ayarlarý")]
    [SerializeField] private Vector3 _unitDownPos;       // Ünitenin ineceði alt nokta
    [SerializeField] private Vector3 _resultUpPos;       // Sonuç ünitesinin çýkacaðý nokta
    private Vector3 _unitStartPos;                       // Ünitenin orijinal pozisyonu
    private Vector3 _resultStartPos;                     // Sonuç ünitesinin orijinal pozisyonu

    [Header("Kontrol ve Zamanlama")]
    public bool isResultPositive = false;                // Kontrol edilecek bool
    [SerializeField] private float _moveSpeed = 1f;

    private bool _isBusy = false;                        // Çakýþmayý önlemek için kilit

    void Awake()
    {
        // Baþlangýç pozisyonlarýný kaydet
        if (_movingUnit) _unitStartPos = _movingUnit.localPosition;
        if (_resultUnit) _resultStartPos = _resultUnit.localPosition;

        // Iþýðý baþta kapat
        if (_flashLight) _flashLight.SetActive(false);
    }

    private void OnMouseDown()
    {
        // Eðer sistem þu an çalýþýyorsa yeni týklamayý reddet
        if (_isBusy) return;
        StartMechanism();
    }

    public void StartMechanism()
    {
        if (CargoCoreManager.instance.pause) return;
        _isBusy = true;
        Sequence seq = DOTween.Sequence();

        // 1. Obje aþaðý iniyor
        seq.Append(_movingUnit.DOLocalMove(_unitDownPos, _moveSpeed).SetEase(Ease.InOutElastic));

        // 2. Flaþ patlatma (Aç-Kapa-Aç-Kapa)
        seq.AppendCallback(() => _flashLight.SetActive(true));
        seq.AppendInterval(0.1f);
        seq.AppendCallback(() => _flashLight.SetActive(false));
        seq.AppendInterval(0.1f);
        seq.AppendCallback(() => _flashLight.SetActive(true));
        seq.AppendInterval(0.1f);
        seq.AppendCallback(() => _flashLight.SetActive(false));

        // 3. Obje geri eski konumuna çýkýyor
        seq.Append(_movingUnit.DOLocalMove(_unitStartPos, _moveSpeed).SetEase(Ease.InOutElastic));

        // 4. Hareket bittikten sonra Bool kontrolü
        seq.OnComplete(() => {
            CheckResult();
        });
    }

    private void CheckResult()
    {
        isResultPositive = CargoCoreManager.instance.Danger;
        if (isResultPositive)
        {
            Debug.Log("<color=green>SONUÇ POZÝTÝF!</color> Obje yükseliyor...");

            _resultUnit.gameObject.SetActive(true);
            _resultUnit.DOLocalMove(_resultUpPos, _moveSpeed).SetEase(Ease.OutBack)
                .OnComplete(() => _isBusy = false);
        }
        else
        {
            Debug.Log("<color=red>kaybetti</color>");
            _isBusy = false;
        }
    }
}