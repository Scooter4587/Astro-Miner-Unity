using System;
using System.IO;
using AstroMiner.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace AstroMiner.Editor
{
    public static class FlightTestSceneBuilder
    {
        private const string Root = "Assets/AstroMiner";
        private const string Art = Root + "/Art/Prototype";
        private const string ScenePath = Root + "/Scenes/FlightTest.unity";
        private const float TileSize = 64f;

        [MenuItem("Astro Miner/Setup/Create Flight Test Scene", true)]
        private static bool CanCreateScene() => !EditorApplication.isPlayingOrWillChangePlaymode && !File.Exists(ScenePath);

        [MenuItem("Astro Miner/Setup/Create Flight Test Scene")]
        public static void CreateScene()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Stop Play Mode before creating the scene.");
            if (File.Exists(ScenePath))
                throw new InvalidOperationException("FlightTest already exists; open it from Project instead. Existing work is preserved.");
            for (int i = 0; i < SceneManager.sceneCount; i++)
                if (SceneManager.GetSceneAt(i).isDirty)
                    throw new InvalidOperationException("Save your open scenes before creating FlightTest.");

            Directory.CreateDirectory(Art);
            Directory.CreateDirectory(Root + "/Scenes");
            Directory.CreateDirectory(Root + "/Prefabs");
            AssetDatabase.Refresh();
            Material material = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default"));
            AssetDatabase.CreateAsset(material, Art + "/PrototypeUnlit.mat");

            Sprite square = MakeSprite("Square", 16, 16f, (x, y) => Color.white);
            Sprite ring = MakeSprite("Ring", 128, 128f, (x, y) => {
                float radius = new Vector2(x - 0.5f, y - 0.5f).magnitude;
                return new Color(1f, 1f, 1f, 1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.018f, 0.03f, Mathf.Abs(radius - 0.4f))));
            });
            Sprite triangle = MakeSprite("Triangle", 128, 64f, (x, y) => {
                bool inside = y >= 0.08f && y <= 0.92f && Mathf.Abs(x - 0.5f) < (0.92f - y) * 0.52f;
                return new Color(1f, 1f, 1f, inside ? 1f : 0f);
            });
            Sprite hull = MakeSprite("ShipHull", 128, 64f, (x, y) => {
                float halfWidth = Mathf.Lerp(0.34f, 0.20f, y);
                bool inside = y > 0.07f && y < 0.9f && Mathf.Abs(x - 0.5f) < halfWidth;
                return new Color(1f, 1f, 1f, inside ? 1f : 0f);
            });
            Sprite haze = MakeSprite("DistantHaze", 512, 8f, (u, v) => {
                float a = Mathf.Sin(u * Mathf.PI * 2f + Mathf.Sin(v * Mathf.PI * 2f));
                float b = Mathf.Cos((u + v) * Mathf.PI * 4f);
                float cloud = Mathf.Pow(Mathf.Clamp01((a + b + 1f) / 3f), 3f);
                return new Color(0.20f, 0.26f, 0.40f, cloud * 0.28f);
            });
            Sprite far = MakeStars("StarsFar", 380, 2401, 0.55f, 1.0f);
            Sprite middle = MakeStars("StarsMiddle", 160, 8407, 0.8f, 1.25f);
            Sprite near = MakeStars("StarsNear", 60, 1709, 1.0f, 1.8f);

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener), typeof(UniversalAdditionalCameraData));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            Camera camera = cameraObject.GetComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 12f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.012f, 0.022f, 0.043f, 1f);
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 100f;
            cameraObject.AddComponent<CameraPreviewPan>();

            GameObject background = new GameObject("Background - repeating parallax layers");
            CreateLayer(background.transform, "Distant Haze", haze, 0.03f, -40, camera.transform, material);
            CreateLayer(background.transform, "Far Stars", far, 0.07f, -30, camera.transform, material);
            CreateLayer(background.transform, "Middle Stars", middle, 0.20f, -20, camera.transform, material);
            CreateLayer(background.transform, "Near Stars", near, 0.45f, -10, camera.transform, material);

            GameObject world = new GameObject("World - fixed reference objects");
            GameObject grid = Child(world.transform, "Reference Grid - 8 unit spacing");
            for (int coordinate = -80; coordinate <= 80; coordinate += 8)
            {
                Color color = coordinate == 0 ? new Color(0.20f, 0.50f, 0.55f, 0.60f) : new Color(0.18f, 0.38f, 0.48f, 0.38f);
                float width = coordinate == 0 ? 0.06f : 0.05f;
                AddSprite(grid.transform, "Vertical " + coordinate, square, new Vector2(coordinate, 0f), new Vector2(width, 160f), color, 0, material);
                AddSprite(grid.transform, "Horizontal " + coordinate, square, new Vector2(0f, coordinate), new Vector2(160f, width), color, 0, material);
            }
            GameObject markers = Child(world.transform, "Navigation Markers");
            Vector2[] positions = { new Vector2(-16f, 0f), new Vector2(16f, 0f), new Vector2(0f, 8f), new Vector2(0f, -8f), new Vector2(-24f, -24f), new Vector2(24f, 24f), new Vector2(-40f, 24f), new Vector2(40f, -24f) };
            for (int i = 0; i < positions.Length; i++)
            {
                GameObject marker = Child(markers.transform, "Beacon " + (i + 1));
                marker.transform.localPosition = positions[i];
                Color color = i % 2 == 0 ? new Color(0.27f, 0.78f, 0.73f, 0.9f) : new Color(0.91f, 0.58f, 0.25f, 0.9f);
                AddSprite(marker.transform, "Ring", ring, Vector2.zero, Vector2.one * 1.2f, color, 3, material);
                AddSprite(marker.transform, "Cross X", square, Vector2.zero, new Vector2(0.55f, 0.035f), color, 4, material);
                AddSprite(marker.transform, "Cross Y", square, Vector2.zero, new Vector2(0.035f, 0.55f), color, 4, material);
            }
            AddSprite(world.transform, "Origin Ring", ring, Vector2.zero, Vector2.one * 5f, new Color(0.18f, 0.58f, 0.63f, 0.5f), 2, material);

            GameObject ship = new GameObject("Ship Placeholder - visual only");
            AddSprite(ship.transform, "Hull", hull, Vector2.zero, Vector2.one, new Color(0.49f, 0.61f, 0.65f), 10, material);
            AddSprite(ship.transform, "Cockpit", square, new Vector2(0f, 0.32f), new Vector2(0.33f, 0.46f), new Color(0.15f, 0.73f, 0.80f), 11, material);
            AddSprite(ship.transform, "Drill Nose - inactive", triangle, new Vector2(0f, 0.96f), new Vector2(0.57f, 0.53f), new Color(0.96f, 0.66f, 0.27f), 12, material);
            AddSprite(ship.transform, "Engine Mount - inactive", square, new Vector2(0f, -0.84f), new Vector2(0.56f, 0.23f), new Color(0.23f, 0.30f, 0.37f), 11, material);
            for (int i = -1; i <= 1; i += 2)
                AddSprite(ship.transform, "RCS Mount " + i, square, new Vector2(i * 0.66f, -0.24f), new Vector2(0.14f, 0.36f), new Color(0.80f, 0.57f, 0.28f), 11, material);
            PrefabUtility.SaveAsPrefabAssetAndConnect(ship, Root + "/Prefabs/ShipPlaceholder.prefab", InteractionMode.AutomatedAction);

            foreach (GameObject item in scene.GetRootGameObjects())
                Undo.RegisterCreatedObjectUndo(item, "Create Astro Miner flight test");
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            Selection.activeGameObject = ship;
            SceneView view = SceneView.lastActiveSceneView;
            if (view != null)
            {
                view.in2DMode = true;
                view.LookAt(Vector3.zero, Quaternion.identity, 18f, true);
            }
            Debug.Log("Astro Miner: FlightTest created. Preview camera: arrows, Shift, Home. Ship flight and mining are not implemented.");
        }

        private static GameObject Child(Transform parent, string name)
        {
            GameObject result = new GameObject(name);
            result.transform.SetParent(parent, false);
            return result;
        }

        private static void CreateLayer(Transform parent, string name, Sprite sprite, float ratio, int order, Transform camera, Material material)
        {
            GameObject layer = Child(parent, name);
            ParallaxLayer2D parallax = layer.AddComponent<ParallaxLayer2D>();
            SerializedObject settings = new SerializedObject(parallax);
            settings.FindProperty("_cameraTransform").objectReferenceValue = camera;
            settings.FindProperty("_motionRatio").floatValue = ratio;
            settings.FindProperty("_tileSize").floatValue = TileSize;
            settings.ApplyModifiedPropertiesWithoutUndo();
            for (int y = -1; y <= 1; y++)
                for (int x = -1; x <= 1; x++)
                    AddSprite(layer.transform, "Tile " + x + "," + y, sprite, new Vector2(x * TileSize, y * TileSize), Vector2.one, Color.white, order, material);
        }

        private static void AddSprite(Transform parent, string name, Sprite sprite, Vector2 position, Vector2 scale, Color color, int order, Material material)
        {
            GameObject result = Child(parent, name);
            result.transform.localPosition = position;
            result.transform.localScale = new Vector3(scale.x, scale.y, 1f);
            SpriteRenderer renderer = result.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = order;
            renderer.sharedMaterial = material;
        }

        private static Sprite MakeStars(string name, int count, int seed, float minRadius, float maxRadius)
        {
            const int size = 1024;
            Color[] pixels = new Color[size * size];
            System.Random random = new System.Random(seed);
            for (int star = 0; star < count; star++)
            {
                int x = random.Next(size);
                int y = random.Next(size);
                float radius = Mathf.Lerp(minRadius, maxRadius, (float)random.NextDouble());
                Color color = Color.Lerp(new Color(0.38f, 0.61f, 0.80f), new Color(0.94f, 0.91f, 0.78f), (float)random.NextDouble());
                float strength = Mathf.Lerp(0.35f, 0.95f, (float)random.NextDouble());
                int bounds = Mathf.CeilToInt(radius + 1f);
                for (int dy = -bounds; dy <= bounds; dy++)
                    for (int dx = -bounds; dx <= bounds; dx++)
                    {
                        float alpha = Mathf.Clamp01(radius + 0.7f - new Vector2(dx, dy).magnitude) * strength;
                        int index = ((y + dy + size) % size) * size + (x + dx + size) % size;
                        if (alpha > pixels[index].a) pixels[index] = new Color(color.r, color.g, color.b, alpha);
                    }
            }
            return SaveSprite(name, size, size / TileSize, pixels);
        }

        private static Sprite MakeSprite(string name, int size, float pixelsPerUnit, Func<float, float, Color> sample)
        {
            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    pixels[y * size + x] = sample((float)x / (size - 1), (float)y / (size - 1));
            return SaveSprite(name, size, pixelsPerUnit, pixels);
        }

        private static Sprite SaveSprite(string name, int size, float pixelsPerUnit, Color[] pixels)
        {
            string path = Art + "/" + name + ".png";
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.SetPixels(pixels);
            texture.Apply();
            File.WriteAllBytes(path, texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = pixelsPerUnit;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
    }
}
