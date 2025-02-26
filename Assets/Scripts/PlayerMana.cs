using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    [SerializeField] int maxMana = 100;
    [SerializeField] float manaRegen = 1.5f; // per second

    public float mana;

    void Start()
    {
        mana = maxMana;
    }

    void Update()
    {
        mana += manaRegen * Time.deltaTime;
        mana = Mathf.Clamp(mana, 0, maxMana);
    }

    // Support both SkillData and Skill objects for flexibility
    public void ModifyMana(SkillData skillData)
    {
        if (skillData != null && mana >= skillData.manaCost)
        {
            mana -= skillData.manaCost;
        }
    }
}
