using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 input;

    public void HandleUpdate()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (input != Vector2.zero)
        {
            transform.Translate(input * moveSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out GrassPatch patch))
        {
            float encounterChance = .1f;
            if(Random.value < encounterChance)
            {
                var encounter = patch.GetRandomEncounter();
                if(encounter != null)
                {
                    int level = Random.Range(encounter.minLevel, encounter.maxLevel + 1);
                    var controller = FindFirstObjectByType<GameController>();
                    controller.StartBattle(level, encounter.creatureBase);
                }
            }
        }
    }
}
