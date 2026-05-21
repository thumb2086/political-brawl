using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ProjectileData data;
    public BaseCharacter owner;
    public Vector2 direction;
    public float distanceTraveled;
    public int maxBounces;
    public int bouncesLeft;

    private bool initialized;

    public void Initialize(ProjectileData data, BaseCharacter owner, Vector2 direction)
    {
        this.data = data;
        this.owner = owner;
        this.direction = direction;
        this.distanceTraveled = 0f;
        this.maxBounces = 0;
        this.bouncesLeft = 0;
        this.initialized = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!initialized) return;

        if (data.isPiercing && bouncesLeft > 0)
        {
            bouncesLeft--;

            BaseCharacter character = other.GetComponent<BaseCharacter>();
            if (character != null && character != owner && !character.IsDead())
            {
                if (data.splashRadius > 0f)
                    ApplySplashDamage(character);
                else
                    character.TakeDamage(data.damage, owner);

                if (data.healAmount > 0f && owner != null)
                    owner.Heal(data.healAmount);

                if (data.slowDuration > 0f)
                    character.ApplySlow(data.slowDuration);
            }

            Vector2 newDir = Vector2.Reflect(direction, Vector2.right);
            direction = newDir;
            transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
            return;
        }

        BaseCharacter hitCharacter = other.GetComponent<BaseCharacter>();
        if (hitCharacter != null && hitCharacter != owner && !hitCharacter.IsDead())
        {
            if (data.splashRadius > 0f)
                ApplySplashDamage(hitCharacter);
            else
                hitCharacter.TakeDamage(data.damage, owner);

            if (data.healAmount > 0f && owner != null)
                owner.Heal(data.healAmount);

            if (data.slowDuration > 0f)
                hitCharacter.ApplySlow(data.slowDuration);

            ProjectileManager.Instance.DespawnProjectile(this);
            return;
        }

        if (other.isTrigger == false)
        {
            if (bouncesLeft > 0)
            {
                bouncesLeft--;
                Vector2 normal = (transform.position - other.transform.position).normalized;
                direction = Vector2.Reflect(direction, normal);
                transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
            }
            else
            {
                ProjectileManager.Instance.DespawnProjectile(this);
            }
        }
    }

    void ApplySplashDamage(BaseCharacter directHit)
    {
        if (data.splashRadius <= 0f) return;

        foreach (var c in GameManager.Instance.allCharacters)
        {
            if (c == null || c.IsDead() || c == owner) continue;

            float dist = Vector3.Distance(directHit.transform.position, c.transform.position);
            if (dist <= data.splashRadius)
            {
                float splashDmg = data.damage * (1f - dist / data.splashRadius);
                c.TakeDamage(splashDmg, owner);

                if (data.slowDuration > 0f)
                    c.ApplySlow(data.slowDuration * (1f - dist / data.splashRadius));
            }
        }
    }

    public void Tick()
    {
        if (!initialized) return;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            distanceTraveled += rb.linearVelocity.magnitude * Time.deltaTime;

            if (data.isHoming)
            {
                BaseCharacter target = GameManager.Instance.FindNearestEnemy(owner, data.range * 2f);
                if (target != null)
                {
                    Vector2 homingDir = (target.transform.position - transform.position).normalized;
                    rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, homingDir * data.speed, Time.deltaTime * 3f);
                    transform.rotation = Quaternion.FromToRotation(Vector3.right, rb.linearVelocity.normalized);
                }
            }
        }

        if (distanceTraveled >= data.range)
        {
            ProjectileManager.Instance.DespawnProjectile(this);
        }
    }
}
