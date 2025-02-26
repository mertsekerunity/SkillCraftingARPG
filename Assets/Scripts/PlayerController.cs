using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float angularSpeed = 5f;
    [SerializeField] float craftingMaxCooldown = 0.4f;

    PlayerMana playerMana;
    Camera mainCam;
    Rigidbody rb;
    Skill skill;
    Skill craftSkill;
    Animator animator;
    SpriteRenderer spriteRenderer;

    PlayerState playerState = PlayerState.Idle;


    List<Orb> activeOrbs = new List<Orb>();
    Dictionary<HashSet<Orb>, Skill> skillBook;

    Vector3 targetPoint = new Vector3();
    Vector3 direction;
    float lastWalkingDirection;

    int maxOrbsCount = System.Enum.GetValues(typeof(Orb)).Length;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        playerMana = GetComponent<PlayerMana>();
        skillBook = SkillBook.GetSkills();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        craftSkill = new Skill("Craft", craftingMaxCooldown, 0);
    }

    // Update is called once per frame
    void Update()
    {
        HandleOrbSelection();
        HandleSkillCrafting();
        HandleSkillExecution();
        HandleStates();
        Debug.Log($"player state: {playerState}");
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

            if(Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                targetPoint = hit.point;
                playerState = PlayerState.Walking;
            }
            else
            {
                playerState = PlayerState.Idle;
                return;
            }

            targetPoint.y = transform.position.y;
            float delta = moveSpeed * Time.deltaTime;
            float angularDelta = angularSpeed * Time.deltaTime;
            direction = (targetPoint - rb.position).normalized;
            lastWalkingDirection = direction.x;
            Vector3 newPos = rb.position + direction * delta;
            //Vector3 newOrientation = Vector3.RotateTowards(rb.position, targetPoint, angularDelta, 0f);
            //newOrientation.x = 0;
            //newOrientation.z = 0;
            //Quaternion QuaternionNewOrientation = Quaternion.Euler(newOrientation);

            rb.MovePosition(newPos);
            //rb.MoveRotation(QuaternionNewOrientation); //game is 3D but player has 2D sprite so it is not useful
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
        if(activeOrbs.Count >= maxOrbsCount)
        {
            activeOrbs.RemoveAt(0);
        }

        activeOrbs.Add(orb);

        Debug.Log($"Current orbs: {string.Join(", ", activeOrbs)}");
    }

    void HandleSkillCrafting()
    {
        if (activeOrbs.Count != maxOrbsCount) return;

        if(craftSkill.skillCooldown > 0)
        {
            craftSkill.skillCooldown -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.R) && craftSkill.skillCooldown <= 0)
        {
            HashSet<Orb> orbSet = new HashSet<Orb>(activeOrbs);

            Debug.Log(string.Join(", ", orbSet));

            if (skillBook.TryGetValue(orbSet, out Skill skill))
            {
                if(this.skill != skill)
                {
                    Debug.Log($"Crafted Skill: {skill.skillName}");
                    orbSet.Clear(); //not necessary imo?
                    this.skill = skill;
                    craftSkill.skillCooldown = craftSkill.skillMaxCooldown;
                }
            }
        }
    }

    void HandleSkillExecution()
    {
        if (skill == null) return;

        if(skill.skillCooldown > 0)
        {
            skill.skillCooldown -= Time.deltaTime;
        }
        Debug.Log($"Remaining cooldown to use {skill.skillName}: {skill.skillCooldown} secs");

        if (Input.GetKeyDown(KeyCode.D) && !(skill.skillCooldown > 0) && playerMana.mana >= skill.requiredMana)
        {
            playerState = PlayerState.UsingSkill;
            Debug.Log($"{skill.skillName} is used.");

            playerMana.ModifyMana(skill);
            Debug.Log($" {playerMana.mana} MP left.");

            skill.skillCooldown = skill.skillMaxCooldown;
        }
    }

    void HandleStates()
    {
        switch (playerState)
        {
            case PlayerState.Idle:
                animator.SetBool("isWalking", false); // play idle animation

                if (lastWalkingDirection < 0)
                {
                    spriteRenderer.flipX = true;
                }
                else
                {
                    spriteRenderer.flipX = false;
                }
                
                break;
            case PlayerState.Walking:
                animator.SetBool("isWalking", true); // play walking animation

                if (direction.x < 0)
                {
                    spriteRenderer.flipX = true;
                }
                else
                {
                    spriteRenderer.flipX = false;
                }
                
                break;
            case PlayerState.Attacking:
                animator.SetTrigger("Attack"); //play attack animation

                break;
            case PlayerState.UsingSkill:

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
    void OnAnimationComplete()
    {
        animator.SetBool("isCastingSpell", false);
    }
}
