using UnityEngine;

public class GazerController : MonoBehaviour
{
    public float speed = 2f;
    public float maxY = 3f;
    public float minY = -3f;

    [HideInInspector] public bool movingUp = true;
}
