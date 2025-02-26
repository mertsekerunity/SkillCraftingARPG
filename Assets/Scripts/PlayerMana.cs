using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    PlayerController player;

    [SerializeField] int maxMana = 100;
    [SerializeField] float manaRegen = 1.5f; //per second
    
    public float mana;
    
    //public int Mana { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        mana = maxMana;
    }

    // Update is called once per frame
    void Update()
    {
        mana += manaRegen * Time.deltaTime;
        mana = Mathf.Clamp(mana, 0, maxMana);
    }

    public void ModifyMana(SkillData skill)
    {
        if (skill != null)
        {
            if (mana >= skill.manaCost)
            {
                mana -= skill.manaCost;
            }
        }
    }
}
