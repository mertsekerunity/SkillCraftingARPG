using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float craftingMaxCooldown = 0.4f;

    [SerializeField] SkillBook skillBookData; // Reference to the SkillBook ScriptableObject
    SkillData currentSkill; // Stores the currently crafted skill

    PlayerMana playerMana;
    Camera mainCam;
    Rigidbody rb;
    Animator animator; // Animator reference for handling animations
    SpriteRenderer spriteRenderer; // SpriteRenderer reference for flipping the sprite

    List<Orb> activeOrbs = new List<Orb>();

    Vector3 targetPoint = new Vector3();
    int maxOrbsCount;
    float craftingCooldown;
    float currentSkillCooldown = 0f; // Track cooldown separately

    private PlayerStates currentState = PlayerStates.Idle; // Track player's current state

    void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        playerMana = GetComponent<PlayerMana>();
        animator = GetComponent<Animator>(); // Get the Animator component
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component

        maxOrbsCount = System.Enum.GetValues(typeof(Orb)).Length; // Number of available orb types
    }

    void Update()
    {
        HandleOrbSelection();
        HandleSkillCrafting();
        HandleSkillExecution();
        UpdateAnimationState(); // Ensure animations update according to state
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        bool isMoving = false; // Track if player is moving

        if (Input.GetMouseButton(0)) // If left mouse button is clicked
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCam.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray.origin, ray.direction, out hit)) // Check if ray hits the ground
            {
                targetPoint = hit.point;
            }
            else
            {
                return;
            }

            targetPoint.y = transform.position.y; // Keep the movement on the same Y-axis
            Vector3 direction = (targetPoint - rb.position).normalized; // Get movement direction

            if (direction.magnitude > 0.1f) // Prevent unnecessary micro movements
            {
                isMoving = true; // Player is moving

                float delta = moveSpeed * Time.deltaTime;
                Vector3 newPos = rb.position + direction * delta;
                rb.MovePosition(newPos);

                // Flip the sprite based on movement direction
                if (direction.x > 0)
                    spriteRenderer.flipX = false; // Facing right
                else if (direction.x < 0)
                    spriteRenderer.flipX = true; // Facing left
            }
        }

        // Set player state based on movement
        currentState = isMoving ? PlayerStates.Walking : PlayerStates.Idle;
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
        Debug.Log($"Current orbs: {string.Join(", ", activeOrbs)}");
    }

    void HandleSkillCrafting()
    {
        if (activeOrbs.Count != maxOrbsCount) return;

        if (craftingCooldown > 0)
        {
            craftingCooldown -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.R) && craftingCooldown <= 0)
        {
            SkillData newSkill = skillBookData.GetSkill(activeOrbs);

            if (newSkill != null && (currentSkill == null || currentSkill.skillName != newSkill.skillName))
            {
                Debug.Log($"Crafted Skill: {newSkill.skillName}");
                currentSkill = newSkill;
                craftingCooldown = craftingMaxCooldown; // Apply cooldown to prevent immediate recrafting
            }
        }
    }

    void HandleSkillExecution()
    {
        if (currentSkill == null) return;

        if (currentSkillCooldown > 0)
        {
            currentSkillCooldown -= Time.deltaTime;
        }

        Debug.Log($"Remaining cooldown for {currentSkill.skillName}: {currentSkillCooldown} secs");

        if (Input.GetKeyDown(KeyCode.D) && currentSkillCooldown <= 0 && playerMana.mana >= currentSkill.manaCost)
        {
            Debug.Log($"{currentSkill.skillName} is used.");

            playerMana.ModifyMana(currentSkill);
            Debug.Log($"{playerMana.mana} MP left.");

            currentSkillCooldown = currentSkill.cooldown;

            // Set player state to UsingSkill
            currentState = PlayerStates.UsingSkill;
            Invoke(nameof(ResetState), 0.5f); // Reset to Idle after a short delay
        }
    }

    void UpdateAnimationState()
    {
        animator.SetBool("isWalking", currentState == PlayerStates.Walking);
        animator.SetBool("isAttacking", currentState == PlayerStates.Attacking);
        animator.SetBool("isUsingSkill", currentState == PlayerStates.UsingSkill);
    }

    void ResetState()
    {
        currentState = PlayerStates.Idle;
    }
}
