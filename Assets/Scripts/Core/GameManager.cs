using UnityEngine;
using System.Collections.Generic;

public enum GameState
{
    MainMenu,
    CharacterSelect,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState currentState = GameState.MainMenu;
    public BaseCharacter[] players;
    public List<BaseCharacter> allCharacters = new List<BaseCharacter>();
    public Transform[] spawnPoints;

    [Header("Game Settings")]
    public float gameTime = 180f;
    public int maxPlayers = 10;

    private float timer;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (currentState == GameState.Playing)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                EndGame();
            }
        }
    }

    public void StartGame(int[] selectedCharacterIndices)
    {
        currentState = GameState.Playing;
        timer = gameTime;
        SpawnPlayers(selectedCharacterIndices);
    }

    void SpawnPlayers(int[] characterIndices)
    {
        players = new BaseCharacter[characterIndices.Length];
        for (int i = 0; i < characterIndices.Length && i < spawnPoints.Length; i++)
        {
            players[i] = SpawnCharacter(characterIndices[i], spawnPoints[i].position);
            if (players[i] != null)
                players[i].teamID = 0;
        }
    }

    public BaseCharacter SpawnCharacter(int characterType, Vector3 position)
    {
        GameObject characterObj = new GameObject();
        BaseCharacter character = null;

        switch (characterType)
        {
            case 0:
                characterObj.name = "Trump";
                character = characterObj.AddComponent<TrumpCharacter>();
                break;
            case 1:
                characterObj.name = "XiJinping";
                character = characterObj.AddComponent<XiCharacter>();
                break;
            case 2:
                characterObj.name = "LaiChingTe";
                character = characterObj.AddComponent<LaiCharacter>();
                break;
            case 3:
                characterObj.name = "TsaiIngWen";
                character = characterObj.AddComponent<TsaiCharacter>();
                break;
            default:
                Destroy(characterObj);
                return null;
        }

        characterObj.transform.position = position;
        allCharacters.Add(character);
        return character;
    }

    public void RegisterCharacter(BaseCharacter character)
    {
        if (!allCharacters.Contains(character))
            allCharacters.Add(character);
    }

    public void OnCharacterDeath(BaseCharacter character)
    {
        allCharacters.Remove(character);
        int survivors = 0;
        foreach (var c in allCharacters)
        {
            if (c != null && c.teamID == 0)
                survivors++;
        }

        if (survivors <= 1 && currentState == GameState.Playing)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        currentState = GameState.GameOver;
    }

    public BaseCharacter FindNearestEnemy(BaseCharacter seeker, float range)
    {
        BaseCharacter nearest = null;
        float nearestDist = range;

        foreach (var c in allCharacters)
        {
            if (c == null || c == seeker || c.IsDead() || c.teamID == seeker.teamID)
                continue;

            float dist = Vector3.Distance(seeker.transform.position, c.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = c;
            }
        }

        return nearest;
    }

    public BaseCharacter FindNearestAlly(BaseCharacter seeker, float range)
    {
        BaseCharacter nearest = null;
        float nearestDist = range;

        foreach (var c in allCharacters)
        {
            if (c == null || c == seeker || c.IsDead() || c.teamID != seeker.teamID)
                continue;

            float dist = Vector3.Distance(seeker.transform.position, c.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = c;
            }
        }

        return nearest;
    }
}
