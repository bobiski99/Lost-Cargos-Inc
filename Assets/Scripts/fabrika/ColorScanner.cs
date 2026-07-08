using UnityEngine;
using DG.Tweening;

public class ColorScanner : MonoBehaviour
{
    [Header("Ayarlar")]
    [SerializeField] private Transform _rayOrigin;
    [SerializeField] private float _rayDistance = 10f;
    [SerializeField] private float _emissionIntensity = 3f;
    [SerializeField] private float _scanRadius = 1.5f;
    
    private Material _targetSlotMaterial;
    private bool _isProcessing = false;
    private bool holding_scanner = false;

   
    void Awake()
    {

        Renderer rend = GetComponent<Renderer>();

        if (rend != null && rend.materials.Length >= 2)
        {
            Material[] mats = rend.materials;
            _targetSlotMaterial = mats[1];

            _targetSlotMaterial.EnableKeyword("_EMISSION");

            _targetSlotMaterial.SetColor("_EmissionColor", Color.black);

            rend.materials = mats;
        }
        else
        {
            Debug.LogError("Renderer veya 2. Material slotu eksik!");
        }
    }

    private void FixedUpdate()
    {
        ScanForBox();
    }

    public void ScanForBox()
    {
        if (_isProcessing || _targetSlotMaterial == null || !holding_scanner)
            return;

        Collider[] hits = Physics.OverlapSphere(_rayOrigin.position, _scanRadius);

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<Box>(out Box boxData))
            {
                if (!boxData.CanBeScanned)
                    continue;

                ApplyEmissionEffect(boxData.color);
                return;
            }
        }
    }
    private void OnMouseDown()
    {
        holding_scanner = true;
    }
    private void OnMouseUp()
    {
        holding_scanner = false;
    }
    private void OnDrawGizmosSelected()
    {
        if (_rayOrigin == null) return;

        Gizmos.color = Color.cyan;

        Gizmos.DrawLine(
            _rayOrigin.position,
            _rayOrigin.position + (-_rayOrigin.up * _rayDistance));

        Gizmos.DrawWireSphere(
            _rayOrigin.position + (-_rayOrigin.up * _rayDistance),
            _scanRadius);
    }
    private void ApplyEmissionEffect(int colorCode)
    {
        if (holding_scanner==true)
        {
            Color targetColor = GetColorFromInt(colorCode) * _emissionIntensity;
            _isProcessing = true;

            DOTween.Kill(_targetSlotMaterial);

            Sequence emissionSeq = DOTween.Sequence().SetId(_targetSlotMaterial);

            emissionSeq.Append(_targetSlotMaterial.DOColor(targetColor, "_EmissionColor", 0.3f).SetLoops(2, LoopType.Yoyo));

            emissionSeq.Append(_targetSlotMaterial.DOColor(targetColor, "_EmissionColor", 0.2f));

            emissionSeq.AppendInterval(2.0f);

            emissionSeq.Append(_targetSlotMaterial.DOColor(Color.black, "_EmissionColor", 0.5f));

            emissionSeq.OnComplete(() => {
                _isProcessing = false;
            });
        }
        
    }

    private Color GetColorFromInt(int code)
    {
        return code switch
        {
            1 => Color.green,
            2 => Color.yellow,
            3 => Color.blue,
            4 => Color.red,
            _ => Color.black
        };
    }

    private void OnDestroy()
    {
        if (_targetSlotMaterial != null) Destroy(_targetSlotMaterial);
    }
}