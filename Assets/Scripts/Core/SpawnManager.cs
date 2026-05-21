using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    public Transform[] powerUpSpawnPoints;
    public GameObject[] powerUpPrefabs;
    public float powerUpSpawnInterval = 10f;
    public int maxPowerUps = 5;

    private List<GameObject> activePowerUps = new List<GameObject>();
    private float powerUpTimer;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        powerUpTimer = powerUpSpawnInterval;
    }

    void Update()
    {
        powerUpTimer -= Time.deltaTime;
        if (powerUpTimer <= 0f)
        {
            powerUpTimer = powerUpSpawnInterval;
            SpawnPowerUp();
        }
    }

    void SpawnPowerUp()
    {
        if (activePowerUps.Count >= maxPowerUps || powerUpSpawnPoints.Length == 0)
            return;

        Transform spawnPoint = powerUpSpawnPoints[Random.Range(0, powerUpSpawnPoints.Length)];
        GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];

        GameObject powerUp = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        activePowerUps.Add(powerUp);
    }

    public void RemovePowerUp(GameObject powerUp)
    {
        activePowerUps.Remove(powerUp);
    }

    public Vector3 GetRandomSpawnPoint()
    {
        if (powerUpSpawnPoints.Length > 0)
        {
            return powerUpSpawnPoints[Random.Range(0, powerUpSpawnPoints.Length)].position;
        }
        return Vector3.zero;
    }
}
