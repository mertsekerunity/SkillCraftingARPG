using System.Collections;
using System.Collections.Generic;
using andywiecko.BurstTriangulator;
using UnityEngine;
using UnityEngine.Playables;
using static UnityEngine.GraphicsBuffer;

public class SpawnSkillPrefab : MonoBehaviour
{
    private float projectileSpeed;
    private Camera mainCamera;
    private PlayerController playerController;
    [SerializeField] Vector3 spawnOffset = new Vector3(0, 1, 0); // Offset relatively from the player position

    float destroyDelay = 1.1f;

    void Start()
    {
        mainCamera = Camera.main;
        playerController = GetComponentInParent<PlayerController>();
    }
    
    public void HandleSkillPrefab()
    {
        if (playerController.currentSkill == null) return;

        if (playerController.currentSkill.isProjectile)
        {
            SpawnProjectile();
        }
        else
        {
            SpawnPrefab();
        }
    }
    void SpawnPrefab()
    {
        // Get mouse position in world space
        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Calculate direction from player to hit point
            Vector3 playerPos = GetComponentInParent<Transform>().transform.position;
            Vector3 targetPoint = hit.point;

            EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();

            // Calculate spawn position (slightly offset from player if needed)
            Vector3 spawnPosition = playerPos + spawnOffset;

            if(enemy != null)
            {
                targetPoint = enemy.transform.position;
                targetPoint.y = enemy.transform.position.y / 2;
            }
            else
            {
                targetPoint += spawnOffset;
            }

            Vector3 direction = (targetPoint - transform.position).normalized;
            //Quaternion targetRotation = Quaternion.LookRotation(direction);

            Quaternion targetRotation = playerController.currentSkill.skillPrefab.transform.rotation;

            // Instantiate fireball at player's position with the proper rotation
            GameObject currentSkillPrefab = Instantiate(playerController.currentSkill.skillPrefab, targetPoint, targetRotation);

            if (enemy == null)
            {
                Destroy(currentSkillPrefab,destroyDelay);
                return;
            }

            //Cache current skill data
            SkillDamage skillDamageComponent = currentSkillPrefab.GetComponent<SkillDamage>();

            skillDamageComponent.GetSkillData(playerController.currentSkill.skillDamage, playerController.currentSkill.skillRange);
        }
    }

    void SpawnProjectile()
    {
        // Get mouse position in world space
        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Calculate direction from player to hit point
            Vector3 playerPos = GetComponentInParent<Transform>().transform.position;
            Vector3 targetPoint = hit.point;

            EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();

            // Calculate spawn position (slightly offset from player if needed)
            Vector3 spawnPosition = playerPos + spawnOffset;

            Vector3 direction = (targetPoint - transform.position).normalized;
            //Quaternion targetRotation = Quaternion.LookRotation(direction);

            Quaternion targetRotation = playerController.currentSkill.skillPrefab.transform.rotation;

            // Instantiate fireball at player's position with the proper rotation

            projectileSpeed = playerController.currentSkill.projectileSpeed;
            GameObject currentSkillPrefab = Instantiate(playerController.currentSkill.skillPrefab, playerController.transform.position, targetRotation);

            Rigidbody currentSkillPrefabRb = currentSkillPrefab.GetComponent<Rigidbody>();

            if (currentSkillPrefabRb != null)
            {
                //direction.y = 0;
                currentSkillPrefabRb.velocity = direction * projectileSpeed;
                currentSkillPrefabRb.useGravity = false;
                //currentSkillPrefabRb.constraints = RigidbodyConstraints.FreezeRotation;
            }

            //Cache current skill data
            SkillDamage skillDamageComponent = currentSkillPrefab.GetComponent<SkillDamage>();

            skillDamageComponent.GetSkillData(playerController.currentSkill.skillDamage, playerController.currentSkill.skillRange);
        }
    }
}
