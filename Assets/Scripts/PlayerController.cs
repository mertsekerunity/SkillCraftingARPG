using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Playables;
using UnityEngine.Rendering.LookDev;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] GameObject attackPrefab;
    [SerializeField] SkillSO craftSkill;
    [SerializeField] UnityEngine.UI.Image activeSkillIcon;
    [SerializeField] UnityEngine.UI.Image timerImage;
    [SerializeField] TextMeshProUGUI timerText;

    [SerializeField] UnityEngine.UI.Image firstActiveOrb;
    [SerializeField] UnityEngine.UI.Image secondActiveOrb;

    [SerializeField] Sprite quasSprite;
    [SerializeField] Sprite wexSprite;

    [SerializeField] GameObject quasPrefab;
    [SerializeField] GameObject wexPrefab;

    [SerializeField] GameObject firstActiveOrbPrefabLocation;
    [SerializeField] GameObject secondActiveOrbPrefabLocation;

    public float attackDamage = 15f;
    public float attackRange = 400f;
    public float attackProjectileSpeed = 75f;
    public float attackMaxCooldown = 0.8f;
    
    float attackCooldown;

    GameObject firstActiveOrbPrefab;
    GameObject secondActiveOrbPrefab;

    LevelManager levelManager;

    [SerializeField] SkillBookSO skillBookSO;

    List<SkillSO> skillsList;

    [HideInInspector] public Vector3 direction;

    PlayerMana playerMana;
    Camera mainCam;
    Rigidbody rb;
    public SkillSO currentSkill;
    Animator animator;
    SpriteRenderer spriteRenderer;
    
    PlayerState playerState = PlayerState.Idle;
    PlayerState previousState = PlayerState.Idle;

    public List<Orb> activeOrbs = new List<Orb>();

    float lastWalkingDirection;
    float lastAttackingDirection;
    float lastUsingSkillDirection;

    public int maxOrbsCount = System.Enum.GetValues(typeof(Orb)).Length;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        playerMana = GetComponent<PlayerMana>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        levelManager = FindObjectOfType<LevelManager>();
        skillsList = skillBookSO.GetSkillsList();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<PlayerHealth>().IsPlayerDead)
        {
            float destroyDelay = GetComponent<PlayerHealth>().destroyDelay; //need to destroy them at sync
            Destroy(firstActiveOrbPrefab, destroyDelay);
            Destroy(secondActiveOrbPrefab, destroyDelay);
            return;
        }

        if (FindObjectOfType<Pause>().isPaused) return;

        HandleSkillCooldowns();
        HandleOrbSelection();
        HandleSkillCrafting();
        HandleSkillExecution();
        HandleAttackExecution();
        HandleStates();

        if(currentSkill != null)
        {
            timerImage.fillAmount = currentSkill.skillCooldown / currentSkill.skillMaxCooldown;
            if(currentSkill.skillCooldown > 1)
            {
                timerText.text = ((int)currentSkill.skillCooldown).ToString();
            }
            else
            {
                timerText.text = "";
            }
            
        }
        else
        {
            timerImage.fillAmount = 0;
            timerText.text = "";
        }

        if (playerState != previousState)
        {
            previousState = playerState;
            //Debug.Log($"player state changed to: {playerState}");
        }
        
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        if (!Input.GetMouseButton(0))
        {
            playerState = PlayerState.Idle;
            return;
        }

        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCam.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit))
        {
            playerState = PlayerState.Walking;

            Vector3 targetPoint = hit.point;
            direction = (targetPoint - rb.position).normalized;
            direction.y = 0;             
            lastWalkingDirection = direction.x;

            float moveDistance = moveSpeed * Time.deltaTime;

            RaycastHit sweepHit;
            bool wouldCollide = rb.SweepTest(direction, out sweepHit, moveDistance);

            if (wouldCollide)
            {
                if (sweepHit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle") ||
                    sweepHit.collider.CompareTag("Obstacle") || sweepHit.collider.CompareTag("Enemy"))
                {
                    float adjustedDistance = Mathf.Max(0, sweepHit.distance - 0.1f);

                    if (adjustedDistance > 0.01f)
                    {
                        Vector3 newPos = rb.position + direction * adjustedDistance;
                        rb.MovePosition(newPos);
                    }

                    if (Vector3.Dot(direction, (sweepHit.point - rb.position).normalized) > 0.7f)
                    {
                        return;
                    }
                }
                else
                {
                    Vector3 newPos = rb.position + direction * moveDistance;
                    rb.MovePosition(newPos);
                }
            }
            else
            {
                Vector3 newPos = rb.position + direction * moveDistance;
                rb.MovePosition(newPos);
            }

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
        if(activeOrbs.Count >= maxOrbsCount)
        {
            activeOrbs.RemoveAt(0);

            firstActiveOrb.sprite = secondActiveOrb.sprite;

            if (activeOrbs[0] == Orb.Quas)
            {
                Destroy(firstActiveOrbPrefab);

                firstActiveOrbPrefab = Instantiate(quasPrefab, firstActiveOrbPrefabLocation.transform.position, Quaternion.identity);
            }
            else if (activeOrbs[0] == Orb.Wex)
            {
                Destroy(firstActiveOrbPrefab);

                firstActiveOrbPrefab = Instantiate(wexPrefab, firstActiveOrbPrefabLocation.transform.position, Quaternion.identity);
            }

            if (orb == Orb.Quas)
            {
                secondActiveOrb.sprite = quasSprite;

                if (secondActiveOrbPrefab != null) Destroy(secondActiveOrbPrefab);

                secondActiveOrbPrefab = Instantiate(quasPrefab, secondActiveOrbPrefabLocation.transform.position, Quaternion.identity);
            }
            else if (orb == Orb.Wex)
            {
                secondActiveOrb.sprite = wexSprite;

                if (secondActiveOrbPrefab != null) Destroy(secondActiveOrbPrefab);

                secondActiveOrbPrefab = Instantiate(wexPrefab, secondActiveOrbPrefabLocation.transform.position, Quaternion.identity);
            }
        }

        if (!firstActiveOrb.isActiveAndEnabled)
        {
            firstActiveOrb.gameObject.SetActive(true);

            if (orb == Orb.Quas)
            {
                firstActiveOrb.sprite = quasSprite;

                if (firstActiveOrbPrefab != null) Destroy(firstActiveOrbPrefab);

                firstActiveOrbPrefab = Instantiate(quasPrefab, firstActiveOrbPrefabLocation.transform.position, Quaternion.identity);
            }
            else if (orb == Orb.Wex)
            {
                firstActiveOrb.sprite = wexSprite;

                if (firstActiveOrbPrefab != null) Destroy(firstActiveOrbPrefab);

                firstActiveOrbPrefab = Instantiate(wexPrefab, firstActiveOrbPrefabLocation.transform.position, Quaternion.identity);
            }
        }
        else if(!secondActiveOrb.isActiveAndEnabled)
        {
            secondActiveOrb.gameObject.SetActive(true);

            if (orb == Orb.Quas)
            {
                secondActiveOrb.sprite = quasSprite;

                if (secondActiveOrbPrefab != null) Destroy(secondActiveOrbPrefab);

                secondActiveOrbPrefab = Instantiate(quasPrefab, secondActiveOrbPrefabLocation.transform.position, Quaternion.identity);
            }
            else if (orb == Orb.Wex)
            {
                secondActiveOrb.sprite = wexSprite;

                if (secondActiveOrbPrefab != null) Destroy(secondActiveOrbPrefab);

                secondActiveOrbPrefab = Instantiate(wexPrefab, secondActiveOrbPrefabLocation.transform.position, Quaternion.identity);
            }
        }

        activeOrbs.Add(orb);

        //Debug.Log($"Current orbs: {string.Join(", ", activeOrbs)}");
    }

    void HandleSkillCrafting()
    {
        if (activeOrbs.Count != maxOrbsCount) return;

        if(craftSkill.skillCooldown > 0)
        {
            craftSkill.skillCooldown -= Time.deltaTime;

            if(craftSkill.skillCooldown < 0)
            {
                craftSkill.skillCooldown = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && craftSkill.skillCooldown <= 0)
        {
            HashSet<Orb> orbSet = new HashSet<Orb>(activeOrbs);

            //Debug.Log(string.Join(", ", orbSet));

            currentSkill = skillBookSO.GetSkill(orbSet); 

            if(currentSkill != null)
            {
                //Debug.Log($"Crafted Skill: {currentSkill.skillName}");

                craftSkill.skillCooldown = craftSkill.skillMaxCooldown;

                if(currentSkill.skillIcon != null)
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

    bool SkillButtonsPressed()
    {
        return (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Space));
    }

    void HandleSkillCooldowns()
    {
        foreach (SkillSO skill in skillsList)
        {
            skill.TickCooldown(Time.deltaTime);
        }
    }

    void HandleSkillExecution()
    {
        if (currentSkill == null) return;

        bool isSkillExecutionPressed = SkillButtonsPressed();

        if (isSkillExecutionPressed && currentSkill.IsReady() && playerMana.mana >= currentSkill.requiredMana)
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCam.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {

                playerState = PlayerState.UsingSkill;

                //Debug.Log($"{currentSkill.skillName} is used.");

                playerMana.ModifyMana(currentSkill);
                //Debug.Log($" {playerMana.mana} MP left.");

                currentSkill.skillCooldown = currentSkill.skillMaxCooldown;
            }
        }
        else if (isSkillExecutionPressed && !currentSkill.IsReady() && playerMana.mana >= currentSkill.requiredMana)
        {
            //Debug.Log($"Remaining cooldown to use {currentSkill.skillName}: {currentSkill.skillCooldown} secs");
        }

        else if (isSkillExecutionPressed && currentSkill.IsReady() && playerMana.mana < currentSkill.requiredMana)
        {
            //Debug.Log("Not enough mana!");
        }
    }

    void HandleAttackExecution()  //attack icin mana olmali mi?
    {
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        if (activeOrbs.Count != maxOrbsCount) return;

        if (Input.GetMouseButtonDown(1) && !(attackCooldown > 0))
        {
            playerState = PlayerState.Attacking;
            attackCooldown = attackMaxCooldown;
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

                switch (currentSkill.skillName)
                {
                    case "Inferno":
                        animator.SetTrigger("Inferno"); //play skill 1 animation
                        break;
                    case "Tornado":
                        animator.SetTrigger("Tornado"); //play skill 2 animation
                        break;
                    case "Lightning Bolt":
                        animator.SetTrigger("Lightning Bolt"); //play skill 3 animation
                        break;
                }
                break;
            default:
                // add hard reset for all animations, not only for walking and after force idle 
                break;
        }
    }
}
