using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float craftingMaxCooldown = 0.4f;

    [SerializeField] SkillBook skillBookData; // Reference to the SkillBook ScriptableObject
    [SerializeField] float attackDamage = 25f;
    [SerializeField] float attackRange = 20f;

    // Currently active skill data
    private SkillData currentSkill;
    [HideInInspector] public SkillData currentActiveSkill = null;
    private float craftingCooldown;
    private float currentSkillCooldown = 0f;

    // Component references
    private PlayerMana playerMana;
    private Camera mainCam;
    private Rigidbody rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private SkillProjectileSpawner projectileSpawner;

    // State tracking
    private PlayerStates currentState = PlayerStates.Idle;
    private Vector3 targetPoint = Vector3.zero;
    private Vector3 direction;
    private float lastWalkingDirection;

    // Orb system
    private List<Orb> activeOrbs = new List<Orb>();
    private int maxOrbsCount;

    void Start()
    {
        // Get component references
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        playerMana = GetComponent<PlayerMana>();

        // Get components from children if needed
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        projectileSpawner = GetComponentInChildren<SkillProjectileSpawner>();

        // Initialize orb system
        maxOrbsCount = System.Enum.GetValues(typeof(Orb)).Length;

        if (projectileSpawner == null)
        {
            Debug.LogError("SkillProjectileSpawner component not found in player or its children!");
        }
    }

    void Update()
    {
        HandleOrbSelection();
        HandleSkillCrafting();
        HandleSkillExecution();
        HandleAttackExecution();
        UpdateAnimationState();

        // Handle cooldowns
        if (craftingCooldown > 0)
        {
            craftingCooldown -= Time.deltaTime;
        }

        if (currentSkillCooldown > 0)
        {
            currentSkillCooldown -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        bool isMoving = false;

        if (Input.GetMouseButton(0)) // Left mouse button for movement
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCam.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                targetPoint = hit.point;
                targetPoint.y = transform.position.y; // Keep on the same Y plane
                direction = (targetPoint - rb.position).normalized;

                if (direction.magnitude > 0.1f) // Prevent micro-movements
                {
                    isMoving = true;
                    lastWalkingDirection = direction.x;

                    float delta = moveSpeed * Time.deltaTime;
                    Vector3 newPos = rb.position + direction * delta;
                    rb.MovePosition(newPos);

                    // Flip sprite based on movement direction
                    spriteRenderer.flipX = direction.x < 0;
                }
            }
        }

        // Update state based on movement
        if (isMoving && currentState != PlayerStates.Attacking && currentState != PlayerStates.UsingSkill)
        {
            currentState = PlayerStates.Walking;
        }
        else if (!isMoving && currentState != PlayerStates.Attacking && currentState != PlayerStates.UsingSkill)
        {
            currentState = PlayerStates.Idle;
        }
    }

    void HandleOrbSelection()
    {
        if (Input.GetKeyDown(KeyCode.Q)) AddOrb(Orb.Quas);
        if (Input.GetKeyDown(KeyCode.W)) AddOrb(Orb.Wex);
        // Future expansion: if (Input.GetKeyDown(KeyCode.E)) AddOrb(Orb.Exort);
    }

    void AddOrb(Orb orb)
    {
        if (activeOrbs.Count >= maxOrbsCount)
        {
            activeOrbs.RemoveAt(0); // Remove oldest orb
        }

        activeOrbs.Add(orb);
        Debug.Log($"Current orbs: {string.Join(", ", activeOrbs)}");
    }

    void HandleSkillCrafting()
    {
        if (activeOrbs.Count != maxOrbsCount) return;

        if (Input.GetKeyDown(KeyCode.R) && craftingCooldown <= 0)
        {
            SkillData newSkill = skillBookData.GetSkill(activeOrbs);

            if (newSkill != null && (currentSkill == null || currentSkill.skillName != newSkill.skillName))
            {
                Debug.Log($"Crafted Skill: {newSkill.skillName}");
                currentSkill = newSkill;
                craftingCooldown = craftingMaxCooldown;
            }
        }
    }

    void HandleSkillExecution()
    {
        if (currentSkill == null) return;

        if (Input.GetKeyDown(KeyCode.D) && currentSkillCooldown <= 0 && playerMana.mana >= currentSkill.manaCost)
        {
            Debug.Log($"{currentSkill.skillName} is being used.");

            // Store the active skill for the animation event
            currentActiveSkill = currentSkill;

            // Compute direction for animation
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCam.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                direction = (hit.point - transform.position).normalized;
            }
            else
            {
                direction = ray.direction;
            }

            // Flip sprite based on skill cast direction
            spriteRenderer.flipX = direction.x < 0;

            // Handle mana consumption
            playerMana.ModifyMana(currentSkill);
            Debug.Log($"{playerMana.mana} MP left.");

            // Set cooldown
            currentSkillCooldown = currentSkill.cooldown;

            // Set state
            currentState = PlayerStates.UsingSkill;

            // Trigger animation based on skill name or animation trigger
            string animTrigger = currentSkill.GetAnimationTrigger();
            animator.SetTrigger(animTrigger);
        }
        else if (Input.GetKeyDown(KeyCode.D) && currentSkillCooldown > 0)
        {
            Debug.Log($"Remaining cooldown for {currentSkill.skillName}: {currentSkillCooldown} secs");
        }
        else if (Input.GetKeyDown(KeyCode.D) && playerMana.mana < currentSkill.manaCost)
        {
            Debug.Log("Not enough mana!");
        }
    }

    public void OnSkillAnimationEvent()
    {
        if (currentActiveSkill == null)
        {
            Debug.LogError("No active skill to cast!");
            return;
        }

        if (projectileSpawner != null)
        {
            projectileSpawner.SpawnProjectileForSkill(currentActiveSkill);
        }
        else
        {
            Debug.LogError("ProjectileSpawner is null when trying to spawn projectile!");
        }
    }

    void HandleAttackExecution()
    {
        if (Input.GetMouseButtonDown(1)) // Right mouse button for attacks
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCam.ScreenPointToRay(mousePos);
            LayerMask enemyLayer = LayerMask.GetMask("Enemy");

            if (Physics.Raycast(ray, out RaycastHit hit, attackRange, enemyLayer))
            {
                
                if (hit.transform.TryGetComponent<EnemyHealth>(out var target))
                {
                    direction = (target.transform.position - transform.position).normalized;
                    spriteRenderer.flipX = direction.x < 0;

                    // Apply damage
                    target.TakeDamage(attackDamage);

                    // Set state and trigger animation
                    currentState = PlayerStates.Attacking;
                    animator.SetTrigger("Attack");
                }
            }
        }
    }

    void UpdateAnimationState()
    {
        animator.SetBool("isWalking", currentState == PlayerStates.Walking);
    }

    // Called from animation events or after skill/attack completes
    public void ResetState()
    {
        // Only reset if we're in an action state, don't interrupt walking
        if (currentState == PlayerStates.Attacking || currentState == PlayerStates.UsingSkill)
        {
            currentState = PlayerStates.Idle;
            currentActiveSkill = null;
        }
    }
}
