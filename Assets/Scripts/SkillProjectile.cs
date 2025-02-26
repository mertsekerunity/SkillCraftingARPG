using UnityEngine;

public class SkillProjectile : MonoBehaviour
{
    // These will be set by the spawner based on SkillData
    [HideInInspector] public float damage;
    [HideInInspector] public float speed;
    [HideInInspector] public float lifetime;

    private void Start()
    {
        // Destroy the projectile after lifetime to prevent memory leaks
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if we hit an enemy
        if (other.TryGetComponent<EnemyHealth>(out var enemyHealth))
        {
            // Apply damage
            enemyHealth.TakeDamage(damage);
            Debug.Log($"Projectile hit for {damage} damage");

            // Destroy the projectile after hitting
            Destroy(gameObject);
        }
    }
}
