using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Win()
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;
        Debug.Log("[Axy] Victoria: el jugador alcanzo la salida.");
    }

    public void Lose()
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;
        Debug.Log("[Axy] Derrota: reiniciando la escena.");
        StartCoroutine(RestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        yield return new WaitForSeconds(0.35f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
