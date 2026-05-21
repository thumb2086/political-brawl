using UnityEngine;

public class XiCharacter : BaseCharacter
{
    private int bounceCount = 3;

    public override void Awake()
    {
        characterName = "習近平";
        maxHealth = 7000f;
        movementSpeed = 3f;
        attackRange = 7.5f;
        attackCooldown = 0.6f;
        reloadSpeed = 1f;
        attackDamage = 1200;
        projectileSpeed = 7f;
        projectileRadius = 0.3f;
        maxSuperCharge = 100f;
        superDamage = 600;
        superRange = 6f;
        gadget1Cooldown = 14f;
        gadget2Cooldown = 16f;

        base.Awake();
    }

    public override void Attack()
    {
        ProjectileData data = CreateDefaultProjectileData();
        data.name = "RedBook";
        data.range = attackRange;

        for (int i = 0; i < 3; i++)
        {
            Vector2 dir = aimDir;
            if (dir == Vector2.zero) dir = Vector2.right;

            float spread = (i - 1) * 8f * Mathf.Deg2Rad;
            Vector2 spreadDir = RotateVector(dir, spread);

            Projectile proj = ProjectileManager.Instance.SpawnBouncingProjectile(
                transform.position, spreadDir, data, this, bounceCount);
        }
    }

    public override void UseSuper()
    {
        Vector3 forward = aimDir != Vector2.zero ? (Vector3)aimDir : transform.right;
        GameObject path = new GameObject("BeltAndRoad");
        path.transform.position = transform.position;
        path.transform.rotation = Quaternion.FromToRotation(Vector3.right, forward);

        LineRenderer line = path.AddComponent<LineRenderer>();
        line.startWidth = 1.5f;
        line.endWidth = 1.5f;
        line.positionCount = 10;
        line.startColor = new Color(1, 0, 0, 0.6f);
        line.endColor = new Color(1, 0, 0, 0.2f);

        for (int i = 0; i < 10; i++)
        {
            float t = i / 9f;
            line.SetPosition(i, transform.position + forward * t * superRange);
        }

        BeltAndRoadPath pathComponent = path.AddComponent<BeltAndRoadPath>();
        pathComponent.owner = this;
        pathComponent.duration = 4f;
        pathComponent.pathDirection = forward;
        pathComponent.superRange = superRange;
        pathComponent.superDamage = superDamage;
    }

    public override void UseGadget1()
    {
        // "核心價值" - Slow zone
        if (gadget1Timer > 0f) return;
        gadget1Timer = gadget1Cooldown;

        GameObject zone = new GameObject("CoreValues_Zone");
        zone.transform.position = transform.position;

        SpriteRenderer renderer = zone.AddComponent<SpriteRenderer>();
        renderer.color = new Color(1, 0, 0, 0.3f);
        renderer.sortingOrder = -1;

        CircleCollider2D collider = zone.AddComponent<CircleCollider2D>();
        collider.radius = 3f;
        collider.isTrigger = true;

        SlowZone slowZone = zone.AddComponent<SlowZone>();
        slowZone.duration = 4f;
        slowZone.slowAmount = 0.4f;
    }

    public override void UseGadget2()
    {
        // "不忘初心" - Heal over time
        if (gadget2Timer > 0f) return;
        gadget2Timer = gadget2Cooldown;

        gameObject.AddComponent<HealOverTime>().healPerSecond = 600f;
        gameObject.AddComponent<HealOverTime>().duration = 4f;
    }

    public override Color GetCharacterColor()
    {
        return Color.red;
    }
}

public class BeltAndRoadPath : MonoBehaviour
{
    public BaseCharacter owner;
    public float duration;
    public Vector3 pathDirection;
    public float superRange;
    public float superDamage;
    private float timer;
    private LineRenderer line;

    void Start()
    {
        timer = duration;
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            transform.position + pathDirection * superRange * 0.5f,
            new Vector2(superRange, 1.5f), 0);

        foreach (var hit in hits)
        {
            BaseCharacter character = hit.GetComponent<BaseCharacter>();
            if (character != null && character != owner && !character.IsDead())
            {
                if (character.teamID == owner.teamID)
                {
                    character.Heal(superDamage * 0.5f * Time.deltaTime);
                }
                else
                {
                    character.TakeDamage(superDamage * Time.deltaTime, owner);
                    character.ApplySlow(0.5f);
                }
            }
        }

        if (line != null)
        {
            Color c = line.startColor;
            c.a = (timer / duration) * 0.6f;
            line.startColor = c;
            line.endColor = c;
        }

        if (timer <= 0f)
            Destroy(gameObject);
    }
}

public class SlowZone : MonoBehaviour
{
    public float duration;
    public float slowAmount;
    private float timer;
    private SpriteRenderer renderer;

    void Start()
    {
        timer = duration;
        renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (renderer != null)
        {
            Color c = renderer.color;
            c.a = (timer / duration) * 0.3f;
            renderer.color = c;
        }
        if (timer <= 0f)
            Destroy(gameObject);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        BaseCharacter character = other.GetComponent<BaseCharacter>();
        if (character != null && !character.IsDead())
        {
            character.ApplySlow(slowAmount);
        }
    }
}

public class HealOverTime : MonoBehaviour
{
    public float healPerSecond;
    public float duration;
    private float timer;
    private BaseCharacter character;

    void Start()
    {
        timer = duration;
        character = GetComponent<BaseCharacter>();
    }

    void Update()
    {
        if (character != null && !character.IsDead())
        {
            character.Heal(healPerSecond * Time.deltaTime);
        }
        timer -= Time.deltaTime;
        if (timer <= 0f)
            Destroy(this);
    }
}
