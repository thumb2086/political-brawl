using UnityEngine;

public class LaiCharacter : BaseCharacter
{
    private int barrageCount = 6;

    public override void Awake()
    {
        characterName = "賴清德";
        maxHealth = 4800f;
        movementSpeed = 3.4f;
        attackRange = 9f;
        attackCooldown = 0.8f;
        reloadSpeed = 1.2f;
        attackDamage = 1800;
        projectileSpeed = 9f;
        projectileRadius = 0.2f;
        maxSuperCharge = 100f;
        superDamage = 1400;
        superRange = 8f;
        gadget1Cooldown = 12f;
        gadget2Cooldown = 20f;

        base.Awake();
    }

    public override void Attack()
    {
        ProjectileData data = CreateDefaultProjectileData();
        data.name = "TaiwanTeam_Shot";
        data.range = attackRange;
        data.radius = 0.2f;

        FireProjectile(aimDir, data);
    }

    public override void UseSuper()
    {
        ProjectileData data = CreateDefaultProjectileData();
        data.name = "UnityTaiwan_Barrage";
        data.damage = superDamage;
        data.range = superRange;
        data.radius = 0.25f;

        FireSpreadProjectiles(barrageCount, 40f, data);
    }

    public override void UseGadget1()
    {
        // "穩健執政" - Next attack has bonus range and damage
        if (gadget1Timer > 0f) return;
        gadget1Timer = gadget1Cooldown;

        gameObject.AddComponent<NextAttackBoost>().duration = 5f;
    }

    public override void UseGadget2()
    {
        // "和平對話" - Create peace zone where enemies can't attack for a moment
        if (gadget2Timer > 0f) return;
        gadget2Timer = gadget2Cooldown;

        GameObject zone = new GameObject("PeaceDialog_Zone");
        zone.transform.position = transform.position;

        SpriteRenderer renderer = zone.AddComponent<SpriteRenderer>();
        renderer.color = new Color(0, 1, 0, 0.2f);
        renderer.sortingOrder = -1;

        CircleCollider2D collider = zone.AddComponent<CircleCollider2D>();
        collider.radius = 3.5f;
        collider.isTrigger = true;

        PeaceZone peaceZone = zone.AddComponent<PeaceZone>();
        peaceZone.duration = 3f;
        peaceZone.owner = this;
    }

    public override Color GetCharacterColor()
    {
        return new Color(0, 0.6f, 0.2f);
    }
}

public class NextAttackBoost : MonoBehaviour
{
    public float duration;
    private float timer;
    private BaseCharacter character;
    private bool boosted;

    void Start()
    {
        timer = duration;
        character = GetComponent<BaseCharacter>();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f && !boosted)
        {
            Destroy(this);
        }
    }
}

public class PeaceZone : MonoBehaviour
{
    public float duration;
    public BaseCharacter owner;
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
            c.a = (timer / duration) * 0.2f;
            renderer.color = c;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 3.5f);
        foreach (var hit in hits)
        {
            BaseCharacter character = hit.GetComponent<BaseCharacter>();
            if (character != null && character != owner && !character.IsDead() && character.teamID != owner.teamID)
            {
                character.ApplyStun(0.2f);
            }
        }

        if (timer <= 0f)
            Destroy(gameObject);
    }
}
