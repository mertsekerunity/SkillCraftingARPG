using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public string skillName;
    public float skillCooldown = 0f;
    public float skillMaxCooldown;
    public int requiredMana;

    public Skill(string skillName, float skillMaxCooldown, int requiredMana)
    {
        this.skillName = skillName;
        this.skillMaxCooldown = skillMaxCooldown;
        this.requiredMana = requiredMana;
    }
}
