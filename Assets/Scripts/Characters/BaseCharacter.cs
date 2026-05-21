using UnityEngine;
using System;

public abstract class BaseCharacter : MonoBehaviour
{
    [Header("Basic Stats")]
    public string characterName;
    public float maxHealth = 6000f;
    public float currentHealth;
    public float movementSpeed = 3f;
    public int teamID = 0;

    [Header("Attack Stats")]
    public float attackRange = 7f;
    public float attackCooldown = 0.5f;
    public float reloadSpeed = 1f;
    public int attackDamage = 1000;
    public float projectileSpeed = 8f;
    public float projectileRadius = 0.3f;

    [Header("Super Stats")]
    public float maxSuperCharge = 100f;
    public float currentSuperCharge;
    public int superDamage = 2000;
    public float superRange = 8f;

    [Header("Gadget Cooldowns")]
    public float gadget1Cooldown = 15f;
    public float gadget2Cooldown = 15f;
    public float gadget1Timer;
    public float gadget2Timer;

    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    [Header("State")]
    public bool isAlive = true;
    public bool canAttack = true;
    public bool canMove = true;
    public bool isSlowed;
    public float slowTimer;
    public bool isStunned;
    public float stunTimer;
    public bool isInvisible;
    public float invisibleTimer;

    protected Rigidbody2D rb;
    protected Vector2 moveDir;
    protected Vector2 aimDir;
    protected float currentAttackCooldown;
    protected bool gadget1Used;
    protected bool gadget2Used;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.color = GetCharacterColor();
        }

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<CircleCollider2D>();
            collider.radius = 0.4f;
        }

        currentHealth = maxHealth;
        currentSuperCharge = 0f;
        gadget1Timer = 0f;
        gadget2Timer = 0f;

        GameManager.Instance?.RegisterCharacter(this);
    }

    public virtual void Update()
    {
        if (!isAlive) return;

        UpdateTimers();
        HandleTimedEffects();
    }

    public virtual void FixedUpdate()
    {
        if (!isAlive || !canMove) return;

        if (rb != null)
        {
            float speed = isSlowed ? movementSpeed * 0.5f : movementSpeed;
            rb.linearVelocity = moveDir * speed;
        }
    }

    void UpdateTimers()
    {
        if (!canAttack)
        {
            currentAttackCooldown -= Time.deltaTime;
            if (currentAttackCooldown <= 0f)
            {
                canAttack = true;
                currentAttackCooldown = 0f;
            }
        }

        if (gadget1Timer > 0f)
            gadget1Timer -= Time.deltaTime;
        if (gadget2Timer > 0f)
            gadget2Timer -= Time.deltaTime;
    }

    void HandleTimedEffects()
    {
        if (isSlowed)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0f)
            {
                isSlowed = false;
                slowTimer = 0f;
            }
        }

        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                isStunned = false;
                stunTimer = 0f;
            }
        }

        if (isInvisible)
        {
            invisibleTimer -= Time.deltaTime;
            if (invisibleTimer <= 0f)
            {
                isInvisible = false;
                spriteRenderer.color = GetCharacterColor();
            }
        }
    }

    public void SetMoveDirection(Vector2 dir)
    {
        moveDir = dir.normalized;
    }

    public void SetAimDirection(Vector2 dir)
    {
        aimDir = dir.normalized;
        if (aimDir != Vector2.zero)
        {
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public virtual void TryAttack()
    {
        if (!canAttack || !isAlive) return;
        Attack();
        canAttack = false;
        currentAttackCooldown = attackCooldown;
    }

    public abstract void Attack();

    public virtual void TryUseSuper()
    {
        if (!isAlive || currentSuperCharge < maxSuperCharge) return;
        UseSuper();
        currentSuperCharge = 0f;
    }

    public abstract void UseSuper();

    public abstract void UseGadget1();

    public abstract void UseGadget2();

    public virtual void TakeDamage(float damage, BaseCharacter attacker = null)
    {
        if (!isAlive) return;

        currentHealth -= damage;
        ChargeSuper(damage * 0.1f);

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
    }

    public virtual void Heal(float amount)
    {
        if (!isAlive) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public virtual void ChargeSuper(float amount)
    {
        currentSuperCharge = Mathf.Min(currentSuperCharge + amount, maxSuperCharge);
    }

    public virtual void Shield(float amount)
    {
    }

    public virtual void Die()
    {
        isAlive = false;
        spriteRenderer.color = Color.gray;
        rb.linearVelocity = Vector2.zero;
        enabled = false;

        GameManager.Instance?.OnCharacterDeath(this);
    }

    public bool IsDead() => !isAlive;

    public void ApplySlow(float duration)
    {
        isSlowed = true;
        slowTimer = duration;
    }

    public void ApplyStun(float duration)
    {
        isStunned = true;
        stunTimer = duration;
    }

    public void ApplyInvisible(float duration)
    {
        isInvisible = true;
        invisibleTimer = duration;
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(1, 1, 1, 0.3f);
    }

    protected ProjectileData CreateDefaultProjectileData()
    {
        return new ProjectileData
        {
            name = characterName + "_Attack",
            speed = projectileSpeed,
            damage = attackDamage,
            range = attackRange,
            radius = projectileRadius,
            color = GetCharacterColor()
        };
    }

    protected void FireProjectile(Vector2 direction, ProjectileData data)
    {
        if (direction == Vector2.zero) return;
        ProjectileManager.Instance.SpawnProjectile(transform.position, direction.normalized, data, this);
    }

    protected void FireSpreadProjectiles(int count, float spreadAngle, ProjectileData data)
    {
        Vector2 baseDir = aimDir;
        if (baseDir == Vector2.zero) baseDir = Vector2.right;

        float startAngle = -spreadAngle / 2f;
        float step = count > 1 ? spreadAngle / (count - 1) : 0f;

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + step * i;
            Vector2 dir = RotateVector(baseDir, angle * Mathf.Deg2Rad);
            FireProjectile(dir, data);
        }
    }

    protected Vector2 RotateVector(Vector2 v, float radians)
    {
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }

    public abstract Color GetCharacterColor();

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (IsDead()) return;

        PowerUp powerUp = other.GetComponent<PowerUp>();
        if (powerUp != null && !powerUp.isCollected)
        {
            powerUp.Collect(this);
            return;
        }
    }
}
