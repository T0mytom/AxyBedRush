using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public float lifetime = 6f;
    public float spinSpeed = 120f;

    private void Update()
    {
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);

        lifetime -= Time.deltaTime;

        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
