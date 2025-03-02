using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/New Skill")]
public class SkillSO : ScriptableObject
{
    [SerializeField] string skillName;
    [SerializeField] Sprite skillIcon;
    [SerializeField] float skillDamage;
    [SerializeField] float skillRange;
    [SerializeField] float skillMaxCooldown;
    [SerializeField] float requiredMana;
    [SerializeField] GameObject skillPrefab;

    [SerializeField] List<Orb> _requiredOrbsList;  // Exposed in Inspector as List
    public HashSet<Orb> requiredOrbs => new HashSet<Orb>(_requiredOrbsList);  // Internally, use HashSet for efficiency

}
