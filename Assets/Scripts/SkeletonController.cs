using UnityEngine;

public class SkeletonController : MonoBehaviour
{
    public GameObject arrowPrefab;
    public float shootInterval = 2.5f;
    public float spawnRadius = 3f;

    [HideInInspector] public float shootTimer = 1.5f;
    [HideInInspector] public GameObject currentArrow;
}
