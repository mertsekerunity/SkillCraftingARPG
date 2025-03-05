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

    public float health;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        health += healthRegen * Time.deltaTime;
        health = Mathf.Clamp(health, 0, maxHealth);
        healthDisplay.fillAmount = health / maxHealth;
        healthText.text = $"MP: {(int)health} / {maxHealth}";
    }
}
