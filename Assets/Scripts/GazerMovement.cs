using UnityEngine;

public class GazerMovement : MonoBehaviour
{
    private GazerController gazer;

    private void Start()
    {
        gazer = transform.GetComponent<GazerController>();
    }

    private void Update()
    {
        Vector2 direction = gazer.movingUp ? new Vector2(0f, 1f) : new Vector2(0f, -1f);
        transform.Translate(direction * gazer.speed * Time.deltaTime);
    }
}
