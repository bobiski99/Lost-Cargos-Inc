using System.Collections.Generic;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    [Header("Görsel Ayarlar")]
    [SerializeField] private Material outlineMaterial;

    private readonly List<GameObject> outlineObjects = new();

    [Header("Kontrol")]
    [SerializeField] private bool _isOutlineActive;

    public bool IsOutlineActive
    {
        get => _isOutlineActive;
        set
        {
            if (_isOutlineActive == value) return;

            _isOutlineActive = value;
            UpdateOutlineState();
        }
    }

    void Awake()
    {
        CreateOutlineObjects();
    }

    void CreateOutlineObjects()
    {
        outlineObjects.Clear();

        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(true);

        foreach (MeshRenderer rend in renderers)
        {
            MeshFilter mf = rend.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null)
                continue;

            if (rend.transform.Find("OutlineObject") != null)
                continue;

            GameObject outline = new GameObject("OutlineObject");
            outline.transform.SetParent(rend.transform, false);
            outline.transform.localPosition = Vector3.zero;
            outline.transform.localRotation = Quaternion.identity;
            outline.transform.localScale = Vector3.one;

            MeshFilter outlineMF = outline.AddComponent<MeshFilter>();
            outlineMF.sharedMesh = mf.sharedMesh;

            MeshRenderer outlineMR = outline.AddComponent<MeshRenderer>();

            Material[] mats = new Material[rend.sharedMaterials.Length];
            for (int i = 0; i < mats.Length; i++)
                mats[i] = outlineMaterial;

            outlineMR.sharedMaterials = mats;
            outlineMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            outlineMR.receiveShadows = false;

            outline.SetActive(_isOutlineActive);

            outlineObjects.Add(outline);
        }
    }

    void UpdateOutlineState()
    {
        foreach (GameObject obj in outlineObjects)
        {
            if (obj != null)
                obj.SetActive(_isOutlineActive);
        }
    }

    void OnValidate()
    {
        UpdateOutlineState();
    }

    void OnMouseEnter()
    {
        IsOutlineActive = true;
    }

    void OnMouseExit()
    {
        IsOutlineActive = false;
    }
}