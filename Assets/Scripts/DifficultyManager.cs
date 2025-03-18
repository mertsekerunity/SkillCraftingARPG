using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    public string currentDifficulty { get; private set; }

    private string defaultDifficulty = "Normal";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicate managers
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep this manager across scenes

        LoadDifficulty();
    }

    private void Start()
    {

    }
    private void LoadDifficulty()
    {
        currentDifficulty = PlayerPrefs.GetString("Difficulty", defaultDifficulty); // Default to Normal if not set
        Debug.Log($"Difficulty set to: {currentDifficulty}");
    }

    public void HandleDifficulty()
    {
        float damage = 25f;
        float maxHealth = 100f;
        bool provokeAll = false;

        switch (currentDifficulty)
        {
            case "Easy":
                damage = 15f;
                maxHealth = 80f;
                provokeAll = false;
                break;

            case "Normal":
                damage = 25f;
                maxHealth = 100f;
                provokeAll = false;
                break;

            case "Hard":
                damage = 50f;
                maxHealth = 140f;
                provokeAll = true;
                break;
        }

        EnemyController[] enemies = FindObjectsOfType<EnemyController>();

        foreach (EnemyController enemy in enemies)
        {
            enemy.GetComponent<EnemyAttack>().damage = damage;
            enemy.GetComponent<EnemyHealth>().maxHealth = maxHealth;

            //if (provokeAll)
            //{
            //    enemy.DamageTaken(); // All enemies chase the player on Hard mode
            //}
        }
    }

}
