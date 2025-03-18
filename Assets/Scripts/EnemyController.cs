using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    //[SerializeField] float turnSpeed = 5f;
    //[SerializeField] float avoidanceRadius = 2f;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] float minimumVelocityThreshold = 0.05f; // Lower threshold to detect stopping sooner
    [SerializeField] float velocityCheckFrequency = 0.1f; // How often to check velocity (seconds)

    public float chaseRange = 60f;

    float distanceToTarget = Mathf.Infinity;
    bool isProvoked = false;
    float currentVelocity = 0f;
    bool wasMoving = false;
    float lastVelocityCheckTime = 0f;

    Transform target;
    Animator animator;
    NavMeshAgent navMeshAgent;
    SpriteRenderer spriteRenderer;

    // Animation parameter hashes (more efficient than strings)
    private readonly int isWalkingHash = Animator.StringToHash("isWalking");
    private readonly int isAttackingHash = Animator.StringToHash("isAttacking");
    private readonly int damageTakenHash = Animator.StringToHash("DamageTaken");
    private readonly int deathHash = Animator.StringToHash("Death");
    // Why is it better?
    // The string-to-hash conversion happens just once during initialization
    // After that, you use the pre-computed integer hash: animator.SetBool(isWalkingHash, true)
    // Integer comparisons are faster than string comparisons
    // This avoids the overhead of string processing every frame

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        target = FindObjectOfType<PlayerHealth>().transform;

        // Disable rotation by NavMeshAgent
        navMeshAgent.updateRotation = false;

        // Enable obstacle avoidance
        navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        navMeshAgent.avoidancePriority = 50;

        // Reset animation states
        animator.SetBool(isWalkingHash, false);
        animator.SetBool(isAttackingHash, false);
    }

    void Update()
    {
        if (GetComponent<EnemyHealth>() != null && GetComponent<EnemyHealth>().IsEnemyDead) return; // Don't update if dead

        if (target == null || target.GetComponent<PlayerHealth>().IsPlayerDead) 
        {
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(isAttackingHash, false);
            return;
        } 

        distanceToTarget = Vector3.Distance(target.position, transform.position);

        // More frequent velocity checking for immediate animation response
        if (Time.time > lastVelocityCheckTime + velocityCheckFrequency)
        {
            // Calculate actual velocity for animation transitions
            currentVelocity = navMeshAgent.velocity.magnitude;
            lastVelocityCheckTime = Time.time;

            // Check if movement state has changed
            bool isMovingNow = currentVelocity > minimumVelocityThreshold;

            // If movement state changed, update animation immediately
            if (wasMoving != isMovingNow)
            {
                wasMoving = isMovingNow;
                animator.SetBool(isWalkingHash, isMovingNow && !animator.GetBool(isAttackingHash));
            }
        }

        // Handle pursuit behavior
        if (isProvoked)
        {
            EngageTarget();
        }

        // Outside chase range - stop and go idle
        if (distanceToTarget > chaseRange)
        {
            if (isProvoked)
            {
                isProvoked = false;
                StopChasing();
            }
        }
        // Inside chase range - start pursuing
        else if (!isProvoked)
        {
            isProvoked = true;
        }
    }

    void StopChasing() //sorun bundan kaynakli olabilir, zombie implementationda bu yok !!!
    {
        // Force stop immediately
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.ResetPath();

        // Force animation to idle immediately
        animator.SetBool(isWalkingHash, false);
        wasMoving = false;
        currentVelocity = 0f;

        // Resume agent for future movement
        navMeshAgent.isStopped = false;
    }

    void ChaseTarget()
    {
        // Only update destination if we need to (optimization)
        if (!navMeshAgent.hasPath || navMeshAgent.pathPending ||
            Vector3.Distance(navMeshAgent.destination, target.position) > 1.0f)
        {
            navMeshAgent.SetDestination(target.position);
        }

        // Flag that we intend to be walking (actual movement checked separately)
        animator.SetBool(isAttackingHash, false);

        // Update sprite direction based on movement
        UpdateSpriteDirection();
    }

    void EngageTarget()
    {
        UpdateSpriteDirection();

        if (distanceToTarget >= navMeshAgent.stoppingDistance)
        {
            ChaseTarget();
        }
        if (distanceToTarget <= navMeshAgent.stoppingDistance)
        {
            if (target.GetComponent<PlayerHealth>().IsPlayerDead) return;

            AttackTarget();
        }
    }

    void AttackTarget()
    {
        // Stop moving
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.isStopped = true;

        // Set animation state
        animator.SetBool(isWalkingHash, false);
        animator.SetBool(isAttackingHash, true);
        wasMoving = false;
    }

    void UpdateSpriteDirection()
    {
        // Check if we have a path and are moving
        if (navMeshAgent.hasPath && currentVelocity > minimumVelocityThreshold)
        {
            // Get movement direction
            Vector3 moveDirection = navMeshAgent.velocity.normalized;

            // In an isometric view with camera at 45° on X and -135° on Y
            // Flip sprite based on X direction of movement
            if (moveDirection.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
        else if (target != null)
        {
            // When not moving but looking at target
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (directionToTarget.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    public void DamageTaken()
    {
        // Trigger damage animation
        //animator.SetTrigger(damageTakenHash);
        isProvoked = true;
    }

    public void TriggerMassProvoke()
    {
        BroadcastMessage("DamageTaken");
        Debug.Log("All enemies should attack the player!");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // Check if navMeshAgent exists before using it
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, navMeshAgent.stoppingDistance);
        }
    }
}
