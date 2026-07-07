using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Coin : MonoBehaviour
{
    Vector3 oldPos;

    [SerializeField] TMP_Text _text;
    [SerializeField] Slider slider;
    [SerializeField] float scratingDis = 0.3f;
    [SerializeField] Transform boxPoint;
    float _var = 0;
    GameObject target;
    void Update()
    {
        if (boxPoint.childCount == 0)
        {
            slider.gameObject.SetActive(false);
            _text.gameObject.SetActive(false);
            _var = 0;
            target = null;
            return;
        }

        slider.gameObject.SetActive(target != null);
        _text.gameObject.SetActive(slider.value >= 1 && target != null);
        if ((transform.position - boxPoint.position).magnitude >= 1f) { target = null; _var = 0; }
        else if (_var <= 0.5f)
        {
            target = boxPoint.GetChild(0).gameObject;
            Box _box = target.GetComponent<Box>();
            _text.gameObject.SetActive(false);
            _text.text = _box.number.ToString();
        }

        
        if (target == null) { _var = 0; return; }
        if ((transform.position - oldPos).magnitude >= scratingDis) _var += 1f * Time.deltaTime;

        slider.value = _var;

        oldPos = transform.position;
    }
}
