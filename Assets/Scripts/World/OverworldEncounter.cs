using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldEncounter : MonoBehaviour
{
    public float encounterRate = .5f;
    private int steps;

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.W))
        {
            steps++;
            if (Random.value < encounterRate)
            {
                TriggerBattle();
            }
        }*/
    }
    
    void TriggerBattle()
    {
        var controller = FindFirstObjectByType<GameController>();

        //controller.StartBattle();
    }
}
