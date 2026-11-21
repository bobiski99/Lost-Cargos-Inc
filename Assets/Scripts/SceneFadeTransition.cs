
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFadeTransition : MonoBehaviour
{
    public static SceneFadeTransition Instance;
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    private bool isTransitioning;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
        fadeCanvasGroup.interactable = false;
    }

    public void LoadSceneWithFade(string sceneName, float fadeOutDuration, float fadeInDuration)
    {
        if (isTransitioning)
            return;

        StartCoroutine(Transition(sceneName, fadeOutDuration, fadeInDuration));
    }

    private IEnumerator Transition(string sceneName, float fadeOutDuration, float fadeInDuration)
    {
        isTransitioning = true;

        // Eski sahneyi 1.5 saniyede karart
        yield return Fade(1f, fadeOutDuration);

        // Yeni sahneyi yükle
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
            yield return null;

        // Yeni sahnenin çizilmesini bekle
        yield return new WaitForEndOfFrame();

        // Yeni sahneyi 0.5 saniyede aç
        yield return Fade(0f, fadeInDuration);

        isTransitioning = false;
    }

    private IEnumerator Fade(float targetAlpha, float duration)
    {
        fadeCanvasGroup.blocksRaycasts = true;

        float startAlpha = fadeCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            fadeCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                Mathf.Clamp01(elapsed / duration)
            );

            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;

        if (targetAlpha == 0f)
            fadeCanvasGroup.blocksRaycasts = false;
    }
}
