using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    Animator animator;

    [SerializeField] float health = 100f;
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float destroyDelay = 1.5f;
    [SerializeField] Image healthDisplay;
    public bool IsEnemyDead { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        healthDisplay.fillAmount = health / maxHealth;
    }
    public void TakeDamage(float damage)
    {
        health -= damage;

        Debug.Log($"Remaining HP: {health}");

        animator.SetTrigger("DamageTaken");

        if(health <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        if (IsEnemyDead) return;

        IsEnemyDead = true;

        animator.SetTrigger("Death");

        Destroy(gameObject, destroyDelay);

        //handle death animation and other stuff
    }
}
