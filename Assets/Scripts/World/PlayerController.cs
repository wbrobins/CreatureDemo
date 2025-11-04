using System.Reflection.Emit;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    [SerializeField] private Vector2 input;

    private Vector2 lastDirection = Vector2.down;

    [SerializeField] private Rigidbody2D rb;
    
    [SerializeField] private Animator playerAnimator;

    public void HandleUpdate()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (input != Vector2.zero)
        {
            lastDirection = input;
            rb.linearVelocity = input * moveSpeed;
            playerAnimator.SetBool("isMoving", true);
            playerAnimator.SetFloat("moveX", input.x);
            playerAnimator.SetFloat("moveY", input.y);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            playerAnimator.SetBool("isMoving", false);
            playerAnimator.SetFloat("moveX", lastDirection.x);
            playerAnimator.SetFloat("moveY", lastDirection.y);
        }

        //Debug.Log(playerAnimator.GetBool("isMoving") + "X: " + playerAnimator.GetFloat("moveX") + " Y: " + playerAnimator.GetFloat("moveY"));
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
