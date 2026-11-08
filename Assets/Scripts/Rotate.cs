using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 rotationAxis;
    [Range(0f, 1f)] public float influence = 0.3f; // 0 = etkilenmez, 1 = spawn çarpan? kadar
    public float maxMultiplier = 1.5f;               // üst limit
    public float smoothing = 2f;                   // ne kadar yumu?ak geçsin (büyük = h?zl? geçi?)

    private float _currentMult = 1f;

    void Update()
    {
        if (PauseManager.Instance.IsPaused) return;

        float target = 1f;
        if (SpawnManager.Instance != null)
        {
            target = Mathf.Lerp(1f, SpawnManager.Instance.SpeedMultiplier, influence);
            target = Mathf.Min(target, maxMultiplier);
        }

        _currentMult = Mathf.MoveTowards(_currentMult, target, smoothing * Time.deltaTime);

        transform.Rotate(rotationAxis * _currentMult * Time.deltaTime);
    }
}