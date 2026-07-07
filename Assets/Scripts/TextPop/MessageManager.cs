using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance;

    [Header("UI References")]
    public GameObject messagePanel;
    public TMP_Text messageText;

    [Header("Settings")]
    public float typingSpeed = 0.05f;  // Harf baþýna bekleme süresi
    public float extraReadTimePerCharacter = 0.05f; // Okuma süresi (karakter baþýna)

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip charSound;
    public AudioClip exclamationSound;

    [Header("Shake Settings")]
    public float shakeDuration = 0.5f;
    public float shakeStrength = 20f;
    public int shakeVibrato = 10;

    private Coroutine currentMessageRoutine;

    void Awake()
    {
        // Singleton yapý
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        messagePanel.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        if (currentMessageRoutine != null)
            StopCoroutine(currentMessageRoutine);

        currentMessageRoutine = StartCoroutine(ShowMessageRoutine(message));
    }

    private IEnumerator ShowMessageRoutine(string message)
    {
        messagePanel.SetActive(true);
        messageText.text = "";

        // Yazýyý harf harf yaz
        foreach (char c in message)
        {

            messageText.text += c;

            if (c != ' ' && c != '_')
            {
                if (charSound != null)
                {
                    audioSource.PlayOneShot(charSound);
                }
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        if (message.EndsWith("!") && exclamationSound != null)
        {
            audioSource.PlayOneShot(exclamationSound);

            // DOTween shake
            messageText.transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato);
        }

        // Okuma süresi = karakter sayýsýna göre
        float totalReadTime = message.Length * extraReadTimePerCharacter;
        yield return new WaitForSeconds(totalReadTime);

        messagePanel.SetActive(false);
        currentMessageRoutine = null;
    }
}
