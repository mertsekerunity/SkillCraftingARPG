using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public string skillName;
    public float skillCooldown;

    public Skill(string skillName, float skillCooldown)
    {
        this.skillName = skillName;
        this.skillCooldown = skillCooldown;
    }
}
