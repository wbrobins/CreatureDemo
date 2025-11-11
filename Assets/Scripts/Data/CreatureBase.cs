using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct LearnableMove
{
    public MoveBase moveBase;
    public int level;
}

[CreateAssetMenu(fileName = "New Creature", menuName = "Creatures/Create new creature")]
public class CreatureBase : ScriptableObject
{
    [SerializeField] string creatureName;
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;
    [SerializeField] int maxHP;
    [SerializeField] int atk;
    [SerializeField] int def;
    [SerializeField] int spd;
    [SerializeField] List<LearnableMove> learnableMoves;
    [SerializeField] GameObject battlePrefab;

    public string Name => creatureName;
    public Sprite FrontSprite => frontSprite;
    public Sprite BackSprite => backSprite;
    public int MaxHP => maxHP;
    public int Atk => atk;
    public int Def => def;
    public int Spd => spd;
    public List<LearnableMove> LearnableMoves => learnableMoves;
    public GameObject BattlePrefab => battlePrefab;
}

