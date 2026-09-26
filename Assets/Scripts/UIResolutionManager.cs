using UnityEngine;

public class UIResolutionManager : MonoBehaviour
{
    public static UIResolutionManager Instance;

    [Header("Reference")]
    [SerializeField] private Vector2 referenceResolution = new Vector2(1920f, 1080f);

    [Header("Cursor")]
    [SerializeField] private RectTransform cursor;
    [SerializeField] private float referenceCursorScale = 1f;

    [Header("Scaling")]
    [SerializeField] private float minScale = 0.75f;
    [SerializeField] private float maxScale = 1.5f;

    public float UIScale { get; private set; } = 1f;

    private int lastWidth;
    private int lastHeight;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        lastWidth = Screen.width;
        lastHeight = Screen.height;

        UpdateScale();
    }

    private void Update()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;

            UpdateScale();
        }
    }

    private void UpdateScale()
    {
        float widthScale = Screen.width / referenceResolution.x;
        float heightScale = Screen.height / referenceResolution.y;

        // Ekran?n daha küçük boyutunu baz al?yoruz.
        // Böylece ultrawide ekranlarda UI gereksiz büyümez.
        float scale = Mathf.Min(widthScale, heightScale);

        UIScale = Mathf.Clamp(scale, minScale, maxScale);

        UpdateCursor();
    }

    private void UpdateCursor()
    {
        if (cursor == null)
            return;

        cursor.localScale = Vector3.one * (referenceCursorScale * UIScale);
    }

    public float GetScale()
    {
        return UIScale;
    }
}