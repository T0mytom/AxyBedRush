using UnityEngine;

public class GazerState : MonoBehaviour
{
    private GazerController gazer;

    private void Start()
    {
        gazer = transform.GetComponent<GazerController>();
    }

    private void LateUpdate()
    {
        Vector3 position = transform.position;

        if (position.y >= gazer.maxY)
        {
            gazer.movingUp = false;
            position.y = gazer.maxY;
        }
        else if (position.y <= gazer.minY)
        {
            gazer.movingUp = true;
            position.y = gazer.minY;
        }

        transform.position = position;
    }
}
