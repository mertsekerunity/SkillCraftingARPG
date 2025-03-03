using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SkillBook", menuName = "Skills/New SkillBook")]
public class SkillBookSO : ScriptableObject
{
    [SerializeField] List<SkillSO> skills; // A list of all skills, editable in the inspector

    public HashSet<SkillSO> Skills => new HashSet<SkillSO>(skills);  // Return a HashSet for internal use

    public SkillSO GetSkill(HashSet<Orb> orbs)
    {
        foreach (var skill in skills)
        {
            if (new HashSet<Orb>(skill.requiredOrbs).SetEquals(orbs))
            {
                return skill;
            }
        }
        return null;  // If no matching skill is found
    }
}
