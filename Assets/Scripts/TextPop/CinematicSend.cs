using UnityEngine;

public class CinematicSend : MonoBehaviour
{
    public void MessageSender(string Text)
    {
        MessageManager.Instance.ShowMessage(Text);
    }
}
