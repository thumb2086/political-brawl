using UnityEngine;
using System.Collections.Generic;

public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager Instance { get; private set; }

    public GameObject projectilePrefab;
    private List<Projectile> activeProjectiles = new List<Projectile>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        for (int i = activeProjectiles.Count - 1; i >= 0; i--)
        {
            if (activeProjectiles[i] == null)
            {
                activeProjectiles.RemoveAt(i);
                continue;
            }
            activeProjectiles[i].Tick();
        }
    }

    public Projectile SpawnProjectile(Vector3 position, Vector2 direction, ProjectileData data, BaseCharacter owner)
    {
        GameObject go = new GameObject("Projectile_" + data.name);
        go.transform.position = position;
        go.transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);

        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        if (data.sprite != null)
            renderer.sprite = data.sprite;
        renderer.color = data.color;

        CircleCollider2D collider = go.AddComponent<CircleCollider2D>();
        collider.radius = data.radius;
        collider.isTrigger = true;

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearVelocity = direction * data.speed;

        Projectile projectile = go.AddComponent<Projectile>();
        projectile.Initialize(data, owner, direction);

        activeProjectiles.Add(projectile);
        return projectile;
    }

    public Projectile SpawnBouncingProjectile(Vector3 position, Vector2 direction, ProjectileData data, BaseCharacter owner, int bounces)
    {
        Projectile p = SpawnProjectile(position, direction, data, owner);
        p.maxBounces = bounces;
        return p;
    }

    public void DespawnProjectile(Projectile projectile)
    {
        if (projectile != null)
        {
            activeProjectiles.Remove(projectile);
            Destroy(projectile.gameObject);
        }
    }

    public void ClearAll()
    {
        foreach (var p in activeProjectiles)
        {
            if (p != null)
                Destroy(p.gameObject);
        }
        activeProjectiles.Clear();
    }
}

[System.Serializable]
public class ProjectileData
{
    public string name;
    public float speed = 8f;
    public float damage = 1000f;
    public float range = 8f;
    public float radius = 0.3f;
    public Color color = Color.white;
    public Sprite sprite;
    public bool isPiercing = false;
    public bool isHoming = false;
    public float splashRadius = 0f;
    public float slowDuration = 0f;
    public float healAmount = 0f;
}
