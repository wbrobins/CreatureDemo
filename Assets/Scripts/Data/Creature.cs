using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

[System.Serializable]
public class Creature
{
    [SerializeField] private CreatureBase creatureBase;
    [SerializeField] private string creature_id;
    [SerializeField] private int level;
    [SerializeField] private int hp;
    [SerializeField] private int maxHP;
    [SerializeField] private int experience;
    [SerializeField] private List<Move> moves;

    // Read-only properties for runtime access
    public CreatureBase Base => creatureBase;
    public string CreatureId => creature_id;
    public int Level => level;
    public int HP => hp;
    public int MaxHP => maxHP;
    public int Experience => experience;
    public List<Move> Moves => moves;

    public bool IsFainted => hp <= 0;

    // Constructor for runtime initialization
    public Creature(string id, int pLevel, int pHp = -1, int pExperience = 0,  List<Move> learnedMoves = null)
    {
        creature_id = id;
        creatureBase = Resources.Load<CreatureBase>($"Data/CreatureBases/" + id);
        Debug.Log(creatureBase);
        level = pLevel;

        maxHP = CalculateMaxHP();

        if(pExperience == 0)
        {
            experience = level * level * level;
        }
        else
        {
            experience = pExperience;
        }

        if(pHp == -1)
        {
          hp = maxHP;  
        }
        else
        {
            hp = pHp;
        }
        
        if (learnedMoves != null)
        {
            moves = learnedMoves;
        }
        else if (learnedMoves == null || !learnedMoves.Any())
        {
            moves = new List<Move>();
            foreach (var moveBase in Base.LearnableMoves)
            {
                moves.Add(new Move(moveBase.moveBase.name));
                if (moves.Count >= 4) break;
            }
        }
    }

    private int CalculateMaxHP()
    {
        return Mathf.FloorToInt(((Base.MaxHP * 2f * Level) / 100f) + Level + 10);
    }

    public int Attack => Mathf.FloorToInt((Base.Atk * 2f * Level) / 100f + 5);
    public int Defense => Mathf.FloorToInt((Base.Def * 2f * Level) / 100f + 5);
    public int Speed => Mathf.FloorToInt((Base.Spd * 2f * Level) / 100f + 5);

    public void TakeDamage(int damage)
    {
        hp = Mathf.Max(hp - damage, 0);
    }

    public void Heal(int amount)
    {
        hp = Mathf.Min(hp + amount, maxHP);
    }

    public void ResetHP()
    {
        hp = maxHP;
    }

    public void GetExperience(int pExperience)
    {
        experience += pExperience;
        Debug.Log("Gained " + pExperience + " exp!");
        while (experience >= (level * level * level))
        {
            level += 1;
        }
        
        foreach(LearnableMove m in creatureBase.LearnableMoves)
        {
            if(level >= m.level && !moves.Exists(x => x.MoveId == m.moveBase.name))
            {
                moves.Add(new Move(m.moveBase.name));
            }
        }
    }

    public Move GetRandomMove()
    {
        if (moves.Count == 0) return null;
        return moves[Random.Range(0, moves.Count)];
    }
}
