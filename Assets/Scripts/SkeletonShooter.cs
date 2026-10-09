using UnityEngine;

public class SkeletonShooter : MonoBehaviour
{
    private SkeletonController skeleton;

    private void Start()
    {
        skeleton = transform.GetComponent<SkeletonController>();
    }

    public void Shoot()
    {
        if (skeleton.arrowPrefab == null)
        {
            return;
        }

        if (skeleton.currentArrow != null)
        {
            Destroy(skeleton.currentArrow);
        }

        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float distance = Random.Range(0f, skeleton.spawnRadius);
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
        Vector2 spawnPosition = (Vector2)transform.position + offset;

        skeleton.currentArrow = Instantiate(skeleton.arrowPrefab, spawnPosition, Quaternion.identity);
    }
}
