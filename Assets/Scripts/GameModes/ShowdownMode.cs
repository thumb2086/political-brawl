using UnityEngine;
using System.Collections.Generic;

public class ShowdownMode : MonoBehaviour
{
    [Header("Gas Settings")]
    public float gasStartDelay = 60f;
    public float gasShrinkInterval = 30f;
    public float gasShrinkAmount = 2f;
    public float gasDamagePerTick = 200f;
    public float gasTickInterval = 1f;

    [Header("Map Bounds")]
    public Vector2 mapCenter = Vector2.zero;
    public Vector2 mapSize = new Vector2(20f, 20f);

    [Header("Power-Ups")]
    public int powerUpCount = 5;
    public float powerUpRespawnTime = 10f;

    private float gameTimer;
    private float gasTimer;
    private float gasTickTimer;
    private float currentGasRadius;
    private float maxGasRadius;
    private int gasPhase;
    private List<BaseCharacter> aliveCharacters = new List<BaseCharacter>();

    void Start()
    {
        maxGasRadius = Mathf.Max(mapSize.x, mapSize.y) * 0.7f;
        currentGasRadius = maxGasRadius;
        gasTimer = gasStartDelay;
        gasTickTimer = 0f;
        gasPhase = 0;

        RefreshAliveList();
    }

    void Update()
    {
        gameTimer += Time.deltaTime;

        UpdateGas();
        CheckWinCondition();

        if (Input.GetKeyDown(KeyCode.M))
        {
            // Dev shortcut: spawn more bots
        }
    }

    void UpdateGas()
    {
        if (gasPhase >= 5) return;

        gasTimer -= Time.deltaTime;
        if (gasTimer <= 0f)
        {
            gasPhase++;
            float shrink = gasShrinkAmount * (1f + gasPhase * 0.5f);
            currentGasRadius = Mathf.Max(currentGasRadius - shrink, 2f);
            gasTimer = gasShrinkInterval;

            if (gasPhase >= 5)
                gasTimer = float.MaxValue;
        }

        gasTickTimer -= Time.deltaTime;
        if (gasTickTimer <= 0f)
        {
            gasTickTimer = gasTickInterval;
            ApplyGasDamage();
        }
    }

    void ApplyGasDamage()
    {
        RefreshAliveList();

        foreach (var character in aliveCharacters)
        {
            if (character == null || character.IsDead()) continue;

            float dist = Vector2.Distance(character.transform.position, mapCenter);
            if (dist > currentGasRadius)
            {
                character.TakeDamage(gasDamagePerTick);
            }
        }
    }

    void CheckWinCondition()
    {
        RefreshAliveList();

        if (aliveCharacters.Count <= 1)
        {
            string winnerName = aliveCharacters.Count == 1 ? aliveCharacters[0].characterName : "None";
            GameUI.Instance?.ShowGameOver($"Winner: {winnerName}");
            GameManager.Instance.currentState = GameState.GameOver;
        }
    }

    void RefreshAliveList()
    {
        aliveCharacters.Clear();
        foreach (var c in GameManager.Instance.allCharacters)
        {
            if (c != null && !c.IsDead())
                aliveCharacters.Add(c);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(mapCenter, mapSize);

        Gizmos.color = new Color(0, 1, 0, 0.1f);
        Gizmos.DrawWireSphere(mapCenter, currentGasRadius);
    }
}
