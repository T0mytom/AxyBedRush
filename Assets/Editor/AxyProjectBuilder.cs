using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AxyProjectBuilder
{
    private const string ArtFolder = "Assets/Art";
    private const string PrefabFolder = "Assets/Prefabs";
    private const string SceneFolder = "Assets/Scenes";
    private const string ArrowPrefabPath = PrefabFolder + "/Arrow.prefab";
    private const string ScenePath = SceneFolder + "/Axy.unity";

    [MenuItem("Axy/Build Axy Scene")]
    public static void BuildAll()
    {
        EnsureFolders();
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

        GameObject arrowPrefab = CreateArrowPrefab();
        CreateScene(arrowPrefab);
        RegisterSceneInBuildSettings();

        AssetDatabase.SaveAssets();
        Debug.Log("[Axy] Escena, prefab y referencias construidos correctamente.");
    }

    private static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder(PrefabFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        if (!AssetDatabase.IsValidFolder(SceneFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Scenes");
        }
    }

    private static Sprite LoadSprite(string spriteName)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtFolder}/{spriteName}.png");
    }

    private static Sprite EnsureGeneratedSprite(string fileName, Color fill, Color border)
    {
        string path = $"{ArtFolder}/{fileName}.png";

        if (!File.Exists(path))
        {
            const int size = 32;
            Texture2D texture = new Texture2D(size, size);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    bool edge = x < 2 || y < 2 || x >= size - 2 || y >= size - 2;
                    texture.SetPixel(x, y, edge ? border : fill);
                }
            }

            texture.Apply();
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static GameObject CreateArrowPrefab()
    {
        GameObject arrow = new GameObject("Arrow");
        arrow.tag = "Arrow";

        SpriteRenderer spriteRenderer = arrow.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = LoadSprite("Arrow");
        spriteRenderer.sortingOrder = 8;

        CircleCollider2D collider = arrow.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.35f;

        arrow.AddComponent<ArrowController>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(arrow, ArrowPrefabPath);
        Object.DestroyImmediate(arrow);
        return prefab;
    }

    private static void CreateScene(GameObject arrowPrefab)
    {
        UnityEngine.SceneManagement.Scene scene =
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateCamera();
        CreateGameManager();
        CreateWalls();
        CreatePlayer();
        CreateGazer();
        CreateSkeleton(arrowPrefab);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
    }

    private static void CreateCamera()
    {
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);

        Camera camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.09f, 0.11f, 0.16f, 1f);
        camera.nearClipPlane = 0.01f;
        camera.farClipPlane = 100f;

        cameraObject.AddComponent<AudioListener>();
    }

    private static void CreateGameManager()
    {
        GameObject manager = new GameObject("GameManager");
        manager.AddComponent<GameManager>();
    }

    private static void CreateWalls()
    {
        Sprite wallSprite = EnsureGeneratedSprite(
            "Wall", new Color(0.33f, 0.27f, 0.21f), new Color(0.17f, 0.13f, 0.1f));
        Sprite exitSprite = EnsureGeneratedSprite(
            "Exit", new Color(0.16f, 0.76f, 0.35f), new Color(0.92f, 0.95f, 0.42f));

        CreateWall("Wall_Top", wallSprite, new Vector2(0f, 4.75f), new Vector2(17f, 0.5f));
        CreateWall("Wall_Bottom", wallSprite, new Vector2(0f, -4.75f), new Vector2(17f, 0.5f));
        CreateWall("Wall_Left", wallSprite, new Vector2(-8.25f, 0f), new Vector2(0.5f, 10f));
        CreateWall("Wall_Right", wallSprite, new Vector2(8.25f, 0f), new Vector2(0.5f, 10f));

        CreateExit(exitSprite);
    }

    private static void CreateWall(string name, Sprite sprite, Vector2 position, Vector2 size)
    {
        GameObject wall = new GameObject(name);
        wall.tag = "Wall";
        wall.transform.position = position;
        wall.transform.localScale = new Vector3(size.x, size.y, 1f);

        SpriteRenderer spriteRenderer = wall.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 2;

        BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;
    }

    private static void CreateExit(Sprite sprite)
    {
        GameObject exit = new GameObject("Exit");
        exit.tag = "Exit";
        exit.transform.position = new Vector3(7f, 3.5f, 0f);
        exit.transform.localScale = new Vector3(1.4f, 1.4f, 1f);

        SpriteRenderer spriteRenderer = exit.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 6;

        BoxCollider2D collider = exit.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        exit.AddComponent<ExitController>();
    }

    private static void CreatePlayer()
    {
        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = new Vector3(-6f, -3f, 0f);

        SpriteRenderer spriteRenderer = player.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = LoadSprite("Axy");
        spriteRenderer.sortingOrder = 10;

        Rigidbody2D body = player.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Dynamic;
        body.gravityScale = 0f;
        body.freezeRotation = true;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;

        BoxCollider2D collider = player.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.7f, 0.7f);

        PlayerController controller = player.AddComponent<PlayerController>();
        controller.speed = 5f;

        player.AddComponent<InputController>();
        player.AddComponent<PhysicsController>();
        player.AddComponent<StateController>();
        player.AddComponent<AudioController>();
    }

    private static void CreateGazer()
    {
        GameObject gazer = new GameObject("Gazer");
        gazer.tag = "Enemy";
        gazer.transform.position = new Vector3(2f, 0f, 0f);

        SpriteRenderer spriteRenderer = gazer.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = LoadSprite("Gazer");
        spriteRenderer.sortingOrder = 5;

        Rigidbody2D body = gazer.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.freezeRotation = true;

        BoxCollider2D collider = gazer.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.8f, 0.8f);

        GazerController controller = gazer.AddComponent<GazerController>();
        controller.speed = 2f;
        controller.maxY = 3f;
        controller.minY = -3f;

        gazer.AddComponent<GazerMovement>();
        gazer.AddComponent<GazerState>();
    }

    private static void CreateSkeleton(GameObject arrowPrefab)
    {
        GameObject skeleton = new GameObject("Skeleton");
        skeleton.tag = "Enemy";
        skeleton.transform.position = new Vector3(6f, 1.5f, 0f);

        SpriteRenderer spriteRenderer = skeleton.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = LoadSprite("Skeleton");
        spriteRenderer.sortingOrder = 5;

        BoxCollider2D collider = skeleton.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.8f, 0.8f);

        SkeletonController controller = skeleton.AddComponent<SkeletonController>();
        controller.arrowPrefab = arrowPrefab;
        controller.shootInterval = 2.5f;
        controller.spawnRadius = 3f;

        skeleton.AddComponent<SkeletonShooter>();
        skeleton.AddComponent<SkeletonTimer>();
    }

    private static void RegisterSceneInBuildSettings()
    {
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(ScenePath, true)
        };
    }
}
