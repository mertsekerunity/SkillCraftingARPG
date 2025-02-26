using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/Skill")]
// skills were hardcoded within a static method.
// If you wanted to change skills, you had to modify the code and recompile the entire game
// Now, skills are modular assets that can be edited directly in the Inspector without modifying code or recompiling the game
// Remember, future-proofing your code is always a good idea
public class SkillData : ScriptableObject
{
    public string skillName;
    public float cooldown;
    public int manaCost;
}
