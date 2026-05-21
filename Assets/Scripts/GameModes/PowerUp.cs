using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType
    {
        Health,
        Damage,
        Speed,
        SuperCharge
    }

    public PowerUpType type = PowerUpType.Health;
    public float amount = 2000f;
    public float duration = 5f;
    public bool isCollected { get; private set; }

    private SpriteRenderer renderer;
    private float respawnTimer;

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            renderer = gameObject.AddComponent<SpriteRenderer>();
        }

        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider == null)
            collider = gameObject.AddComponent<CircleCollider2D>();
        collider.radius = 0.3f;
        collider.isTrigger = true;

        switch (type)
        {
            case PowerUpType.Health:
                renderer.color = Color.green;
                break;
            case PowerUpType.Damage:
                renderer.color = Color.red;
                break;
            case PowerUpType.Speed:
                renderer.color = Color.yellow;
                break;
            case PowerUpType.SuperCharge:
                renderer.color = Color.magenta;
                break;
        }
    }

    void Update()
    {
        transform.Rotate(0, 0, 90f * Time.deltaTime);

        if (isCollected)
        {
            respawnTimer -= Time.deltaTime;
            if (respawnTimer <= 0f)
            {
                isCollected = false;
                renderer.enabled = true;
                GetComponent<Collider2D>().enabled = true;
            }
        }
    }

    public void Collect(BaseCharacter character)
    {
        if (isCollected) return;
        isCollected = true;

        switch (type)
        {
            case PowerUpType.Health:
                character.Heal(amount);
                break;
            case PowerUpType.Damage:
                character.attackDamage += (int)(character.attackDamage * 0.3f);
                Invoke(nameof(RemoveDamageBuff), duration);
                break;
            case PowerUpType.Speed:
                character.movementSpeed *= 1.3f;
                Invoke(nameof(RemoveSpeedBuff), duration);
                break;
            case PowerUpType.SuperCharge:
                character.ChargeSuper(amount);
                break;
        }

        renderer.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        respawnTimer = 10f;

        SpawnManager.Instance?.RemovePowerUp(gameObject);
    }

    void RemoveDamageBuff()
    {
    }

    void RemoveSpeedBuff()
    {
    }
}
