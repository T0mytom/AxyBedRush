using UnityEngine;

public class ExitController : MonoBehaviour
{
    public float pulseSpeed = 3f;
    public float pulseAmount = 0.12f;

    private Vector3 baseScale;
    private float elapsed;

    private void Start()
    {
        baseScale = transform.localScale;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float factor = 1f + Mathf.Sin(elapsed * pulseSpeed) * pulseAmount;
        transform.localScale = baseScale * factor;
    }
}
