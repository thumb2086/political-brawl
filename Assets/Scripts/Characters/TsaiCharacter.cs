using UnityEngine;

public class TsaiCharacter : BaseCharacter
{
    public GameObject turretPrefab;

    public override void Awake()
    {
        characterName = "蔡英文";
        maxHealth = 5200f;
        movementSpeed = 3.2f;
        attackRange = 7.5f;
        attackCooldown = 0.5f;
        reloadSpeed = 1f;
        attackDamage = 800;
        projectileSpeed = 7f;
        projectileRadius = 0.3f;
        maxSuperCharge = 100f;
        superDamage = 600;
        superRange = 7f;
        gadget1Cooldown = 16f;
        gadget2Cooldown = 18f;

        base.Awake();
    }

    public override void Attack()
    {
        ProjectileData data = CreateDefaultProjectileData();
        data.name = "PolicyBeef";
        data.range = attackRange;
        data.healAmount = attackDamage * 0.5f;
        data.isPiercing = false;
        data.splashRadius = 0f;

        FireProjectile(aimDir, data);
    }

    public override void UseSuper()
    {
        Vector3 spawnPos = transform.position + (Vector3)aimDir.normalized * 1.5f;

        GameObject turretObj = new GameObject("NationalTeam_Turret");
        turretObj.transform.position = spawnPos;

        SpriteRenderer renderer = turretObj.AddComponent<SpriteRenderer>();
        renderer.color = new Color(0, 0.7f, 0.3f);
        renderer.sortingOrder = 0;

        CircleCollider2D collider = turretObj.AddComponent<CircleCollider2D>();
        collider.radius = 0.4f;
        collider.isTrigger = true;

        Turret turret = turretObj.AddComponent<Turret>();
        turret.owner = this;
        turret.attackDamage = superDamage;
        turret.attackRange = superRange;
        turret.attackCooldown = 0.8f;
        turret.duration = 10f;
        turret.health = 3000f;
    }

    public override void UseGadget1()
    {
        // "轉型正義" - Remove debuffs from allies
        if (gadget1Timer > 0f) return;
        gadget1Timer = gadget1Cooldown;

        float radius = 4f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in hits)
        {
            BaseCharacter character = hit.GetComponent<BaseCharacter>();
            if (character != null && !character.IsDead() && character.teamID == teamID)
            {
                character.isSlowed = false;
                character.slowTimer = 0f;
                character.isStunned = false;
                character.stunTimer = 0f;
                character.isInvisible = false;
                character.invisibleTimer = 0f;
            }
        }
    }

    public override void UseGadget2()
    {
        // "國際發聲" - Reveal all enemies on map
        if (gadget2Timer > 0f) return;
        gadget2Timer = gadget2Cooldown;

        foreach (var character in GameManager.Instance.allCharacters)
        {
            if (character != null && !character.IsDead() && character.teamID != teamID)
            {
                if (character.spriteRenderer != null)
                    character.spriteRenderer.color = Color.magenta;
                character.isInvisible = false;
            }
        }
    }

    public override Color GetCharacterColor()
    {
        return new Color(0, 0.5f, 0.8f);
    }
}

public class Turret : MonoBehaviour
{
    public BaseCharacter owner;
    public float attackDamage = 600f;
    public float attackRange = 7f;
    public float attackCooldown = 0.8f;
    public float duration = 10f;
    public float health = 3000f;
    public float maxHealth = 3000f;

    private float attackTimer;
    private SpriteRenderer renderer;

    void Start()
    {
        attackTimer = 0f;
        renderer = GetComponent<SpriteRenderer>();
        maxHealth = health;
    }

    void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0f || health <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            BaseCharacter target = GameManager.Instance.FindNearestEnemy(owner, attackRange);
            if (target != null)
            {
                ProjectileData data = new ProjectileData
                {
                    name = "Turret_Shot",
                    speed = 8f,
                    damage = attackDamage,
                    range = attackRange,
                    radius = 0.2f,
                    color = Color.cyan
                };

                Vector2 dir = (target.transform.position - transform.position).normalized;
                ProjectileManager.Instance.SpawnProjectile(transform.position, dir, data, owner);
                attackTimer = attackCooldown;
            }
            else
            {
                attackTimer = 0.3f;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Projectile proj = other.GetComponent<Projectile>();
        if (proj != null && proj.owner != owner)
        {
            health -= proj.data.damage;
            ProjectileManager.Instance.DespawnProjectile(proj);

            if (renderer != null)
            {
                renderer.color = Color.Lerp(Color.red, Color.cyan, health / maxHealth);
            }
        }
    }
}
