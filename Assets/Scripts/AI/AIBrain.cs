using UnityEngine;

public enum AIState
{
    Idle,
    Patrol,
    Chase,
    Attack,
    Flee,
    CollectPowerUp
}

public class AIBrain : MonoBehaviour
{
    public AIState currentState = AIState.Patrol;
    public float detectionRange = 8f;
    public float attackRange = 6f;
    public float fleeHealthPercent = 0.25f;
    public float decisionInterval = 0.5f;

    private BaseCharacter character;
    private float decisionTimer;
    private BaseCharacter currentTarget;
    private Vector2 wanderTarget;
    private float wanderTimer;

    void Start()
    {
        character = GetComponent<BaseCharacter>();
        decisionTimer = Random.Range(0f, decisionInterval);
        PickRandomWanderTarget();
    }

    void Update()
    {
        if (character == null || character.IsDead()) return;

        decisionTimer -= Time.deltaTime;
        if (decisionTimer <= 0f)
        {
            decisionTimer = decisionInterval;
            MakeDecision();
        }

        ExecuteState();
    }

    void MakeDecision()
    {
        if (character.currentHealth / character.maxHealth < fleeHealthPercent)
        {
            currentState = AIState.Flee;
            return;
        }

        currentTarget = GameManager.Instance.FindNearestEnemy(character, detectionRange);

        if (currentTarget != null)
        {
            float dist = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (dist <= attackRange)
                currentState = AIState.Attack;
            else
                currentState = AIState.Chase;
        }
        else
        {
            currentState = AIState.Patrol;
        }
    }

    void ExecuteState()
    {
        switch (currentState)
        {
            case AIState.Patrol:
                PatrolBehavior();
                break;
            case AIState.Chase:
                ChaseBehavior();
                break;
            case AIState.Attack:
                AttackBehavior();
                break;
            case AIState.Flee:
                FleeBehavior();
                break;
        }
    }

    void PatrolBehavior()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f)
        {
            PickRandomWanderTarget();
            wanderTimer = Random.Range(2f, 5f);
        }

        Vector2 dir = (wanderTarget - (Vector2)transform.position).normalized;
        character.SetMoveDirection(dir);

        if (dir != Vector2.zero)
        {
            character.SetAimDirection(dir);
            character.TryAttack();
        }
    }

    void ChaseBehavior()
    {
        if (currentTarget == null) return;

        Vector2 dir = (currentTarget.transform.position - transform.position).normalized;
        character.SetMoveDirection(dir);
        character.SetAimDirection(dir);

        if (character.currentSuperCharge >= character.maxSuperCharge)
            character.TryUseSuper();
    }

    void AttackBehavior()
    {
        if (currentTarget == null)
        {
            currentState = AIState.Patrol;
            return;
        }

        Vector2 dir = (currentTarget.transform.position - transform.position).normalized;
        character.SetMoveDirection(Vector2.zero);
        character.SetAimDirection(dir);
        character.TryAttack();

        if (character.currentSuperCharge >= character.maxSuperCharge)
            character.TryUseSuper();

        if (Random.value < 0.2f)
        {
            if (character.gadget1Timer <= 0f && Random.value < 0.5f)
                character.UseGadget1();
            else if (character.gadget2Timer <= 0f)
                character.UseGadget2();
        }
    }

    void FleeBehavior()
    {
        BaseCharacter threat = GameManager.Instance.FindNearestEnemy(character, detectionRange);
        if (threat != null)
        {
            Vector2 fleeDir = (transform.position - threat.transform.position).normalized;
            character.SetMoveDirection(fleeDir);
        }
        else
        {
            currentState = AIState.Patrol;
        }
    }

    void CollectPowerUp(Collider2D powerUpCollider)
    {
        Vector2 dir = (powerUpCollider.transform.position - transform.position).normalized;
        character.SetMoveDirection(dir);
    }

    void PickRandomWanderTarget()
    {
        wanderTarget = (Vector2)transform.position + Random.insideUnitCircle * 5f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PowerUp powerUp = other.GetComponent<PowerUp>();
        if (powerUp != null && !powerUp.isCollected)
        {
            powerUp.Collect(character);
        }
    }
}
