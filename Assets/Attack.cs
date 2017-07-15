using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack
{
    public AttackType type = AttackType.Attack;
    public BonusEffect effect = BonusEffect.None;
    public int damage = 10;
    public int turnsCharge = 0;
    public int bonusLength = 0;
    public string attackName = "Attack 1";
    public string specialString = "regular attacks dont need this!";
    public AttackObject attackObject;

    public Attack(AttackType t, int damage, int charge, string name, string specialDescription, int bonusLength)
    {
        this.type = t;
        this.damage = damage;
        this.turnsCharge = charge;
        this.attackName = name;
        this.specialString = specialDescription;
        this.bonusLength = bonusLength;
    }

    public string GetAttackDescription()
    {
        string bonus = "";

        if(effect != BonusEffect.None)
        {
            bonus = bonusLength > 1 ? "\n    + bonus: " + effect.ToString() + " for " + bonusLength + " turns.\n" :
                "\n    + bonus: " + effect.ToString() + " for " + bonusLength + " turn.\n";
        }
        else
        {
            bonus = "\n\n";
        }
        switch(type)
        {
            case AttackType.Attack:
                if (turnsCharge == 0)
                    return "Deals " + damage + " damage." + bonus;
                else
                    return "Charges for " + turnsCharge + " turns, then deals " + damage + " damage." + bonus;
            case AttackType.Special:
                if (effect != BonusEffect.None)
                    return bonusLength > 1 ? effect.ToString() + " for " + bonusLength + " turns.\n\n" :
                        effect.ToString() + " for " + bonusLength + " turn.\n\n";
                else return specialString + "\n\n";
            case AttackType.Passive:
                return "Passive: " + specialString + "\n\n";
        }

        return "blah blah blah";
    }

    public string GetAttackName()
    {
        return attackName;
    }
}
