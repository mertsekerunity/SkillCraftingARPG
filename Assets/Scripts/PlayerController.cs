using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Playables;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float craftingMaxCooldown = 0.4f;
    [SerializeField] float attackDamage = 25f;
    [SerializeField] float attackRange = 20f;

    [HideInInspector] public Skill currentActiveSkill = null;
    [HideInInspector] public Vector3 direction;

    PlayerMana playerMana;
    Camera mainCam;
    Rigidbody rb;
    Skill skill;
    Skill craftSkill;
    Animator animator;
    SpriteRenderer spriteRenderer;


    PlayerState playerState = PlayerState.Idle;
    PlayerState previousState = PlayerState.Idle;


    List<Orb> activeOrbs = new List<Orb>();
    Dictionary<HashSet<Orb>, Skill> skillBook;

    float lastWalkingDirection;
    float lastAttackingDirection;
    float lastUsingSkillDirection;

    int maxOrbsCount = System.Enum.GetValues(typeof(Orb)).Length;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        playerMana = GetComponent<PlayerMana>();
        skillBook = SkillBook.GetSkills();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        craftSkill = new Skill("Craft", craftingMaxCooldown, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        HandleOrbSelection();
        HandleSkillCrafting();
        HandleSkillExecution();
        HandleAttackExecution();
        HandleStates();

        if (playerState != previousState)
        {
            previousState = playerState;
            Debug.Log($"player state changed to: {playerState}");
        }

    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCam.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                playerState = PlayerState.Walking;

                Vector3 targetPoint = hit.point;
                direction = (targetPoint - rb.position).normalized;
                targetPoint.y = transform.position.y;
                float delta = moveSpeed * Time.deltaTime;
                lastWalkingDirection = direction.x;
                Vector3 newPos = rb.position + direction * delta;

                rb.MovePosition(newPos);
            }
            else
            {
                playerState = PlayerState.Idle;
                return;
            }
        }
        else
        {
            playerState = PlayerState.Idle;
        }
    }

    void HandleOrbSelection()
    {
        if (Input.GetKeyDown(KeyCode.Q)) AddOrb(Orb.Quas);
        if (Input.GetKeyDown(KeyCode.W)) AddOrb(Orb.Wex);
        //if (Input.GetKeyDown(KeyCode.E)) AddOrb(Orb.Exort);
    }

    void AddOrb(Orb orb)
    {
        if (activeOrbs.Count >= maxOrbsCount)
        {
            activeOrbs.RemoveAt(0);
        }

        activeOrbs.Add(orb);

        Debug.Log($"Current casted orbs: {string.Join(", ", activeOrbs)}");
    }

    void HandleSkillCrafting()
    {
        if (activeOrbs.Count != maxOrbsCount) return;

        if (craftSkill.skillCooldown > 0)
        {
            craftSkill.skillCooldown -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.R) && craftSkill.skillCooldown <= 0)
        {
            HashSet<Orb> orbSet = new HashSet<Orb>(activeOrbs);

            //Debug.Log(string.Join(", ", orbSet));

            if (skillBook.TryGetValue(orbSet, out Skill skill))
            {
                if (this.skill != skill)
                {
                    Debug.Log($"Crafted Skill: {skill.skillName}");
                    orbSet.Clear();
                    this.skill = skill;
                    craftSkill.skillCooldown = craftSkill.skillMaxCooldown;
                }
            }
        }
    }

    void HandleSkillExecution()
    {
        if (skill == null)
        {
            return;
        }

        if (skill.skillCooldown > 0)
        {
            skill.skillCooldown -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.D) && !(skill.skillCooldown > 0) && playerMana != null && playerMana.mana >= skill.requiredMana)
        {
            playerState = PlayerState.UsingSkill;
            currentActiveSkill = skill; // Store the skill reference

            // Direction calculation for animation
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCam.ScreenPointToRay(mousePos);
            RaycastHit hit;
            Vector3 targetPoint;

            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.origin + ray.direction * skill.skillRange;
            }

            Vector3 direction = (targetPoint - transform.position).normalized;
            lastUsingSkillDirection = direction.x;

            // Handle mana consumption
            if (playerMana != null)
            {
                playerMana.ModifyMana(skill);
                Debug.Log($" {playerMana.mana} MP left.");
            }
            else
            {
                Debug.LogError("PlayerMana is null!");
            }

            // Start cooldown
            skill.skillCooldown = skill.skillMaxCooldown;
        }
        else if (Input.GetKeyDown(KeyCode.D) && !(skill.skillCooldown <= 0) && playerMana.mana >= skill.requiredMana)
        {
            Debug.Log($"Remaining cooldown to use {skill.skillName}: {skill.skillCooldown} secs");
        }
        else if (Input.GetKeyDown(KeyCode.D) && !(skill.skillCooldown > 0) && playerMana.mana < skill.requiredMana)
        {
            Debug.Log("Not enough mana!");
        }
    }

    public void OnFireballAnimationEvent()
    {
        if (currentActiveSkill == null)
        {
            Debug.LogError("No active skill to cast!");
            return;
        }

        // Get reference to the projectile spawner (with built-in null check)
        if (TryGetComponent<SkillProjectileSpawner>(out var projectileSpawner))
        {
            // Spawn projectile with direct reference to skill
            projectileSpawner.SpawnProjectileForSkill(currentActiveSkill);
        }
        else
        {
            Debug.LogError("SkillProjectileSpawner component missing from player!");
        }
    }

    void HandleAttackExecution()  //attack cooldownu yok o yüzden tekrar attack edince bozuyor, onun icin bir mekanizma ekle!! GetMouseButtonDown?
    {
        if (Input.GetMouseButtonDown(1))
        {
            playerState = PlayerState.Attacking;

            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCam.ScreenPointToRay(mousePos);
            RaycastHit hit;
            LayerMask layerMask = LayerMask.GetMask("Enemy");

            if (Physics.Raycast(ray.origin, ray.direction, out hit, attackRange, layerMask))
            {
                EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
                direction = (target.transform.position - rb.position).normalized;
                lastAttackingDirection = direction.x;

                if (target != null)
                {
                    float dist = Vector3.Distance(target.transform.position, transform.position);
                    target.TakeDamage(attackDamage);
                }
                else return;
            }
        }
    }

    void HandleStates()
    {
        switch (playerState)
        {
            case PlayerState.Idle:
                if (lastWalkingDirection < 0.02f)
                {
                    spriteRenderer.flipX = true;
                }
                else
                {
                    spriteRenderer.flipX = false;
                }

                animator.SetBool("isWalking", false); // play idle animation

                break;
            case PlayerState.Walking:
                if (direction.x < 0.02f)
                {
                    spriteRenderer.flipX = true;
                }
                else
                {
                    spriteRenderer.flipX = false;
                }

                animator.SetBool("isWalking", true); // play walking animation

                break;
            case PlayerState.Attacking:
                if (direction.x < 0.02f)
                {
                    spriteRenderer.flipX = true;
                }
                else
                {
                    spriteRenderer.flipX = false;
                }

                animator.SetTrigger("Attack"); //play attack animation

                break;
            case PlayerState.UsingSkill:
                if (direction.x < 0.02f)
                {
                    spriteRenderer.flipX = true;
                }
                else
                {
                    spriteRenderer.flipX = false;
                }

                switch (skill.skillName)
                {
                    case "Skill 1":
                        animator.SetTrigger("Skill 1"); //play skill 1 animation
                        break;
                    case "Skill 2":
                        animator.SetTrigger("Skill 2"); //play skill 2 animation
                        break;
                    case "Skill 3":
                        animator.SetTrigger("Skill 3"); //play skill 3 animation
                        break;
                }
                break;
            default:
                // add hard reset for all animations, not only for walking and after force idle 
                break;
        }
    }
}
