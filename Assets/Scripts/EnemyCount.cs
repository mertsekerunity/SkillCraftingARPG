using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EnemyCount : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI enemiesText;
    EnemyHealth[] enemies;
    int maxEnemyCount;
    public int enemyCount;

    // Start is called before the first frame update
    void Start()
    {
        enemies = GetComponentsInChildren<EnemyHealth>();
        maxEnemyCount = enemies.Length;
    }

    // Update is called once per frame
    void Update()
    {
        enemies = GetComponentsInChildren<EnemyHealth>();
        enemyCount = enemies.Length;
        enemiesText.text = $"Enemies remaining: {enemyCount}";
        
        if(enemyCount == 0)
        {
            enemiesText.color = Color.green;
        }
    }
}
