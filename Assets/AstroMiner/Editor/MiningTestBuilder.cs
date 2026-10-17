using System;
using System.Collections.Generic;
using AstroMiner.Mining;
using AstroMiner.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

namespace AstroMiner.Editor
{
    public static class MiningTestBuilder
    {
        private const string Root="Assets/AstroMiner";
        private const string Art=Root+"/Art/Prototype/Mining";
        private const string DefinitionPath=Root+"/Settings/PrototypeAsteroid.asset";
        private const string RockPath=Root+"/Prefabs/MineableAsteroid.prefab";
        private const string ShipPath=Root+"/Prefabs/ShipMiningPrototype.prefab";
        private const string LaserPath=Root+"/Settings/LaserMiningPrototype.asset";
        private const string DrillPath=Root+"/Settings/MiningDrillModePrototype.asset";

        [MenuItem("Astro Miner/Setup/Add Laser Mining Test")]
        public static void Create()
        {
            var scene=EditorSceneManager.GetActiveScene();
            if(EditorApplication.isPlayingOrWillChangePlaymode || scene.isDirty ||
                scene.path!=Root+"/Scenes/FlightTest.unity")
                throw new InvalidOperationException("Open a clean, stopped FlightTest before setup.");
            foreach(string path in new[]{DefinitionPath,RockPath,ShipPath,LaserPath,DrillPath})
                if(AssetDatabase.LoadMainAssetAtPath(path)!=null)
                    throw new InvalidOperationException("Existing mining asset preserved: "+path);
            GameObject oldShip=null,oldRock=null;
            foreach(var go in scene.GetRootGameObjects())
            {
                if(go.GetComponent<ShipDrillController>()!=null) oldShip=go;
                if(go.GetComponentInChildren<Tilemap>()!=null) oldRock=go;
            }
            if(oldShip==null || oldRock==null)
                throw new InvalidOperationException("Expected drill fixture ship and asteroid.");
            var camera=UnityEngine.Object.FindAnyObjectByType<ShipCameraFollow>();
            if(camera==null) throw new InvalidOperationException("Missing follow camera.");
            if(!AssetDatabase.IsValidFolder(Art)) AssetDatabase.CreateFolder(Root+"/Art/Prototype","Mining");
            var definition=ScriptableObject.CreateInstance<AsteroidDefinition>();
            var source=PrefabUtility.LoadPrefabContents(Root+"/Prefabs/StaticAsteroid.prefab");
            try
            {
                var map=source.GetComponentInChildren<Tilemap>(); map.CompressBounds();
                BoundsInt bounds=map.cellBounds;
                const float spacing=0.125f;
                Vector2 origin=(Vector2)map.CellToLocal(bounds.min)-Vector2.one*0.5f;
                int width=bounds.size.x*2+9, height=bounds.size.y*2+9;
                var values=new float[width*height];
                for(int y=0;y<height;y++) for(int x=0;x<width;x++)
                {
                    Vector2 point=origin+new Vector2(x,y)*spacing;
                    Vector2 g=point/0.25f-Vector2.one*0.5f;
                    int gx=Mathf.FloorToInt(g.x),gy=Mathf.FloorToInt(g.y);
                    float u=g.x-gx,v=g.y-gy;
                    float a=map.HasTile(new Vector3Int(gx,gy,0))?1f:-1f;
                    float b=map.HasTile(new Vector3Int(gx+1,gy,0))?1f:-1f;
                    float c=map.HasTile(new Vector3Int(gx+1,gy+1,0))?1f:-1f;
                    float d=map.HasTile(new Vector3Int(gx,gy+1,0))?1f:-1f;
                    float value=Mathf.Lerp(Mathf.Lerp(a,b,u),Mathf.Lerp(d,c,u),v)*0.25f;
                    values[x+y*width]=Mathf.Abs(value)<0.000001f?-0.000001f:value;
                }
                definition.SetAuthoringData(width,height,spacing,origin,values);
            }
            finally { PrefabUtility.UnloadPrefabContents(source); }
            AssetDatabase.CreateAsset(definition,DefinitionPath);
            var laser=ScriptableObject.CreateInstance<LaserMiningProfile>();
            AssetDatabase.CreateAsset(laser,LaserPath);
            var drill=UnityEngine.Object.Instantiate(
                AssetDatabase.LoadAssetAtPath<DrillModeProfile>(Root+"/Settings/DrillModePrototype.asset"));
            drill.name="MiningDrillModePrototype";
            var drillData=new SerializedObject(drill);
            drillData.FindProperty("_beamNearWidth").floatValue=4f;
            drillData.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.CreateAsset(drill,DrillPath);
            var shader=Shader.Find("AstroMiner/Prototype/Vertex Color Unlit");
            if(shader==null) throw new InvalidOperationException("Missing mesh surface shader.");
            var material=new Material(shader) {name="AsteroidSurfaceUnlit"};
            AssetDatabase.CreateAsset(material,Art+"/AsteroidSurfaceUnlit.mat");
            var contact=AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(Root+"/Settings/RockContact.physicsMaterial2D");
            // Use the contact asset already authored on the accepted asteroid.
            if(contact==null)
            {
                var original=AssetDatabase.LoadAssetAtPath<GameObject>(Root+"/Prefabs/StaticAsteroid.prefab");
                contact=original.GetComponentInChildren<CompositeCollider2D>().sharedMaterial;
            }
            var preview=EditorSceneManager.NewPreviewScene();
            GameObject rockAsset,shipAsset;
            try
            {
                var rock=new GameObject("Asteroid - Runtime Mining",typeof(Rigidbody2D),typeof(CompositeCollider2D));
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(rock,preview);
                rock.GetComponent<Rigidbody2D>().bodyType=RigidbodyType2D.Static;
                var composite=rock.GetComponent<CompositeCollider2D>();
                composite.geometryType=CompositeCollider2D.GeometryType.Polygons;
                composite.generationType=CompositeCollider2D.GenerationType.Manual;
                composite.vertexDistance=0.003f; composite.offsetDistance=0f;
                composite.sharedMaterial=contact;
                var state=definition.CreateState();
                var chunks=new List<AsteroidChunk>();
                const int size=32;
                for(int y=0;y<state.Height-1;y+=size) for(int x=0;x<state.Width-1;x+=size)
                {
                    var cells=new RectInt(x,y,Mathf.Min(size,state.Width-1-x),Mathf.Min(size,state.Height-1-y));
                    var chunkGo=new GameObject("Rock chunk "+x/size+","+y/size,
                        typeof(MeshFilter),typeof(MeshRenderer),typeof(PolygonCollider2D),typeof(AsteroidChunk));
                    chunkGo.transform.SetParent(rock.transform,false);
                    var surface=new AsteroidSurface(); surface.Build(state,cells);
                    var mesh=new Mesh {name="Rock chunk "+x/size+","+y/size};
                    surface.WriteMesh(mesh,definition.RockColor,true);
                    AssetDatabase.CreateAsset(mesh,Art+"/Rock_"+x/size+"_"+y/size+".asset");
                    chunkGo.GetComponent<MeshFilter>().sharedMesh=mesh;
                    var renderer=chunkGo.GetComponent<MeshRenderer>();
                    renderer.sharedMaterial=material; renderer.sortingOrder=4;
                    renderer.shadowCastingMode=ShadowCastingMode.Off; renderer.receiveShadows=false;
                    var collider=chunkGo.GetComponent<PolygonCollider2D>();
                    collider.compositeOperation=Collider2D.CompositeOperation.Merge;
                    surface.WriteCollider(collider);
                    var chunk=chunkGo.GetComponent<AsteroidChunk>(); chunk.SetAuthoringCells(cells); chunks.Add(chunk);
                }
                var floor=new GameObject("Interior - nonblocking original footprint",typeof(MeshFilter),typeof(MeshRenderer));
                floor.transform.SetParent(rock.transform,false);
                var floorSurface=new AsteroidSurface();
                floorSurface.Build(state,new RectInt(0,0,state.Width-1,state.Height-1));
                var floorMesh=new Mesh {name="Asteroid interior footprint"};
                floorSurface.WriteMesh(floorMesh,definition.InteriorColor,false);
                AssetDatabase.CreateAsset(floorMesh,Art+"/Interior.asset");
                floor.GetComponent<MeshFilter>().sharedMesh=floorMesh;
                var floorRenderer=floor.GetComponent<MeshRenderer>();
                floorRenderer.sharedMaterial=material; floorRenderer.sortingOrder=3;
                floorRenderer.shadowCastingMode=ShadowCastingMode.Off; floorRenderer.receiveShadows=false;
                var runtime=rock.AddComponent<AsteroidRuntime>();
                Reference(runtime,"_definition",definition);
                var serialized=new SerializedObject(runtime);
                var list=serialized.FindProperty("_chunks"); list.arraySize=chunks.Count;
                for(int i=0;i<chunks.Count;i++) list.GetArrayElementAtIndex(i).objectReferenceValue=chunks[i];
                serialized.ApplyModifiedPropertiesWithoutUndo();
                composite.GenerateGeometry();
                rockAsset=PrefabUtility.SaveAsPrefabAsset(rock,RockPath);

                var ship=(GameObject)PrefabUtility.InstantiatePrefab(
                    AssetDatabase.LoadAssetAtPath<GameObject>(Root+"/Prefabs/ShipDrillPrototype.prefab"),preview);
                ship.name="Ship - Flight Drill and Mining";
                Reference(ship.GetComponent<ShipDrillController>(),"_profile",drill);
                var miner=ship.AddComponent<ShipLaserMiner>(); Reference(miner,"_profile",laser);
                shipAsset=PrefabUtility.SaveAsPrefabAsset(ship,ShipPath);
            }
            finally { EditorSceneManager.ClosePreviewScene(preview); }
            int group=Undo.GetCurrentGroup(); Undo.SetCurrentGroupName("Set up real laser mining");
            var newShip=(GameObject)PrefabUtility.InstantiatePrefab(shipAsset,scene);
            var newRock=(GameObject)PrefabUtility.InstantiatePrefab(rockAsset,scene);
            newShip.name="Ship - Flight Drill and Mining"; newRock.name="Asteroid - Runtime Mining";
            newShip.transform.position=new Vector3(18f,4f); newRock.transform.position=new Vector3(18f,16f);
            Undo.RegisterCreatedObjectUndo(newShip,"Add mining ship");
            Undo.RegisterCreatedObjectUndo(newRock,"Add mineable asteroid");
            Reference(newShip.GetComponent<ShipLaserMiner>(),"_asteroid",newRock.GetComponent<AsteroidRuntime>());
            PrefabUtility.RecordPrefabInstancePropertyModifications(newShip.GetComponent<ShipLaserMiner>());
            Undo.RecordObject(camera,"Follow mining ship"); Reference(camera,"_target",newShip.transform);
            PrefabUtility.RecordPrefabInstancePropertyModifications(camera);
            Undo.RecordObject(camera.transform,"Move camera"); camera.transform.position=new Vector3(18f,4f,-10f);
            Undo.DestroyObjectImmediate(oldShip); Undo.DestroyObjectImmediate(oldRock);
            Undo.CollapseUndoOperations(group);
            EditorSceneManager.MarkSceneDirty(scene); AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(scene);
            Selection.activeGameObject=newShip;
            Debug.Log("Real laser mining fixture ready: R mode, LMB mines, W/S advance and reverse.");
        }

        private static void Reference(UnityEngine.Object target,string name,UnityEngine.Object value)
        {
            var data=new SerializedObject(target); data.FindProperty(name).objectReferenceValue=value;
            data.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
