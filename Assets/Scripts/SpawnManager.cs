using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [Header("Varyant Ayarları")]
    public GameObject[] variants = new GameObject[5]; // 5 farklı prefab
    public Transform[] spawnPoints;

    [Header("Zorluk Ayarları")]
    public float baseSpawnInterval = 2.0f; // Oyun başındaki başlangıç süresi (Örn: 10sn)
    public float minSpawnInterval = 4.0f;  // İnilebilecek en düşük süre (Limit)
    public float reductionAmount = 0.3f;   // Her adımda ne kadar azalacak
    public float reductionStepTime = 5f;  // Kaç saniyede bir zorlaşacak

    private float _timer = 0f;
    private float _nextSpawnTime = 0f;


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

    void Update()
    {
        if (PauseManager.Instance.IsPaused)
        {
            return;
        }
        _timer = CargoCoreManager.instance.timer;
        float currentInterval = CalculateCurrentInterval();

        if (Time.time >= _nextSpawnTime)
        {
            SpawnRandomVariant();
            _nextSpawnTime = Time.time + currentInterval;
        }
    }

    float CalculateCurrentInterval()
    {
        int steps = Mathf.FloorToInt(_timer / reductionStepTime);
        float calculatedInterval = baseSpawnInterval - (steps * reductionAmount);

        return Mathf.Max(calculatedInterval, minSpawnInterval);
    }
    void SpawnRandomVariant()
    {
        if (variants == null || variants.Length == 0) return;

        Vector3 spawnPos = Vector3.zero;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        }

        GameObject selectedPrefab;

        float chance = Random.Range(0f, 100f);
        if (chance < 1f)
        {
            // %1 Dice
            selectedPrefab = dicePrefab;
        }
        else if (chance < 4f)
        {
            // %3 Cat
            GameObject[] catVariants = { cat0Cargo, cat1Cargo, cat2Cargo, catSealCargo };
            selectedPrefab = catVariants[Random.Range(0, catVariants.Length)];
        }
        else if (chance < 7f)
        {
            // %3 Hat
            GameObject[] hatvariants = { hatPrefab, hatPrefab0, hatPrefab1 };
            selectedPrefab = hatvariants[Random.Range(0, hatvariants.Length)];
        }
        else if (chance < 10f)
        {
            // %1 Disco
            selectedPrefab = discoPrefab;
        }
        else
        {
            // %92 Normal Box
            selectedPrefab = variants[Random.Range(0, variants.Length)];
        }

        // Chances
        // 0-1      → Dice %1
        // 1-4      → Cat %3
        // 4-7      → Hat %3
        // 7-8      → Disco %1
        // 8-100    → Normal Box %92
        Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
    }
}