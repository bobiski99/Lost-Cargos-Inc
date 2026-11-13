
using UnityEngine;
using DG.Tweening;

public enum MenuButtonType
{
    Play,
    Credits,
    Exit
}

public class physical_buttons : MonoBehaviour
{
    public MenuButtonType buttonType;
    [SerializeField] private Color glowColor = new Color(1f, 0.75f, 0.3f, 0.15f);
    [SerializeField] private float glowIntensity = 0.8f;
    [SerializeField] private bool liftOnHover = false;
    [SerializeField] private float hoverOffsetY = 0.567f;
    [SerializeField] private float hoverDuration = 0.25f;
    private Renderer[] renderers;
    private Material[][] materials;
    private Color[][] originalEmissionColors;
    private bool[][] hasEmission;
    private Vector3 startLocalPosition;
    private Tween movementTween;
    private static readonly int EmissionColor =
        Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);

        materials = new Material[renderers.Length][];
        originalEmissionColors = new Color[renderers.Length][];
        hasEmission = new bool[renderers.Length][];
        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].materials;
            originalEmissionColors[i] = new Color[materials[i].Length];
            hasEmission[i] = new bool[materials[i].Length];
            for (int j = 0; j < materials[i].Length; j++)
            {
                Material mat = materials[i][j];
                if (mat.HasProperty(EmissionColor))
                {
                    mat.EnableKeyword("_EMISSION");
                    originalEmissionColors[i][j] =
                        mat.GetColor(EmissionColor);
                    hasEmission[i][j] = true;
                }
            }
        }
        startLocalPosition = transform.localPosition;
    }

    public void SetHovered(bool hovered)
    {
        SetGlow(hovered);
        SetMovement(hovered);
    }

    private void SetGlow(bool enabled)
    {
        for (int i = 0; i < materials.Length; i++)
        {
            for (int j = 0; j < materials[i].Length; j++)
            {
                if (!hasEmission[i][j])
                {
                    continue;
                }
                Material mat = materials[i][j];
                Color original = originalEmissionColors[i][j];
                Color target = glowColor * glowIntensity;
                Color result = enabled ? Color.Lerp(original, target, glowColor.a) : original;
                mat.SetColor(EmissionColor, result);
            }
        }
    }

    private void SetMovement(bool hovered)
    {
        if (!liftOnHover)
            return;

        movementTween?.Kill();

        Vector3 targetPosition = startLocalPosition;

        if (hovered)
        {
            targetPosition.y += hoverOffsetY;
        }

        movementTween = transform.DOLocalMove(targetPosition,hoverDuration).SetEase(Ease.OutSine);
    }

    private void OnDestroy()
    {
        movementTween?.Kill();
    }
}
