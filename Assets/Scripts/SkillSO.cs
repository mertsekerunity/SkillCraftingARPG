using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/New Skill")]
public class SkillSO : ScriptableObject
{
    public float skillCooldown;
    public string skillName;
    public Sprite skillIcon;
    public float skillDamage;
    public float skillRange;
    public float skillMaxCooldown;
    public float requiredMana;
    public GameObject skillPrefab;
    public bool isProjectile;
    public float projectileSpeed;

    public bool IsReady()
    {
        return skillCooldown <= 0;
    }

    [SerializeField] List<Orb> _requiredOrbsList;  // Exposed in Inspector as List
    public HashSet<Orb> requiredOrbs => new HashSet<Orb>(_requiredOrbsList);  // Internally, use HashSet for efficiency

    public void UseSkill()
    {
        skillCooldown = skillMaxCooldown;
    }

    public void TickCooldown(float deltaTime)
    {
        if (skillCooldown > 0)
        {
            skillCooldown -= deltaTime;
            if (skillCooldown < 0)
            {
                skillCooldown = 0;
            }
        }
    }

    private void OnEnable()
    {
        skillCooldown = 0f;
    }

}
