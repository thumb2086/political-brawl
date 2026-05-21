using UnityEngine;

public class TrumpCharacter : BaseCharacter
{
    public override void Awake()
    {
        characterName = "川普";
        maxHealth = 10000f;
        movementSpeed = 2.8f;
        attackRange = 6f;
        attackCooldown = 0.7f;
        reloadSpeed = 0.8f;
        attackDamage = 1400;
        projectileSpeed = 7f;
        projectileRadius = 0.35f;
        maxSuperCharge = 100f;
        superDamage = 0;
        superRange = 0f;
        gadget1Cooldown = 18f;
        gadget2Cooldown = 20f;

        base.Awake();
    }

    public override void Attack()
    {
        ProjectileData data = CreateDefaultProjectileData();
        data.name = "TruthSocial_Tweet";
        data.splashRadius = 0f;
        data.range = attackRange;

        FireSpreadProjectiles(5, 30f, data);
    }

    public override void UseSuper()
    {
        Vector3 forward = aimDir != Vector2.zero ? (Vector3)aimDir : transform.right;
        Vector3 wallPos = transform.position + forward * 2f;

        GameObject wall = new GameObject("TrumpWall");
        wall.transform.position = wallPos;
        wall.transform.rotation = Quaternion.FromToRotation(Vector3.right, forward);

        SpriteRenderer renderer = wall.AddComponent<SpriteRenderer>();
        renderer.color = new Color(0.6f, 0.4f, 0.2f);

        BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(3f, 1f);
        collider.isTrigger = false;

        Rigidbody2D rb = wall.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        wall.AddComponent<WallDestroyTimer>().duration = 5f;
    }

    public override void UseGadget1()
    {
        // "You're Fired!" - Knockback nearby enemies with damage
        if (gadget1Timer > 0f) return;
        gadget1Timer = gadget1Cooldown;

        float knockbackRadius = 3f;
        float knockbackDamage = 800f;
        float knockbackForce = 10f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, knockbackRadius);
        foreach (var hit in hits)
        {
            BaseCharacter character = hit.GetComponent<BaseCharacter>();
            if (character != null && character != this && character.teamID != teamID && !character.IsDead())
            {
                character.TakeDamage(knockbackDamage, this);
                Vector2 pushDir = (character.transform.position - transform.position).normalized;
                Rigidbody2D targetRb = character.GetComponent<Rigidbody2D>();
                if (targetRb != null)
                    targetRb.AddForce(pushDir * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }

    public override void UseGadget2()
    {
        // "Trump Card" - Brief invincibility
        if (gadget2Timer > 0f) return;
        gadget2Timer = gadget2Cooldown;

        gameObject.AddComponent<InvincibilityTimer>().duration = 2f;
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(1, 1, 0.5f, 0.8f);
    }

    public override Color GetCharacterColor()
    {
        return new Color(0.9f, 0.4f, 0.1f);
    }
}

public class WallDestroyTimer : MonoBehaviour
{
    public float duration;

    void Start()
    {
        Destroy(gameObject, duration);
    }
}

public class InvincibilityTimer : MonoBehaviour
{
    public float duration;
    private BaseCharacter character;
    private Color originalColor;

    void Start()
    {
        character = GetComponent<BaseCharacter>();
        duration = 2f;
        Destroy(this, duration);
    }

    void OnDestroy()
    {
    }
}
