using System.Collections.Generic;
using static UnityEditor.Rendering.FilterWindow;

public static class SkillBook
{
    static Dictionary<HashSet<Orb>, Skill> skillBook = new Dictionary<HashSet<Orb>, Skill>(HashSet<Orb>.CreateSetComparer());


    static void CreateSkills()
    {
        skillBook.Add(new HashSet<Orb> { Orb.Quas, Orb.Quas}, new Skill("Fireball", 5f));
        skillBook.Add(new HashSet<Orb> {Orb.Wex, Orb.Wex }, new Skill("Ice Nova", 10f));
        skillBook.Add(new HashSet<Orb> {Orb.Quas, Orb.Wex }, new Skill("Lightning bolt", 4f));
    }

    public static Dictionary<HashSet<Orb>, Skill> GetSkills()
    {
        CreateSkills();
        return skillBook;
    }

}   