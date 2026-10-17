using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace AstroMiner.Editor
{
    // One-time prototype authoring. The saved Tilemap is the source of solid rock cells.
    public static class StaticAsteroidBuilder
    {
        private const string Root = "Assets/AstroMiner";
        private const string PrefabPath = Root + "/Prefabs/StaticAsteroid.prefab";
        private const string TilePath = Root + "/Tiles/PrototypeRock.asset";
        private const string SpritePath = Root + "/Art/Prototype/RockCell.png";
        private const string MaterialPath = Root + "/Physics/RockContact.physicsMaterial2D";
        private const float CellSize = 0.25f;

        [MenuItem("Astro Miner/Setup/Add Static Test Asteroid", true)]
        private static bool CanCreate() => !EditorApplication.isPlayingOrWillChangePlaymode && !File.Exists(PrefabPath);

        [MenuItem("Astro Miner/Setup/Add Static Test Asteroid")]
        public static void Create()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (EditorApplication.isPlayingOrWillChangePlaymode || scene.isDirty ||
                scene.path != Root + "/Scenes/FlightTest.unity")
                throw new InvalidOperationException("Open a clean, stopped FlightTest before adding the asteroid.");
            foreach (string path in new[] { PrefabPath, TilePath, SpritePath, MaterialPath })
                if (File.Exists(path)) throw new InvalidOperationException("Existing asteroid asset preserved: " + path);

            Directory.CreateDirectory(Root + "/Tiles");
            Directory.CreateDirectory(Root + "/Physics");
            AssetDatabase.Refresh();
            Sprite sprite = CreateRockSprite();
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.name = "Prototype Rock";
            tile.sprite = sprite;
            tile.colliderType = Tile.ColliderType.Grid;
            tile.flags = TileFlags.LockTransform;
            AssetDatabase.CreateAsset(tile, TilePath);
            var contact = new PhysicsMaterial2D("Rock Contact") { friction = 0f, bounciness = 0f };
            AssetDatabase.CreateAsset(contact, MaterialPath);

            var asteroid = new GameObject("Asteroid - Static Prototype", typeof(Grid));
            Undo.RegisterCreatedObjectUndo(asteroid, "Add static asteroid");
            asteroid.GetComponent<Grid>().cellSize = new Vector3(CellSize, CellSize, 1f);
            asteroid.transform.position = new Vector3(18f, 16f, 0f);
            var rock = new GameObject("Rock - Tilemap", typeof(Tilemap), typeof(TilemapRenderer), typeof(Rigidbody2D));
            rock.transform.SetParent(asteroid.transform, false);
            rock.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            var map = rock.GetComponent<Tilemap>();
            var renderer = rock.GetComponent<TilemapRenderer>();
            renderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(Root + "/Art/Prototype/PrototypeUnlit.mat");
            renderer.sortingOrder = 5;
            renderer.mode = TilemapRenderer.Mode.Chunk;
            var composite = rock.AddComponent<CompositeCollider2D>();
            composite.geometryType = CompositeCollider2D.GeometryType.Polygons;
            composite.generationType = CompositeCollider2D.GenerationType.Manual;
            composite.vertexDistance = 0.005f;
            composite.offsetDistance = 0.001f;
            composite.sharedMaterial = contact;
            var source = rock.AddComponent<TilemapCollider2D>();
            source.compositeOperation = Collider2D.CompositeOperation.Merge;
            source.extrusionFactor = 0f;

            for (int y = -40; y < 40; y++)
            for (int x = -48; x < 48; x++)
            {
                if (!IsRock(x, y)) continue;
                Vector3Int cell = new Vector3Int(x, y, 0);
                map.SetTile(cell, tile);
                map.SetColor(cell, RockColor(x, y));
            }
            map.CompressBounds();
            source.ProcessTilemapChanges();
            composite.GenerateGeometry();
            if (composite.pathCount == 0) throw new InvalidOperationException("Asteroid collider was not generated.");
            PrefabUtility.SaveAsPrefabAssetAndConnect(asteroid, PrefabPath, InteractionMode.AutomatedAction);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Selection.activeGameObject = rock;
            Debug.Log("Astro Miner: static asteroid saved at (18,16). WASD/C flight unchanged; drilling is not implemented.");
        }

        private static bool IsRock(int x, int y)
        {
            float nx = (x + 0.5f) / 42f;
            float ny = (y + 0.5f) / 34f;
            float angle = Mathf.Atan2(ny, nx);
            float rim = 1f + 0.08f * Mathf.Sin(angle * 3f) +
                0.07f * Mathf.Sin(angle * 5f + 1.3f) + 0.035f * Mathf.Sin(angle * 9f);
            return nx * nx + ny * ny <= rim * rim;
        }

        private static Color RockColor(int x, int y)
        {
            float grain = Mathf.PerlinNoise((x + 107f) * 0.14f, (y + 211f) * 0.14f);
            float broad = Mathf.PerlinNoise((x + 307f) * 0.045f, (y + 97f) * 0.045f);
            float shade = 0.22f + grain * 0.28f + broad * 0.27f + (-x + y) * 0.0015f;
            // Surface markings only: darker basins are still solid rock, not openings.
            foreach (Vector3 crater in new[] { new Vector3(-16, 10, 13), new Vector3(15, -9, 11), new Vector3(8, 20, 7) })
            {
                float distance = Vector2.Distance(new Vector2(x, y), crater) / crater.z;
                if (distance < 1f) shade -= (1f - distance) * 0.24f;
                if (distance > 0.82f && distance < 1.08f) shade += 0.09f;
            }
            bool edge = !IsRock(x - 1, y) || !IsRock(x + 1, y) || !IsRock(x, y - 1) || !IsRock(x, y + 1);
            if (edge) shade += 0.17f;
            return Color.Lerp(new Color(0.15f, 0.18f, 0.21f), new Color(0.62f, 0.64f, 0.61f), Mathf.Clamp01(shade));
        }

        private static Sprite CreateRockSprite()
        {
            var texture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            for (int y = 0; y < 32; y++)
            for (int x = 0; x < 32; x++)
            {
                float grain = 0.94f + 0.06f * Mathf.PerlinNoise(x * 0.38f + 7f, y * 0.38f + 3f);
                texture.SetPixel(x, y, new Color(grain, grain, grain, 1f));
            }
            texture.Apply();
            File.WriteAllBytes(SpritePath, texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(SpritePath, ImportAssetOptions.ForceSynchronousImport);
            var importer = (TextureImporter)AssetImporter.GetAtPath(SpritePath);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 128f;
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);
            importer.filterMode = FilterMode.Bilinear;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath);
        }
    }
}
