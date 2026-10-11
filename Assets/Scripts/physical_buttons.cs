using UnityEngine;
public enum MenuButtonType
{
    Play,
    Settings,
    Credits,
    Exit
}
public class physical_buttons : MonoBehaviour
{
    public MenuButtonType buttonType;
}