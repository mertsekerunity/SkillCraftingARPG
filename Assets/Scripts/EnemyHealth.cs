using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    Animator animator;

    [SerializeField] float health = 100f;
    [SerializeField] float destroyDelay = 1.5f;
    [SerializeField] Image healthDisplay;

    public float maxHealth = 100f;
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

        if(DifficultyManager.Instance.currentDifficulty != "Easy")
        {
            FindObjectOfType<EnemyController>().DamageTaken();
        }
        
        if(DifficultyManager.Instance.currentDifficulty == "Hard")
        {
            FindObjectOfType<EnemyController>().TriggerMassProvoke();
            //BroadcastMessage("DamageTaken");
        }

        //Debug.Log($"Remaining HP: {(int)health}");

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
    }
}
