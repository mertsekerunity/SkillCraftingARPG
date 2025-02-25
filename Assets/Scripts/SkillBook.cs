using System.Collections.Generic;
using static UnityEditor.Rendering.FilterWindow;

public static class SkillBook
{
    static Dictionary<HashSet<Orb>, Skill> skillBook = new Dictionary<HashSet<Orb>, Skill>(HashSet<Orb>.CreateSetComparer());


    static void CreateSkills()
    {
        skillBook.Add(new HashSet<Orb> { Orb.Quas, Orb.Quas}, new Skill("Fireball", 15f, 30));
        skillBook.Add(new HashSet<Orb> {Orb.Wex, Orb.Wex }, new Skill("Ice Nova", 10f, 20));
        skillBook.Add(new HashSet<Orb> {Orb.Quas, Orb.Wex }, new Skill("Lightning bolt", 4f, 8));
    }

    public static Dictionary<HashSet<Orb>, Skill> GetSkills()
    {
        CreateSkills();
        return skillBook;
    }

}   