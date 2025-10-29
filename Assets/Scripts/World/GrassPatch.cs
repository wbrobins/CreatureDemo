using UnityEngine;

public class GrassPatch : MonoBehaviour
{
    public EncounterTable encounterTable;

    public EncounterTable.Encounter GetRandomEncounter()
    {
        if (encounterTable == null || encounterTable.possibleEncounters.Count == 0)
        {
            Debug.Log("Something is null in the GetRandomEncounter function.");
            return null;
        }

        float roll = Random.value;
        Debug.Log($"Random roll: {roll}");
        float cumulative = 0f;

        foreach (var encounter in encounterTable.possibleEncounters)
        {
            cumulative += encounter.spawnChance;
            if (roll <= cumulative)
            {
                return encounter;
            }
        }

        return encounterTable.possibleEncounters[0];
    }
}
