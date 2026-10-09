using UnityEngine;

public class StateController : MonoBehaviour
{
    private PlayerController player;
    private AudioController audioController;

    private void Start()
    {
        player = transform.GetComponent<PlayerController>();
        audioController = transform.GetComponent<AudioController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!player.isAlive)
        {
            return;
        }

        audioController.PlayHit();

        if (collision.collider.CompareTag("Enemy"))
        {
            Lose();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!player.isAlive)
        {
            return;
        }

        if (other.CompareTag("Arrow"))
        {
            audioController.PlayHit();
            Lose();
            return;
        }

        if (other.CompareTag("Exit"))
        {
            Win();
        }
    }

    private void Lose()
    {
        player.isAlive = false;
        player.Body.linearVelocity = Vector2.zero;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.Lose();
        }
    }

    private void Win()
    {
        player.isAlive = false;
        player.Body.linearVelocity = Vector2.zero;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.Win();
        }
    }
}
