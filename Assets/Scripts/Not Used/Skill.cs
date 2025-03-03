using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public string skillName;
    public float skillCooldown = 0f;
    public float skillMaxCooldown;
    public int requiredMana;
    public string animationName;
    public float skillRange;
    public float skillDamage;

    public Skill(string skillName, float skillMaxCooldown, int requiredMana, float skillRange, float skillDamage)
    {
        this.skillName = skillName;
        this.skillMaxCooldown = skillMaxCooldown;
        this.requiredMana = requiredMana;
        this.skillRange = skillRange;
        this.skillDamage = skillDamage;
    }
}
