using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class OutlineController : MonoBehaviour
{
    [Header("Gorsel Ayarlar")]
    [SerializeField] private Material outlineMaterial;

    private Renderer _mainRenderer;
    private GameObject _outlineObj;
    private Renderer _outlineRenderer;

    [Header("Kontrol")]
    [SerializeField] private bool _isOutlineActive = false;

    public bool IsOutlineActive
    {
        get => _isOutlineActive;
        set
        {
            if (_isOutlineActive != value)
            {
                _isOutlineActive = value;
                UpdateOutlineState();
            }
        }
    }

    void Awake()
    {
        _mainRenderer = GetComponent<Renderer>();
        CreateOutlineObject();
    }

    private void CreateOutlineObject()
    {
        // Eski bir outline varsa temizle
        if (transform.Find("OutlineObject")) return;

        _outlineObj = new GameObject("OutlineObject");
        _outlineObj.transform.SetParent(transform);
        _outlineObj.transform.localPosition = Vector3.zero;
        _outlineObj.transform.localRotation = Quaternion.identity;
        _outlineObj.transform.localScale = Vector3.one;

        // Mesh yapýsýný kopyala (MeshFilter veya SkinnedMeshRenderer uyumu)
        if (_mainRenderer is MeshRenderer)
        {
            _outlineObj.AddComponent<MeshFilter>().sharedMesh = GetComponent<MeshFilter>().sharedMesh;
            _outlineRenderer = _outlineObj.AddComponent<MeshRenderer>();
        }
        else if (_mainRenderer is SkinnedMeshRenderer smr)
        {
            var outlineSmr = _outlineObj.AddComponent<SkinnedMeshRenderer>();
            outlineSmr.sharedMesh = smr.sharedMesh;
            outlineSmr.rootBone = smr.rootBone;
            outlineSmr.bones = smr.bones;
            _outlineRenderer = outlineSmr;
        }

        // Sub-mesh sayýsý kadar outline materyali ata
        int subMeshCount = _mainRenderer.sharedMaterials.Length;
        Material[] muls = new Material[subMeshCount];
        for (int i = 0; i < subMeshCount; i++)
        {
            muls[i] = outlineMaterial;
        }
        _outlineRenderer.sharedMaterials = muls;
        _outlineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        _outlineObj.SetActive(_isOutlineActive);
    }

    private void UpdateOutlineState()
    {
        if (_outlineObj != null)
        {
            _outlineObj.SetActive(_isOutlineActive);
        }
    }

    // Editörde materyal deðiþirse anýnda güncelleme yapabilmek için
    void OnValidate()
    {
        if (_outlineRenderer != null && outlineMaterial != null)
        {
            UpdateOutlineState();
        }
    }
}