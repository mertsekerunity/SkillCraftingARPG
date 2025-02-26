using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillBook", menuName = "Skills/SkillBook")]
public class SkillBook : ScriptableObject
{
    // previous approach used a static dictionary, meaning all skill data was loaded into memory at runtime whether or not it was needed.
    // But with ScriptableObjects, skills are loaded and referenced only when needed, reducing RAM consumption
    [System.Serializable]
    public struct SkillEntry
    {
        public List<Orb> combination;
        public SkillData skill;
    }

    // predefined list of skills always more efficient than a dictionary lookup with HashSet comparisons
    public List<SkillEntry> skillEntries = new List<SkillEntry>();

    public SkillData GetSkill(List<Orb> orbs)
    {
        foreach (var entry in skillEntries)
        {
            if (new HashSet<Orb>(entry.combination).SetEquals(orbs))
            {
                return entry.skill;
            }
        }
        return null;
    }
}
