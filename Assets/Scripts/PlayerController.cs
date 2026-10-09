using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    [HideInInspector] public Vector2 inputDirection;
    [HideInInspector] public bool isAlive = true;

    public Rigidbody2D Body { get; private set; }

    private void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
    }
}
