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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
