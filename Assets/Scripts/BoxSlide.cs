using DG.Tweening;
using UnityEngine;

public class BoxSlide : MonoBehaviour
{
    float speed;
    float timer;
    void Start()
    {
        speed = CargoCoreManager.instance.BoxSpeed;
    }

    private void OnDestroy()
    {
        transform.DOKill();
        transform.eulerAngles = Vector3.zero;
    }

    void Update()
    {
        if (timer >= 1)
        {
            transform.DOShakeRotation(1f, 2f);
            timer = 0;
        }
        timer += Time.deltaTime;
        transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
    }
}
