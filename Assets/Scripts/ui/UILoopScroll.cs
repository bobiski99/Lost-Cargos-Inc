using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class UILoopScroll : MonoBehaviour
{
    [SerializeField] private Vector2 _speed = new Vector2(0.5f, 0);
    private RawImage _rawImage;

    void Awake()
    {
        _rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        // Resmin UV koordinatlarýný sürekli kaydýrýyoruz
        Rect currentUV = _rawImage.uvRect;
        currentUV.x += _speed.x * Time.deltaTime;
        currentUV.y += _speed.y * Time.deltaTime;

        _rawImage.uvRect = currentUV;
    }
}