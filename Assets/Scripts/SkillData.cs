using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/Skill")]
public class SkillData : ScriptableObject
{
    [Header("Basic Properties")]
    [Tooltip("If left empty, will use this asset's name")]
    public string skillName;
    public float cooldown;
    public int manaCost;

    [Header("Projectile Properties")]
    public GameObject projectilePrefab;
    public float range = 20f;
    public float damage = 50f;
    public float projectileSpeed = 10f;
    public float projectileLifetime = 5f;

    [Header("Animation Settings")]
    [Tooltip("Animation trigger name. If left empty, will use skillName as trigger.")]
    public string animationTrigger;

    private void OnValidate()
    {
        // Auto-fill skill name from asset name if empty
        if (string.IsNullOrEmpty(skillName))
        {
            skillName = this.name;
        }
    }

    // Get the correct animation trigger name
    public string GetAnimationTrigger()
    {
        return string.IsNullOrEmpty(animationTrigger) ? skillName : animationTrigger;
    }
}