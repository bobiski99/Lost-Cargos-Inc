using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RadioVisualizer : MonoBehaviour
{
    public Transform visualTarget; // dýþ model
    public float scaleMultiplier = 5f;
    public float lerpSpeed = 10f;
    public FFTWindow fftWindow = FFTWindow.Blackman;
    public int spectrumSampleIndex = 0;

    private AudioSource audioSource;
    private float[] spectrum = new float[64];
    private Vector3 originalScale;
    private Vector3 originalPosition;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        originalScale = visualTarget.localScale;
        originalPosition = visualTarget.localPosition;
    }

    void Update()
    {
        audioSource.GetSpectrumData(spectrum, 0, fftWindow);
        float intensity = spectrum[spectrumSampleIndex] * scaleMultiplier;
        intensity = Mathf.Clamp(intensity, 0f, 1f);

        // Yeni scale hesapla
        Vector3 targetScale = originalScale * (1f + intensity);
        visualTarget.localScale = Vector3.Lerp(
            visualTarget.localScale,
            targetScale,
            Time.deltaTime * lerpSpeed
        );

        // Yükseklik farkýný telafi et
        float yOffset = (visualTarget.localScale.y - originalScale.y) * 0.5f;
        Vector3 newPos = originalPosition + Vector3.up * yOffset;

        visualTarget.localPosition = Vector3.Lerp(
            visualTarget.localPosition,
            newPos,
            Time.deltaTime * lerpSpeed
        );
    }
}
