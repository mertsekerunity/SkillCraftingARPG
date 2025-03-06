using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;
    [SerializeField] float healthRegen = 1.5f; //per second
    [SerializeField] Image healthDisplay;
    [SerializeField] TextMeshProUGUI healthText;

    Animator animator;

    public float health;
    public float destroyDelay = 1.2f;

    public bool IsPlayerDead { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsPlayerDead)
        {
            health += healthRegen * Time.deltaTime;
        }
        health = Mathf.Clamp(health, 0, maxHealth);
        healthDisplay.fillAmount = health / maxHealth;
        healthText.text = $"HP: {(int)health} / {maxHealth}";
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        Debug.Log($"Remaining HP: {(int)health}");

        animator.SetTrigger("DamageTaken");

        if (health <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        if (IsPlayerDead) return;

        IsPlayerDead = true;

        animator.SetTrigger("Death");

        Destroy(gameObject, destroyDelay);
    }
}
