using UnityEngine;

public class BotSpawner : MonoBehaviour
{
    public int botCount = 5;
    public float spawnRadius = 8f;
    public GameObject botPrefab;

    void Start()
    {
        for (int i = 0; i < botCount; i++)
        {
            SpawnBot();
        }
    }

    void SpawnBot()
    {
        Vector3 spawnPos = transform.position + (Vector3)Random.insideUnitCircle * spawnRadius;
        int characterType = Random.Range(0, 4);

        BaseCharacter character = GameManager.Instance.SpawnCharacter(characterType, spawnPos);
        if (character != null)
        {
            character.teamID = 1;
            character.gameObject.AddComponent<AIBrain>();
        }
    }
}
