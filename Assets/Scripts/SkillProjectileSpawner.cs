using UnityEngine;

public class SkillProjectileSpawner : MonoBehaviour
{
    [SerializeField] Vector3 spawnOffset = new Vector3(0, 1, 0);

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void SpawnProjectileForSkill(SkillData skillData)
    {
        if (skillData == null)
        {
            Debug.LogError("Null skillData passed to SpawnProjectileForSkill");
            return;
        }

        if (skillData.projectilePrefab == null)
        {
            Debug.LogError($"No projectile prefab specified for skill: {skillData.skillName}");
            return;
        }

        // Get target point from mouse position
        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * skillData.range;
        }

        // Calculate direction and spawn position
        Vector3 direction = (targetPoint - transform.position).normalized;
        Vector3 spawnPosition = transform.position + spawnOffset;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Instantiate projectile
        GameObject projectileObj = Instantiate(skillData.projectilePrefab, spawnPosition, targetRotation);

        // Configure the projectile using SkillData properties
        if (projectileObj.TryGetComponent<SkillProjectile>(out var projectile))
        {
            projectile.damage = skillData.damage;
            projectile.speed = skillData.projectileSpeed;
            projectile.lifetime = skillData.projectileLifetime;

            Debug.Log($"Spawned {skillData.skillName} projectile: damage={skillData.damage}");
        }
        else
        {
            Debug.LogError($"SkillProjectile component missing from prefab for skill: {skillData.skillName}");
            Destroy(projectileObj);
            return;
        }

        // Configure physics
        if (projectileObj.TryGetComponent<Rigidbody>(out var projectileRb))
        {
            projectileRb.velocity = direction * skillData.projectileSpeed;
            projectileRb.useGravity = false;
            projectileRb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            Debug.LogWarning($"No Rigidbody on projectile for skill: {skillData.skillName}");
        }
    }
}