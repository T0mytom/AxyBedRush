using UnityEngine;

public class SkeletonTimer : MonoBehaviour
{
    private SkeletonController skeleton;
    private SkeletonShooter shooter;

    private void Start()
    {
        skeleton = transform.GetComponent<SkeletonController>();
        shooter = transform.GetComponent<SkeletonShooter>();
    }

    private void Update()
    {
        skeleton.shootTimer -= Time.deltaTime;

        if (skeleton.shootTimer > 0f)
        {
            return;
        }

        skeleton.shootTimer = skeleton.shootInterval;
        shooter.Shoot();
    }
}
