using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float angularSpeed = 5f;
    [SerializeField] float craftingMaxCooldown = 0.4f;

    [SerializeField] SkillBook skillBookData; // Reference to the SkillBook ScriptableObject
    SkillData currentSkill; // Stores the currently crafted skill

    PlayerMana playerMana;
    Camera mainCam;
    Rigidbody rb;

    List<Orb> activeOrbs = new List<Orb>();

    Vector3 targetPoint = new Vector3();
    int maxOrbsCount;
    float craftingCooldown;
    float currentSkillCooldown = 0f; // Track cooldown separately

    void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        playerMana = GetComponent<PlayerMana>();

        maxOrbsCount = System.Enum.GetValues(typeof(Orb)).Length; // Number of available orb types
    }

    void Update()
    {
        HandleOrbSelection();
        HandleSkillCrafting();
        HandleSkillExecution();
    }

    void FixedUpdate()
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
        }
    }
}
