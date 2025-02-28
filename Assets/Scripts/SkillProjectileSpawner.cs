using UnityEngine;

public class SkillProjectileSpawner : MonoBehaviour
{
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] Vector3 spawnOffset = new Vector3(0, 1, 0); // Offset relatively from the player position

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void SpawnProjectileForSkill(Skill skill)
    {
        // Get mouse position in world space
        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            // If raycast doesn't hit anything, project to a far distance
            targetPoint = ray.origin + ray.direction * 100f;
        }

        // Calculate direction from player to target point
        Vector3 playerPos = transform.position;
        Vector3 direction = (targetPoint - playerPos).normalized;

        // Calculate spawn position (slightly offset from player if needed)
        Vector3 spawnPosition = transform.position + spawnOffset;

        // Create rotation that looks in the direction of the target
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Instantiate fireball at player's position with the proper rotation
        GameObject fireball = Instantiate(fireballPrefab, spawnPosition, targetRotation);

        // Set the damage value directly from the skill
        SkillProjectile projectile = fireball.GetComponent<SkillProjectile>();
        if (projectile != null)
        {
            projectile.damage = skill.skillDamage;
            projectile.speed = projectileSpeed;
        }
        else
        {
            Debug.LogError("SkillProjectile component missing from fireball prefab!");
        }

        // Get the Rigidbody component of the fireball
        Rigidbody fireballRb = fireball.GetComponent<Rigidbody>();
        if (fireballRb != null)
        {
            // Set velocity in the calculated direction
            fireballRb.velocity = direction * projectileSpeed;

            // Ensure no gravity for a straight path
            fireballRb.useGravity = false;

            // Freeze rotation to prevent the projectile from spinning
            fireballRb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        Debug.Log($"Fireball instantiated for {skill.skillName} with damage: {skill.skillDamage}, Direction: {direction}");
    }
}