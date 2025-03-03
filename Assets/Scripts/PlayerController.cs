using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float attackDamage = 25f;
    [SerializeField] float attackRange = 20f;
    [SerializeField] SkillSO craftSkill;
    [SerializeField] UnityEngine.UI.Image activeSkillIcon;

    [SerializeField] UnityEngine.UI.Image firstActiveOrb;
    [SerializeField] UnityEngine.UI.Image secondActiveOrb;

    [SerializeField] Sprite quasSprite;
    [SerializeField] Sprite wexSprite;

    [SerializeField] GameObject quasPrefab;
    [SerializeField] GameObject wexPrefab;

    [SerializeField] GameObject firstActiveOrbPrefabLocation;
    [SerializeField] GameObject secondActiveOrbPrefabLocation;

    GameObject firstActiveOrbPrefab;
    GameObject secondActiveOrbPrefab;


    [SerializeField] SkillBookSO skillBookSO;


    [HideInInspector] public Vector3 direction;

    PlayerMana playerMana;
    Camera mainCam;
    Rigidbody rb;
    [HideInInspector] public SkillSO currentSkill;
    Animator animator;
    SpriteRenderer spriteRenderer;

    PlayerState playerState = PlayerState.Idle;
    PlayerState previousState = PlayerState.Idle;

    List<Orb> activeOrbs = new List<Orb>();

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
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Reset craft skill cooldown if needed
        if (craftSkill != null)
        {
            craftSkill.skillCooldown = 0f;
        }
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

                if (firstActiveOrbPrefab != null)
                {
                    firstActiveOrbPrefab.transform.position = firstActiveOrbPrefabLocation.transform.position;
                }
                if (secondActiveOrbPrefab != null)
                {
                    secondActiveOrbPrefab.transform.position = secondActiveOrbPrefabLocation.transform.position;
                }
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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddOrb(Orb.Quas);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            AddOrb(Orb.Wex);
        }

        //if (Input.GetKeyDown(KeyCode.E)) AddOrb(Orb.Exort);
    }

    void AddOrb(Orb orb)
    {
        // Get sprite and prefab based on orb type
        Sprite orbSprite = orb == Orb.Quas ? quasSprite : wexSprite;
        GameObject orbPrefab = orb == Orb.Quas ? quasPrefab : wexPrefab;

        // Handle max orbs case
        if (activeOrbs.Count >= maxOrbsCount)
        {
            activeOrbs.RemoveAt(0);

            // Move second orb to first position
            firstActiveOrb.sprite = secondActiveOrb.sprite;
            Destroy(firstActiveOrbPrefab);
            firstActiveOrbPrefab = secondActiveOrbPrefab;
            firstActiveOrbPrefab.transform.position = firstActiveOrbPrefabLocation.transform.position;

            // Add new orb to second position
            secondActiveOrb.sprite = orbSprite;
            secondActiveOrbPrefab = Instantiate(orbPrefab, secondActiveOrbPrefabLocation.transform.position, Quaternion.identity);
        }
        else
        {
            // Determine which slot to fill
            bool isFirstSlotEmpty = !firstActiveOrb.isActiveAndEnabled;
            UnityEngine.UI.Image targetImage = isFirstSlotEmpty ? firstActiveOrb : secondActiveOrb;
            GameObject targetLocation = isFirstSlotEmpty ? firstActiveOrbPrefabLocation : secondActiveOrbPrefabLocation;

            // Set up the new orb
            targetImage.gameObject.SetActive(true);
            targetImage.sprite = orbSprite;

            // Handle prefab instantiation
            if (isFirstSlotEmpty)
            {
                if (firstActiveOrbPrefab != null) Destroy(firstActiveOrbPrefab);
                firstActiveOrbPrefab = Instantiate(orbPrefab, targetLocation.transform.position, Quaternion.identity);
            }
            else
            {
                if (secondActiveOrbPrefab != null) Destroy(secondActiveOrbPrefab);
                secondActiveOrbPrefab = Instantiate(orbPrefab, targetLocation.transform.position, Quaternion.identity);
            }
        }

        activeOrbs.Add(orb);
        Debug.Log($"Current orbs: {string.Join(", ", activeOrbs)}");
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

            Debug.Log(string.Join(", ", orbSet));

            currentSkill = skillBookSO.GetSkill(orbSet);

            if (currentSkill != null)
            {
                Debug.Log($"Crafted Skill: {currentSkill.skillName}");

                craftSkill.skillCooldown = craftSkill.skillMaxCooldown;

                if (currentSkill.skillIcon != null)
                {
                    activeSkillIcon.sprite = currentSkill.skillIcon;
                    activeSkillIcon.gameObject.SetActive(true);
                }
                else
                {
                    activeSkillIcon.gameObject.SetActive(false);
                }

                orbSet.Clear();
            }
        }
    }

    bool SkillButtonsPressed() => Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Space);

    void HandleSkillExecution()
    {
        if (currentSkill == null) return;

        if (currentSkill.skillCooldown > 0)
        {
            currentSkill.skillCooldown -= Time.deltaTime;
        }

        bool isSkillExecutionPressed = SkillButtonsPressed();

        if (isSkillExecutionPressed && !(currentSkill.skillCooldown > 0) && playerMana.mana >= currentSkill.requiredMana)
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCam.ScreenPointToRay(mousePos);
            RaycastHit hit;
            //LayerMask layerMask = LayerMask.GetMask("Enemy", "Wall");

            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {

                playerState = PlayerState.UsingSkill;

                Debug.Log($"{currentSkill.skillName} is used.");

                playerMana.ModifyMana(currentSkill);
                Debug.Log($" {playerMana.mana} MP left.");

                currentSkill.skillCooldown = currentSkill.skillMaxCooldown;
            }
        }
        else if (isSkillExecutionPressed && !(currentSkill.skillCooldown <= 0) && playerMana.mana >= currentSkill.requiredMana)
        {
            Debug.Log($"Remaining cooldown to use {currentSkill.skillName}: {currentSkill.skillCooldown} secs");
        }

        else if (isSkillExecutionPressed && !(currentSkill.skillCooldown > 0) && playerMana.mana < currentSkill.requiredMana)
        {
            Debug.Log("Not enough mana!");
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
                    //PlayHitEffect();
                }
                else return;
            }
        }
    }

    void HandleStates()
    {
        // Handle sprite flipping consistently across all states
        bool shouldFlipSprite;

        switch (playerState)
        {
            case PlayerState.Walking:
                shouldFlipSprite = direction.x < 0.02f; // When shouldFlipSprite is true (direction.x < 0.02f), it sets spriteRenderer.flipX = true
                animator.SetBool("isWalking", true);    // When shouldFlipSprite is false (direction.x >= 0.02f), it sets spriteRenderer.flipX = false
                break;

            case PlayerState.Attacking:
                shouldFlipSprite = direction.x < 0.02f;
                animator.SetTrigger("Attack");
                break;

            case PlayerState.UsingSkill:
                shouldFlipSprite = direction.x < 0.02f;

                // Use a dictionary or switch without repeating the check
                string triggerName = currentSkill?.skillName switch
                {
                    "Fireball" => "Fireball",
                    "Ice Nova" => "Ice Nova",
                    "Lightning Bolt" => "Lightning Bolt",
                    _ => ""
                };

                if (!string.IsNullOrEmpty(triggerName))
                    animator.SetTrigger(triggerName);
                break;

            case PlayerState.Idle:
            default:
                shouldFlipSprite = lastWalkingDirection < 0.02f;
                animator.SetBool("isWalking", false);
                break;
        }

        spriteRenderer.flipX = shouldFlipSprite;
    }
}
