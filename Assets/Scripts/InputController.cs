using UnityEngine;

public class InputController : MonoBehaviour
{
    private PlayerController player;

    private void Start()
    {
        player = transform.GetComponent<PlayerController>();
    }

    private void Update()
    {
        player.inputDirection = ReadInput();
    }

    private Vector2 ReadInput()
    {
        if (!player.isAlive)
        {
            return Vector2.zero;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            horizontal += 1f;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            horizontal -= 1f;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            vertical += 1f;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            vertical -= 1f;
        }

        return new Vector2(horizontal, vertical).normalized;
    }
}
