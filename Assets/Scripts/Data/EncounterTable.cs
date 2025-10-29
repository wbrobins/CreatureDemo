using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEncounterTable", menuName = "Game/Encounter Table")]
public class EncounterTable : ScriptableObject
{
    [System.Serializable]
    public class Encounter
    {
        public CreatureBase creatureBase;
        public int minLevel;
        public int maxLevel;
        [Range(0f, 1f)] public float spawnChance = 1f;
    }

    public List<Encounter> possibleEncounters;
}
