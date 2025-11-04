using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -19f);

    void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        Vector3 pos = new Vector3(player.transform.position.x, player.transform.position.y, offset.z) + new Vector3(offset.x, offset.y, 0);

        Vector3 smoothedPos = Vector3.Lerp(transform.position, pos, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPos;
    }
}
