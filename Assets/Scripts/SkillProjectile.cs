using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillProjectile : MonoBehaviour
{
    public float damage;
    public float speed = 10f;
    public float lifetime = 5f;

    private void Start()
    {
        // Destroy the projectile after lifetime to prevent memory leaks
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if we hit an enemy
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            // Apply damage
            enemyHealth.TakeDamage(damage);

            // Destroy the projectile after hitting
            Destroy(gameObject);
        }
    }
}
