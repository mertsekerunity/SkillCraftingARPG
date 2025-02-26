using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float angularSpeed = 5f;
    [SerializeField] float craftingMaxCooldown = 0.4f;
    [SerializeField] private SkillBook skillBookData;

    PlayerMana playerMana;
    Camera mainCam;
    Rigidbody rb;
    Skill skill;
    Skill craftSkill;


    List<Orb> activeOrbs = new List<Orb>();
    Dictionary<HashSet<Orb>, Skill> skillBook;

    Vector3 targetPoint = new Vector3();

    int maxOrbsCount = System.Enum.GetValues(typeof(Orb)).Length;
    float craftingCooldown;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        playerMana = GetComponent<PlayerMana>();
        craftSkill = new Skill("Craft", craftingMaxCooldown, 0);
    }

    // Update is called once per frame
    void Update()
    {
        HandleOrbSelection();
        HandleSkillCrafting();
        HandleSkillExecution();
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
            }
            else
            {
                return;
            }

            targetPoint.y = transform.position.y;
            float delta = moveSpeed * Time.deltaTime;
            float angularDelta = angularSpeed * Time.deltaTime;
            Vector3 direction = (targetPoint - rb.position).normalized;
            Vector3 newPos = rb.position + direction * delta;
            Vector3 newOrientation = Vector3.RotateTowards(rb.position, targetPoint, angularDelta, 0f);
            newOrientation.x = 0;
            newOrientation.z = 0;
            Quaternion QuaternionNewOrientation = Quaternion.Euler(newOrientation);

            rb.MovePosition(newPos);
            rb.MoveRotation(QuaternionNewOrientation);
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

        if (craftSkill.skillCooldown > 0)
        {
            craftSkill.skillCooldown -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.R) && craftSkill.skillCooldown <= 0)
        {
            SkillData skill = skillBookData.GetSkill(activeOrbs);

            if (skill != null && this.skill.skillName != skill.skillName)
            {
                Debug.Log($"Crafted Skill: {skill.skillName}");
                this.skill = new Skill(skill.skillName, skill.cooldown, skill.manaCost);
                craftSkill.skillCooldown = craftSkill.skillMaxCooldown;
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
            Debug.Log($"{skill.skillName} is used.");

            playerMana.ModifyMana(skill);
            Debug.Log($" {playerMana.mana} MP left.");

            skill.skillCooldown = skill.skillMaxCooldown;
        }
    }
}
