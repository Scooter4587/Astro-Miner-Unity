#if UNITY_EDITOR
using System.Collections;
using AstroMiner.Flight;
using AstroMiner.Mining;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace AstroMiner.PlayModeTests
{
    public sealed class LaserMiningTests
    {
        private GameObject _ship,_rock;
        private ShipDrillController _drill;
        private ShipLaserMiner _miner;
        private AsteroidRuntime _asteroid;
        private Rigidbody2D _body;
        private CapsuleCollider2D _hull;
        private CompositeCollider2D _solid;
        private float _oldStep;

        [SetUp]
        public void SetUp()
        {
            _oldStep=Time.fixedDeltaTime; Time.fixedDeltaTime=0.02f;
            _rock=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/AstroMiner/Prefabs/MineableAsteroid.prefab"),new Vector3(2000f,2000f),Quaternion.identity);
            _asteroid=_rock.GetComponent<AsteroidRuntime>(); _solid=_rock.GetComponent<CompositeCollider2D>();
            _ship=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/AstroMiner/Prefabs/ShipMiningPrototype.prefab"),new Vector3(2000f,1988f),Quaternion.identity);
            _ship.GetComponent<ShipKeyboardInput>().enabled=false;
            _drill=_ship.GetComponent<ShipDrillController>(); _miner=_ship.GetComponent<ShipLaserMiner>();
            var data=new SerializedObject(_miner);
            data.FindProperty("_asteroid").objectReferenceValue=_asteroid; data.ApplyModifiedPropertiesWithoutUndo();
            _body=_ship.GetComponent<Rigidbody2D>(); _hull=_ship.GetComponent<CapsuleCollider2D>();
            Physics2D.SyncTransforms();
        }

        [TearDown]
        public void TearDown()
        {
            if(_ship!=null) Object.DestroyImmediate(_ship);
            if(_rock!=null) Object.DestroyImmediate(_rock);
            Time.fixedDeltaTime=_oldStep;
        }

        private static IEnumerator Steps(int count)
        {
            for(int i=0;i<count;i++) yield return new WaitForFixedUpdate();
        }

        private IEnumerator Ready()
        {
            _drill.ToggleMode(); yield return Steps(15);
            Assert.That(_drill.Mode,Is.EqualTo(ShipControlMode.Drill));
        }

        private void CheckGeometry()
        {
            Assert.That(_asteroid.GeometryRevision,Is.EqualTo(_asteroid.State.Revision));
            var field=_asteroid.State;
            for(int y=-85;y<=75;y+=3) for(int x=-95;x<=95;x+=3)
            {
                Vector2 p=new Vector2(x*0.1f+0.011f,y*0.1f+0.019f);
                float value=field.Sample(p);
                if(Mathf.Abs(value)<0.015f) continue;
                Assert.That(_solid.OverlapPoint((Vector2)_rock.transform.position+p),
                    Is.EqualTo(value>0f),"Updated native geometry at "+p);
            }
        }

        [UnityTest]
        public IEnumerator LmbMinesOnlyInReadyDrillAndReleasePreservesTheHoleWithoutEditingTheAsset()
        {
            float before=_asteroid.State.TotalArea();
            _drill.SetBeamPreview(true); yield return Steps(12);
            Assert.That(_asteroid.State.Revision,Is.Zero);
            yield return Ready();
            _drill.SetBeamPreview(true); yield return Steps(15);
            Assert.That(_miner.RemovedArea,Is.GreaterThan(1f));
            Assert.That(before-_asteroid.State.TotalArea(),Is.EqualTo(_miner.RemovedArea).Within(0.002f));
            Assert.That(_asteroid.Definition.CreateState().TotalArea(),Is.EqualTo(before).Within(0.0001f));
            CheckGeometry();
            int activeRevision=_asteroid.State.Revision;
            _drill.RequestStop(); yield return Steps(10);
            Assert.That(_asteroid.State.Revision,Is.GreaterThan(activeRevision),"C brakes movement without cancelling held LMB.");
            _drill.SetBeamPreview(false);
            int revision=_asteroid.State.Revision; float area=_asteroid.State.TotalArea();
            yield return Steps(20);
            Assert.That(_asteroid.State.Revision,Is.EqualTo(revision));
            Assert.That(_asteroid.State.TotalArea(),Is.EqualTo(area));
            Assert.That(_ship.GetComponent<ShipFlightController>().MainEngineActive,Is.False);
        }

        [UnityTest]
        public IEnumerator ShipMinesItsOwnTunnelTurnsInsideItAndReturnsToOpenSpace()
        {
            yield return Ready(); _drill.SetBeamPreview(true); _drill.SetInput(1f,0f);
            for(int i=0;i<850 && _body.position.y<1997f;i++)
            {
                yield return Steps(1);
                Assert.That(_hull.Distance(_solid).distance,Is.GreaterThan(-0.06f));
            }
            Assert.That(_body.position.y,Is.GreaterThan(1996.8f),"Ship must advance into newly cut rock. Removed="+_miner.RemovedArea+" Revision="+_asteroid.State.Revision+" Beam="+_drill.BeamPreviewActive);
            Assert.That(_miner.RemovedArea,Is.GreaterThan(15f));
            _drill.SetBeamPreview(false); _drill.RequestStop(); yield return Steps(80); CheckGeometry();
            Vector2 pivot=_body.worldCenterOfMass;
            float previous=_body.rotation,turned=0f;
            _drill.SetInput(0f,1f);
            for(int i=0;i<450 && turned<345f;i++)
            {
                yield return Steps(1);
                turned+=Mathf.DeltaAngle(previous,_body.rotation); previous=_body.rotation;
                Assert.That(_hull.Distance(_solid).distance,Is.GreaterThan(0.02f),"Turn clearance in self-mined tunnel.");
            }
            _drill.RequestStop(); yield return Steps(80);
            Assert.That(turned,Is.GreaterThan(344f));
            Assert.That(Vector2.Distance(pivot,_body.worldCenterOfMass),Is.LessThan(0.06f));
            Assert.That(Mathf.Abs(Mathf.DeltaAngle(0f,_body.rotation)),Is.LessThan(6f));
            _drill.SetInput(-1f,0f);
            for(int i=0;i<650 && _body.position.y>1988.7f;i++) yield return Steps(1);
            Assert.That(_body.position.y,Is.LessThan(1989f));
            _drill.RequestStop(); yield return Steps(80);
            Assert.That(_body.linearVelocity.magnitude,Is.LessThan(0.03f));
            Assert.That(_rock.GetComponentsInChildren<Rigidbody2D>().Length,Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator ShipCutsARightAngleBranchAcrossChunkBoundaries()
        {
            yield return Ready(); _drill.SetBeamPreview(true); _drill.SetInput(1f,0f);
            for(int i=0;i<800 && _body.position.y<1996f;i++) yield return Steps(1);
            Assert.That(_body.position.y,Is.GreaterThan(1995.8f));
            _drill.SetBeamPreview(false); _drill.RequestStop(); yield return Steps(80);
            float previous=_body.rotation, turned=0f;
            _drill.SetInput(0f,-1f);
            for(int i=0;i<150 && turned>-75f;i++)
            {
                yield return Steps(1);
                turned+=Mathf.DeltaAngle(previous,_body.rotation); previous=_body.rotation;
                Assert.That(_hull.Distance(_solid).distance,Is.GreaterThan(0.02f));
            }
            _drill.RequestStop(); yield return Steps(80);
            Assert.That(Mathf.Abs(Mathf.DeltaAngle(-90f,_body.rotation)),Is.LessThan(6f));
            Vector2 corner=_body.position;
            _drill.SetBeamPreview(true); _drill.SetInput(1f,0f);
            for(int i=0;i<600 && _body.position.x<corner.x+4f;i++)
            {
                yield return Steps(1);
                Assert.That(_hull.Distance(_solid).distance,Is.GreaterThan(-0.06f));
            }
            Assert.That(_body.position.x,Is.GreaterThan(corner.x+3.8f));
            _drill.SetBeamPreview(false); _drill.RequestStop(); yield return Steps(80);
            CheckGeometry();
            Assert.That(_asteroid.State.Sample(new Vector2(2f,corner.y-2000f)),Is.LessThan(0f));
        }

        [UnityTest]
        public IEnumerator DiagonalMiningUpdatesSharedCollisionAndReturnToFlightStopsExcavation()
        {
            _body.position=new Vector2(1994f,1988f); _body.rotation=-35f; Physics2D.SyncTransforms();
            Vector2 start=_body.position;
            yield return Ready(); _drill.SetBeamPreview(true); _drill.SetInput(1f,0f);
            for(int i=0;i<600 && Vector2.Distance(start,_body.position)<6f;i++)
            {
                yield return Steps(1);
                Assert.That(_hull.Distance(_solid).distance,Is.GreaterThan(-0.06f));
            }
            Assert.That(Vector2.Distance(start,_body.position),Is.GreaterThan(5.8f));
            Assert.That(_miner.RemovedArea,Is.GreaterThan(8f));
            _drill.SetBeamPreview(false); _drill.RequestStop(); yield return Steps(30); CheckGeometry();
            _drill.ToggleMode(); int revision=_asteroid.State.Revision;
            _drill.SetBeamPreview(true); yield return Steps(20);
            Assert.That(_asteroid.State.Revision,Is.EqualTo(revision));
            Assert.That(_drill.Mode,Is.EqualTo(ShipControlMode.Flight));
        }
    }
}
#endif
