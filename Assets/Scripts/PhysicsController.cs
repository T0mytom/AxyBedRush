using UnityEngine;

public class PhysicsController : MonoBehaviour
{
    private PlayerController player;
    private Rigidbody2D body;

    private void Start()
    {
        player = transform.GetComponent<PlayerController>();
        body = transform.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!player.isAlive)
        {
            body.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 step = player.inputDirection * player.speed * Time.deltaTime;
        body.MovePosition(body.position + step);
    }
}
