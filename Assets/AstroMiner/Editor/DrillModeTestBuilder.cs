using System;
using AstroMiner.Flight;
using AstroMiner.Mining;
using AstroMiner.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering;

namespace AstroMiner.Editor
{
    // Author one repeatable control fixture. This is not runtime excavation or world generation.
    public static class DrillModeTestBuilder
    {
        private const string Root = "Assets/AstroMiner";
        private const string ShipPath = Root + "/Prefabs/ShipDrillPrototype.prefab";
        private const string RockPath = Root + "/Prefabs/DrillTestAsteroid.prefab";
        private const string ProfilePath = Root + "/Settings/DrillModePrototype.asset";
        private const string InteriorPath = Root + "/Tiles/PrototypeInterior.asset";

        [MenuItem("Astro Miner/Setup/Add Drill Mode Test", true)]
        private static bool CanCreate() => !EditorApplication.isPlayingOrWillChangePlaymode &&
            AssetDatabase.LoadAssetAtPath<GameObject>(ShipPath) == null;

        [MenuItem("Astro Miner/Setup/Add Drill Mode Test")]
        public static void Create()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (EditorApplication.isPlayingOrWillChangePlaymode || scene.isDirty ||
                scene.path != Root + "/Scenes/FlightTest.unity")
                throw new InvalidOperationException("Open a clean, stopped FlightTest before setup.");
            foreach (string path in new[] { ShipPath, RockPath, ProfilePath, InteriorPath })
                if (AssetDatabase.LoadMainAssetAtPath(path) != null)
                    throw new InvalidOperationException("Existing drill asset preserved: " + path);
            var shipSource = AssetDatabase.LoadAssetAtPath<GameObject>(Root + "/Prefabs/ShipPlaceholder.prefab");
            var rockSource = AssetDatabase.LoadAssetAtPath<GameObject>(Root + "/Prefabs/StaticAsteroid.prefab");
            GameObject oldShip = null, oldRock = null;
            foreach (var root in scene.GetRootGameObjects())
            {
                var source = PrefabUtility.GetCorrespondingObjectFromSource(root);
                if (source == shipSource) oldShip = root;
                if (source == rockSource) oldRock = root;
            }
            var camera = UnityEngine.Object.FindAnyObjectByType<ShipCameraFollow>();
            if (oldShip == null || oldRock == null || camera == null)
                throw new InvalidOperationException("Expected flight ship, static asteroid and follow camera.");
            if (!AssetDatabase.IsValidFolder(Root + "/Settings"))
                AssetDatabase.CreateFolder(Root, "Settings");
            var profile = ScriptableObject.CreateInstance<DrillModeProfile>();
            AssetDatabase.CreateAsset(profile, ProfilePath);
            var material = AssetDatabase.LoadAssetAtPath<Material>(Root + "/Art/Prototype/PrototypeUnlit.mat");

            var preview = EditorSceneManager.NewPreviewScene();
            GameObject shipAsset, rockAsset;
            try
            {
                var ship = (GameObject)PrefabUtility.InstantiatePrefab(shipSource, preview);
                ship.name = "Ship - Flight and Drill Prototype";
                var emitter = new GameObject("Drill Emitter - beam preview").transform;
                emitter.SetParent(ship.transform, false);
                emitter.localPosition = new Vector3(0f, 1.6f);
                var drill = ship.AddComponent<ShipDrillController>();
                SetReference(drill, "_profile", profile);
                SetReference(drill, "_emitter", emitter);
                ship.AddComponent<ShipDrillDebug>();
                var feedback = ship.AddComponent<ShipDrillFeedback>();
                var ring = MakeLine(ship.transform, "Drill Mode Indicator", material, 48, true, 0.035f, true);
                var beam = MakeLine(ship.transform, "Laser Working Profile - preview only", material, 4, true, 0.055f, true);
                beam.startColor = beam.endColor = new Color(0.15f, 0.9f, 1f, 0.85f);
                var jets = new LineRenderer[4];
                Vector3[] starts = { new Vector3(0f,-1.2f), new Vector3(-0.7f,0.2f),
                    new Vector3(0f,1.6f), new Vector3(0.7f,0.2f) };
                Vector3[] exhaust = { Vector3.down, Vector3.left, Vector3.up, Vector3.right };
                for (int i = 0; i < jets.Length; i++)
                {
                    jets[i] = MakeLine(ship.transform, "Drill RCS Translation " + i, material, 2, false, 0.12f, false);
                    jets[i].SetPosition(0, starts[i]);
                    jets[i].SetPosition(1, starts[i] + exhaust[i] * 0.4f);
                    jets[i].startColor = new Color(0.9f, 0.85f, 0.55f);
                    jets[i].endColor = new Color(1f, 0.55f, 0.15f, 0.2f);
                }
                SetReference(feedback, "_modeRing", ring);
                SetReference(feedback, "_beamPreview", beam);
                var feedbackData = new SerializedObject(feedback);
                var jetArray = feedbackData.FindProperty("_rcsJets");
                jetArray.arraySize = 4;
                for (int i = 0; i < 4; i++) jetArray.GetArrayElementAtIndex(i).objectReferenceValue = jets[i];
                feedbackData.ApplyModifiedPropertiesWithoutUndo();
                shipAsset = PrefabUtility.SaveAsPrefabAsset(ship, ShipPath);

                var rock = (GameObject)PrefabUtility.InstantiatePrefab(rockSource, preview);
                rock.name = "Asteroid - Prepared Drill Corridor";
                var map = rock.GetComponentInChildren<Tilemap>();
                var interior = ScriptableObject.CreateInstance<Tile>();
                interior.name = "Prototype Interior";
                interior.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/Art/Prototype/RockCell.png");
                interior.colliderType = Tile.ColliderType.None;
                interior.color = new Color(0.035f, 0.055f, 0.07f);
                AssetDatabase.CreateAsset(interior, InteriorPath);
                var floor = new GameObject("Interior - nonblocking prepared tunnel", typeof(Tilemap), typeof(TilemapRenderer));
                floor.transform.SetParent(rock.transform, false);
                var floorMap = floor.GetComponent<Tilemap>();
                var floorRenderer = floor.GetComponent<TilemapRenderer>();
                floorRenderer.sharedMaterial = map.GetComponent<TilemapRenderer>().sharedMaterial;
                floorRenderer.sortingOrder = 3;
                int removed = 0;
                foreach (Vector3Int cell in map.cellBounds.allPositionsWithin)
                {
                    if (!map.HasTile(cell)) continue;
                    Vector2 p = map.GetCellCenterLocal(cell);
                    float distance = Mathf.Min(DistanceToSegment(p, new Vector2(0f,-11f), new Vector2(0f,3f)),
                        DistanceToSegment(p, new Vector2(0f,3f), new Vector2(6f,3f)));
                    // Include intersecting cells, so the authored corridor has at least radius 2.1.
                    if (distance > 2.1f + Mathf.Sqrt(2f) * 0.125f) continue;
                    map.SetTile(cell, null);
                    floorMap.SetTile(cell, interior);
                    removed++;
                }
                map.GetComponent<TilemapCollider2D>().ProcessTilemapChanges();
                map.GetComponent<CompositeCollider2D>().GenerateGeometry();
                var marker = MakeLine(rock.transform, "Prepared Corridor Entrance", material, 3, true, 0.07f, false);
                marker.SetPosition(0, new Vector3(-0.45f,-11.3f));
                marker.SetPosition(1, new Vector3(0f,-10.6f));
                marker.SetPosition(2, new Vector3(0.45f,-11.3f));
                marker.startColor = marker.endColor = Color.cyan;
                marker.enabled = true;
                rockAsset = PrefabUtility.SaveAsPrefabAsset(rock, RockPath);
                Debug.Log("Prepared drill fixture: " + removed + " cells cleared in authoring, not mined.");
            }
            finally { EditorSceneManager.ClosePreviewScene(preview); }

            int group = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Set up drill control test");
            var newShip = (GameObject)PrefabUtility.InstantiatePrefab(shipAsset, scene);
            var newRock = (GameObject)PrefabUtility.InstantiatePrefab(rockAsset, scene);
            newShip.name = "Ship - Flight and Drill Prototype";
            newRock.name = "Asteroid - Prepared Drill Corridor";
            Undo.RegisterCreatedObjectUndo(newShip, "Add drill ship");
            Undo.RegisterCreatedObjectUndo(newRock, "Add prepared asteroid");
            newShip.transform.position = new Vector3(18f, 4f);
            newRock.transform.position = new Vector3(18f, 16f);
            var cameraData = new SerializedObject(camera);
            cameraData.FindProperty("_target").objectReferenceValue = newShip.transform;
            cameraData.ApplyModifiedProperties();
            Undo.RecordObject(camera.transform, "Move test camera");
            camera.transform.position = new Vector3(18f, 4f, -10f);
            Undo.DestroyObjectImmediate(oldShip);
            Undo.DestroyObjectImmediate(oldRock);
            Undo.CollapseUndoOperations(group);
            EditorSceneManager.MarkSceneDirty(scene);
            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveScene(scene);
            Selection.activeGameObject = newShip;
            Debug.Log("Drill control fixture ready. R mode, W/S move, A/D turn, C brake; LMB preview only.");
        }

        private static float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
        {
            Vector2 delta = b - a;
            float t = Mathf.Clamp01(Vector2.Dot(p - a, delta) / delta.sqrMagnitude);
            return Vector2.Distance(p, a + delta * t);
        }

        private static LineRenderer MakeLine(Transform parent, string name, Material material,
            int count, bool loop, float width, bool worldSpace)
        {
            var line = new GameObject(name, typeof(LineRenderer)).GetComponent<LineRenderer>();
            line.transform.SetParent(parent, false);
            line.sharedMaterial = material;
            line.positionCount = count;
            line.loop = loop;
            line.useWorldSpace = worldSpace;
            line.startWidth = line.endWidth = width;
            line.numCapVertices = 2;
            line.sortingOrder = 20;
            line.shadowCastingMode = ShadowCastingMode.Off;
            line.receiveShadows = false;
            line.enabled = false;
            return line;
        }

        private static void SetReference(UnityEngine.Object target, string name, UnityEngine.Object value)
        {
            var data = new SerializedObject(target);
            data.FindProperty(name).objectReferenceValue = value;
            data.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
