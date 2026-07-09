using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ScannerScreen : MonoBehaviour
{
    [Header("Material")]
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private int materialIndex = 1;

    [Header("Textures")]
    public Texture readyTexture;
    public Texture[] scanningTextures;
    public Texture cleanTexture;
    public Texture dangerTexture;

    [Header("Emission")]
    public Color normalEmission = Color.white;
    public Color dangerEmission = Color.red;

    public float emissionIntensity = 3f;

    private Material mat;
    private Coroutine scanRoutine;

    [SerializeField] private Transform xrayGrid;


    void Awake()
    {
        mat = targetRenderer.materials[materialIndex];
        SetTexture(readyTexture, normalEmission, 2f);
    }

    public void StartScan(Box box)
    {
        if (scanRoutine != null)
            StopCoroutine(scanRoutine);

        scanRoutine = StartCoroutine(ScanRoutine(box));
    }

    public void StopScan()
    {
        if (scanRoutine != null)
        {
            StopCoroutine(scanRoutine);
        }
        DOVirtual.DelayedCall(1f, () =>
        {
            SetTexture(readyTexture, normalEmission, 2f);
        });

        xrayGrid.DOKill();

        xrayGrid.localPosition = new Vector3(xrayGrid.localPosition.x, -0.92f,xrayGrid.localPosition.z);
    }

    IEnumerator ScanRoutine(Box box)
    {
        xrayGrid.DOKill();

        Sequence gridSeq = DOTween.Sequence();

        gridSeq.Append(xrayGrid.DOLocalMoveY(0.005f, 0.5f).SetEase(Ease.Linear));

        gridSeq.Append(xrayGrid.DOLocalMoveY(-0.92f, 0.5f).SetEase(Ease.Linear));
        float timer = 0;
        while (timer < 1f)
        {
            foreach (Texture tex in scanningTextures)
            {
                SetTexture(tex, normalEmission, 2f);

                yield return new WaitForSeconds(0.25f);

                timer += 0.25f;

                if (timer >= 1f)
                    break;
            }
        }

        if (box.Danger)
        {
            SetTexture(dangerTexture, dangerEmission, emissionIntensity);
        }
        else
        {
            SetTexture(cleanTexture, normalEmission, 2f);
        }
        scanRoutine = null;
    }

    void SetTexture(Texture tex, Color emissionColor, float intensity = 2f)
    {
        mat.SetTexture("_BaseMap", tex);
        mat.SetTexture("_EmissionMap", tex);
        mat.SetColor("_EmissionColor", emissionColor * intensity);
    }
}