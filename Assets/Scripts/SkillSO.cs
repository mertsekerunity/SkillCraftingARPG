using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/New Skill")]
public class SkillSO : ScriptableObject
{
    [SerializeField] private float _defaultSkillCooldown = 0f;

    [System.NonSerialized]
    public float skillCooldown = 0f;

    public string skillName;
    public Sprite skillIcon;
    public float skillDamage;
    public float skillRange;
    public float skillMaxCooldown;
    public float requiredMana;
    public GameObject skillPrefab;

    [SerializeField] List<Orb> _requiredOrbsList;  // Exposed in Inspector as List
    public HashSet<Orb> requiredOrbs => new HashSet<Orb>(_requiredOrbsList);  // Internally, use HashSet for efficiency


    private void OnEnable()
    {
        skillCooldown = _defaultSkillCooldown;
    }
}
