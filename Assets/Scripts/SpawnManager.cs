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
    public GameObject hatPrefab;

    public GameObject hatPrefab0;
    public GameObject discoPrefab;
    public GameObject sealPrefab;


    void Update()
    {
        // Toplam geçen süreyi tutuyoruz
        _timer = CargoCoreManager.instance.timer;

        // Mevcut spawn aralığını hesapla
        float currentInterval = CalculateCurrentInterval();

        // Spawn zamanı kontrolü
        if (Time.time >= _nextSpawnTime)
        {
            SpawnRandomVariant();
            _nextSpawnTime = Time.time + currentInterval;
        }
    }

    float CalculateCurrentInterval()
    {
        // Kaç tane 10 saniyelik blok geçtiğini hesapla (0, 1, 2, 3...)
        int steps = Mathf.FloorToInt(_timer / reductionStepTime);

        // Yeni süreyi hesapla: Başlangıç - (Adım Sayısı * 0.3)
        float calculatedInterval = baseSpawnInterval - (steps * reductionAmount);

        // Matematiksel limit koy: Belirlediğimiz minSpawnInterval (4) altına düşmesin
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

        if (chance < 3f)
        {
            // %3 Kedi
            GameObject[] catVariants = { cat0Cargo, cat1Cargo, cat2Cargo };
            selectedPrefab = catVariants[Random.Range(0, catVariants.Length)];
        }
        else if (chance < 6f)
        {
            // %3 Hat
            GameObject[] hatvariants = { hatPrefab, hatPrefab0 };
            selectedPrefab = hatvariants[Random.Range(0, hatvariants.Length)]; 
        }
        else if (chance < 100f)
        {
            // %3 Disco
            selectedPrefab = discoPrefab;
        }
        else if (chance < 10f)
        {
            // %1 Seal
            selectedPrefab = sealPrefab;
        }
        else
        {
            // %90 Normal box
            selectedPrefab = variants[Random.Range(0, variants.Length)];
        }
        //Chances
        //0-3       → Cat %3
        //3-6       → Hat %3
        //6-9       → Disco %3
        //9-10      → Seal %1
        //10-100    → Normal Box

        Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
    }
}