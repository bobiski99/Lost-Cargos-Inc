using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [SerializeField] private Image cursorImage;

    [SerializeField] private Sprite normalCursor;
    [SerializeField] private Sprite grabCursor;

    private RectTransform rect;

    private void Awake()
    {
        Instance = this;

        Cursor.visible = false;

        rect = cursorImage.rectTransform;

        cursorImage.sprite = normalCursor;
    }

    void Update()
    {
        rect.position = Input.mousePosition;
    }

    public void SetNormal()
    {
        cursorImage.sprite = normalCursor;
    }

    public void SetGrab()
    {
        cursorImage.sprite = grabCursor;
    }
}