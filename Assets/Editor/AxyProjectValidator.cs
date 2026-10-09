using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class AxyProjectValidator
{
    private const string ScenePath = "Assets/Scenes/Axy.unity";
    private const string ArrowPrefabPath = "Assets/Prefabs/Arrow.prefab";
    private const string ScriptsFolder = "Assets/Scripts";

    private static readonly string[] RequiredScripts =
    {
        "PlayerController", "InputController", "PhysicsController", "StateController", "AudioController",
        "GazerController", "GazerMovement", "GazerState",
        "SkeletonController", "SkeletonShooter", "SkeletonTimer",
        "ArrowController", "ExitController", "GameManager"
    };

    private static int failures;

    [MenuItem("Axy/Validate Project")]
    public static void Validate()
    {
        failures = 0;

        ValidateScripts();
        ValidateScene();

        if (failures == 0)
        {
            Debug.Log("[AxyValidator] VALIDACION CORRECTA: todos los requisitos estan listos.");
            return;
        }

        Debug.LogError($"[AxyValidator] {failures} comprobacion(es) fallida(s).");
        EditorApplication.Exit(1);
    }

    private static void ValidateScripts()
    {
        string[] scripts = Directory.GetFiles(ScriptsFolder, "*.cs", SearchOption.AllDirectories)
            .Select(Path.GetFileNameWithoutExtension)
            .ToArray();

        foreach (string script in RequiredScripts)
        {
            Check(scripts.Contains(script), $"Existe el script {script}.cs");
        }

        Check(Contains("PlayerController", "public float speed"), "La velocidad del jugador es publica");
        Check(Contains("PlayerController", "Rigidbody2D"), "El contenedor del jugador declara Rigidbody2D");

        Check(Contains("InputController", "Input.GetKey"), "InputController lee el teclado");
        Check(Contains("InputController", "transform.GetComponent<PlayerController>()"),
            "InputController obtiene el contenedor con transform.GetComponent");
        Check(Contains("InputController", "public Vector2") == false && Contains("InputController", "inputDirection"),
            "InputController escribe la direccion en el estado del contenedor");

        Check(Contains("PhysicsController", "Time.deltaTime"), "PhysicsController usa Time.deltaTime");
        Check(Contains("PhysicsController", "MovePosition"), "PhysicsController aplica el movimiento al Rigidbody2D");
        Check(Contains("PhysicsController", "transform.GetComponent<PlayerController>()"),
            "PhysicsController obtiene el contenedor con transform.GetComponent");

        Check(Contains("StateController", "transform.GetComponent<PlayerController>()"),
            "StateController obtiene el contenedor con transform.GetComponent");
        Check(Contains("StateController", "OnCollisionEnter2D"), "StateController detecta colisiones 2D");
        Check(Contains("StateController", "OnTriggerEnter2D"), "StateController detecta triggers 2D");
        Check(Contains("StateController", "CompareTag"), "StateController distingue muros, enemigos, flechas y salida");

        Check(Contains("AudioController", "AudioSource"), "AudioController usa un AudioSource");
        Check(Contains("AudioController", "Play"), "AudioController reproduce el sonido de colision");
        Check(Contains("AudioController", "transform.GetComponent<AudioSource>()"),
            "AudioController obtiene el AudioSource con transform.GetComponent");

        Check(Contains("GazerMovement", "Time.deltaTime"), "GazerMovement usa Time.deltaTime");
        Check(Contains("GazerMovement", "new Vector2(0"), "GazerMovement se mueve en vertical");
        Check(Contains("GazerState", "movingUp"), "GazerState actualiza el estado del movimiento");
        Check(Contains("GazerState", "maxY") && Contains("GazerState", "minY"), "GazerState respeta los limites de patrulla");

        Check(Contains("SkeletonShooter", "Instantiate("), "SkeletonShooter instancia la flecha");
        Check(Contains("SkeletonShooter", "Random.Range("), "SkeletonShooter usa posicion aleatoria");
        Check(Contains("SkeletonTimer", "Time.deltaTime"), "SkeletonTimer usa Time.deltaTime (temporizacion)");
        Check(Contains("SkeletonTimer", "shootTimer"), "SkeletonTimer controla la cadencia de disparo");

        Check(Contains("GameManager", "public void Win()"), "GameManager define la condicion de victoria");
        Check(Contains("GameManager", "public void Lose()"), "GameManager define la condicion de derrota");
        Check(Contains("GameManager", "SceneManager.LoadScene"), "GameManager reinicia la escena al perder");

        Check(Contains("ArrowController", "Time.deltaTime"), "ArrowController usa Time.deltaTime");
        Check(Contains("ExitController", "Time.deltaTime"), "ExitController usa Time.deltaTime");
    }

    private static void ValidateScene()
    {
        Check(File.Exists(ArrowPrefabPath), "El prefab de la flecha existe en Assets/Prefabs");

        GameObject arrowPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ArrowPrefabPath);
        Check(arrowPrefab != null, "El prefab de la flecha es un GameObject valido");

        if (arrowPrefab != null)
        {
            SpriteRenderer renderer = arrowPrefab.GetComponent<SpriteRenderer>();
            Check(renderer != null && renderer.sprite != null, "La flecha tiene sprite asignado");
            CircleCollider2D collider = arrowPrefab.GetComponent<CircleCollider2D>();
            Check(collider != null && collider.isTrigger, "La flecha tiene un CircleCollider2D de tipo trigger");
            Check(arrowPrefab.GetComponent<ArrowController>() != null, "La flecha tiene ArrowController");
            Check(arrowPrefab.CompareTag("Arrow"), "La flecha tiene el tag Arrow");
        }

        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Check(scene.IsValid() && scene.isLoaded, "La escena Axy.unity abre correctamente");
        Check(EditorBuildSettings.scenes.Any(s => s.path == ScenePath && s.enabled), "Axy.unity esta en Build Settings");

        Camera camera = UnityEngine.Object.FindAnyObjectByType<Camera>();
        Check(camera != null && camera.orthographic, "La camara es ortografica");
        Check(camera != null && Mathf.Approximately(camera.orthographicSize, 5f),
            "La camara abarca el area de juego (orthographicSize = 5)");

        Check(UnityEngine.Object.FindAnyObjectByType<GameManager>() != null, "Existe el GameManager del juego");

        ValidateWalls();
        ValidateExit();
        ValidatePlayer();
        ValidateGazer();
        ValidateSkeleton();
    }

    private static void ValidateWalls()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        Check(walls.Length >= 4, $"Hay al menos 4 muros visibles (encontrados: {walls.Length})");

        foreach (GameObject wall in walls)
        {
            SpriteRenderer renderer = wall.GetComponent<SpriteRenderer>();
            BoxCollider2D collider = wall.GetComponent<BoxCollider2D>();
            Check(renderer != null && renderer.sprite != null, $"El muro {wall.name} tiene sprite visible");
            Check(collider != null && !collider.isTrigger, $"El muro {wall.name} tiene BoxCollider2D solido");
        }
    }

    private static void ValidateExit()
    {
        GameObject[] exits = GameObject.FindGameObjectsWithTag("Exit");
        Check(exits.Length >= 1, "Existe un objeto de salida con el tag Exit");

        foreach (GameObject exit in exits)
        {
            BoxCollider2D collider = exit.GetComponent<BoxCollider2D>();
            SpriteRenderer renderer = exit.GetComponent<SpriteRenderer>();
            Check(collider != null && collider.isTrigger, $"La salida {exit.name} tiene un collider trigger");
            Check(renderer != null && renderer.sprite != null, $"La salida {exit.name} es visible");
            Check(exit.GetComponent<ExitController>() != null, $"La salida {exit.name} tiene ExitController");
        }
    }

    private static void ValidatePlayer()
    {
        PlayerController player = UnityEngine.Object.FindAnyObjectByType<PlayerController>();
        Check(player != null, "Existe el Player con PlayerController");

        if (player == null)
        {
            return;
        }

        Check(Mathf.Approximately(player.speed, 5f), "El Player tiene speed = 5");
        Check(player.GetComponent<SpriteRenderer>() != null && player.GetComponent<SpriteRenderer>().sprite != null,
            "El Player tiene sprite asignado");

        Rigidbody2D body = player.GetComponent<Rigidbody2D>();
        Check(body != null && body.bodyType == RigidbodyType2D.Dynamic, "El Player tiene Rigidbody2D dinamico");
        Check(body != null && Mathf.Approximately(body.gravityScale, 0f), "La gravedad del Player es 0");
        Check(player.GetComponent<Collider2D>() != null, "El Player tiene un Collider2D");

        Check(player.GetComponent<InputController>() != null, "El Player tiene InputController");
        Check(player.GetComponent<PhysicsController>() != null, "El Player tiene PhysicsController");
        Check(player.GetComponent<StateController>() != null, "El Player tiene StateController");
        Check(player.GetComponent<AudioController>() != null, "El Player tiene AudioController");
        Check(player.GetComponent<AudioSource>() != null, "El Player tiene un AudioSource");
    }

    private static void ValidateGazer()
    {
        GazerController gazer = UnityEngine.Object.FindAnyObjectByType<GazerController>();
        Check(gazer != null, "Existe el Gazer con GazerController");

        if (gazer == null)
        {
            return;
        }

        Check(gazer.CompareTag("Enemy"), "El Gazer tiene el tag Enemy");
        Check(Mathf.Approximately(gazer.maxY, 3f) && Mathf.Approximately(gazer.minY, -3f),
            "El Gazer patrulla entre -3 y 3");
        Check(gazer.GetComponent<SpriteRenderer>() != null && gazer.GetComponent<SpriteRenderer>().sprite != null,
            "El Gazer tiene sprite asignado");
        Check(gazer.GetComponent<Collider2D>() != null, "El Gazer tiene un Collider2D");
        Check(gazer.GetComponent<GazerMovement>() != null, "El Gazer tiene GazerMovement");
        Check(gazer.GetComponent<GazerState>() != null, "El Gazer tiene GazerState");
    }

    private static void ValidateSkeleton()
    {
        SkeletonController skeleton = UnityEngine.Object.FindAnyObjectByType<SkeletonController>();
        Check(skeleton != null, "Existe el Skeleton con SkeletonController");

        if (skeleton == null)
        {
            return;
        }

        Check(skeleton.arrowPrefab != null, "El Skeleton tiene arrowPrefab asignado");
        Check(Mathf.Approximately(skeleton.spawnRadius, 3f), "El radio de aparicion de la flecha es 3");
        Check(skeleton.CompareTag("Enemy"), "El Skeleton tiene el tag Enemy");
        Check(skeleton.GetComponent<SpriteRenderer>() != null && skeleton.GetComponent<SpriteRenderer>().sprite != null,
            "El Skeleton tiene sprite asignado");
        Check(skeleton.GetComponent<Collider2D>() != null, "El Skeleton tiene un Collider2D");
        Check(skeleton.GetComponent<SkeletonShooter>() != null, "El Skeleton tiene un script de disparo (SkeletonShooter)");
        Check(skeleton.GetComponent<SkeletonTimer>() != null, "El Skeleton tiene un script de temporizacion (SkeletonTimer)");
    }

    private static string Read(string scriptName)
    {
        return File.ReadAllText(Path.Combine(ScriptsFolder, scriptName + ".cs"));
    }

    private static bool Contains(string scriptName, string fragment)
    {
        return Read(scriptName).Contains(fragment, StringComparison.Ordinal);
    }

    private static void Check(bool condition, string label)
    {
        if (condition)
        {
            Debug.Log($"[AxyValidator] OK   {label}");
            return;
        }

        failures++;
        Debug.LogError($"[AxyValidator] FAIL {label}");
    }
}
