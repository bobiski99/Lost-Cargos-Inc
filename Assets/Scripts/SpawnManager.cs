using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [Header("Varyant Ayarları")]
    public GameObject[] variants = new GameObject[5];
    public Transform[] spawnPoints;

    [Header("Zorluk Ayarları")]
    public float baseSpawnInterval = 4.0f;  // Oyun başındaki spawn aralığı (sn)
    public float minSpawnInterval = 1.0f;   // İnilebilecek en düşük aralık
    public float reductionAmount = 0.3f;    // Her adımda azalacak miktar
    public float reductionStepTime = 5f;    // Kaç saniyede bir zorlaşacak

    // 1 = başlangıç, aralık düştükçe artar (Rotate bunu okuyor)
    public float SpeedMultiplier { get; private set; } = 1f;

    private float _spawnCountdown = 0f;

    [Header("Special Box Prefabs")]
    public GameObject cat0Cargo;
    public GameObject cat1Cargo;
    public GameObject cat2Cargo;
    public GameObject catSealCargo;
    public GameObject hatPrefab;
    public GameObject hatPrefab0;
    public GameObject hatPrefab1;
    public GameObject discoPrefab;
    public GameObject dicePrefab;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (PauseManager.Instance.IsPaused) return;

        float currentInterval = CalculateCurrentInterval();
        SpeedMultiplier = baseSpawnInterval / currentInterval;

        _spawnCountdown -= Time.deltaTime;
        if (_spawnCountdown <= 0f)
        {
            SpawnRandomVariant();
            _spawnCountdown = currentInterval;
        }
    }

    float CalculateCurrentInterval()
    {
        float timer = CargoCoreManager.instance.timer;
        int steps = Mathf.FloorToInt(timer / reductionStepTime);
        float interval = baseSpawnInterval - (steps * reductionAmount);

        return Mathf.Max(interval, minSpawnInterval);
    }

    void SpawnRandomVariant()
    {
        if (variants == null || variants.Length == 0) return;

        Vector3 spawnPos = Vector3.zero;
        if (spawnPoints != null && spawnPoints.Length > 0)
            spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;

        GameObject selectedPrefab;

        // 0-1 Dice %1 | 1-3 Cat %2 | 3-6 Hat %3 | 6-8 Disco %2 | 8-100 Normal %92
        float chance = Random.Range(0f, 100f);
        if (chance < 1f)
        {
            selectedPrefab = dicePrefab;
        }
        else if (chance < 3f)
        {
            GameObject[] cats = { cat0Cargo, cat1Cargo, cat2Cargo, catSealCargo };
            selectedPrefab = cats[Random.Range(0, cats.Length)];
        }
        else if (chance < 6f)
        {
            GameObject[] hats = { hatPrefab, hatPrefab0, hatPrefab1 };
            selectedPrefab = hats[Random.Range(0, hats.Length)];
        }
        else if (chance < 8f)
        {
            selectedPrefab = discoPrefab;
        }
        else
        {
            selectedPrefab = variants[Random.Range(0, variants.Length)];
        }

        if (selectedPrefab == null) return;

        Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
    }
}