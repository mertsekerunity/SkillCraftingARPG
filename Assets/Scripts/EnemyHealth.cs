using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float health = 100f;
    [SerializeField] float destroyDelay = 1.5f;

    public bool IsDead { get; private set; }

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }
    public void TakeDamage(float damage)
    {
        health -= damage;

        Debug.Log($"Remaining HP: {health}");

        animator.SetTrigger("DamageTaken");

        if (health <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        if (IsDead) return;

        IsDead = true;

        animator.SetTrigger("Death");

        Destroy(gameObject, destroyDelay);

        //handle death animation and other stuff
    }
}
