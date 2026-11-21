
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
        rect = cursorImage.rectTransform;
        cursorImage.sprite = normalCursor;
        Cursor.visible = false;
    }

    private void Update()
    {
        Cursor.visible = false;
        rect.position = Input.mousePosition;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.visible = false;
        }
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