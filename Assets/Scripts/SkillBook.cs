using System.Collections.Generic;
using static UnityEditor.Rendering.FilterWindow;

public static class SkillBook
{
    static Dictionary<HashSet<Orb>, Skill> skillBook = new Dictionary<HashSet<Orb>, Skill>(HashSet<Orb>.CreateSetComparer());


    static void CreateSkills()
    {
        skillBook.Add(new HashSet<Orb> { Orb.Quas, Orb.Quas}, new Skill("Skill 1", 15f, 30, 1000f, 90f));
        skillBook.Add(new HashSet<Orb> {Orb.Wex, Orb.Wex }, new Skill("Skill 2", 10f, 20, 1000f, 70f));
        skillBook.Add(new HashSet<Orb> {Orb.Quas, Orb.Wex }, new Skill("Skill 3", 3f, 8, 600f, 20f));
    }

    public static Dictionary<HashSet<Orb>, Skill> GetSkills()
    {
        CreateSkills();
        return skillBook;
    }

}   