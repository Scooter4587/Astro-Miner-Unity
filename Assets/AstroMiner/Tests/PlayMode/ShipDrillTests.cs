#if UNITY_EDITOR
using System.Collections;
using AstroMiner.Flight;
using AstroMiner.Mining;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;

namespace AstroMiner.PlayModeTests
{
    public sealed class ShipDrillTests
    {
        private GameObject _ship, _rock;
        private ShipDrillController _drill;
        private ShipFlightController _flight;
        private ShipKeyboardInput _input;
        private Rigidbody2D _body;
        private CapsuleCollider2D _hull;
        private CompositeCollider2D _rockCollider;
        private Keyboard _keyboard;
        private Mouse _mouse;
        private InputTestFixture _inputFixture;
        private float _oldStep;

        [SetUp]
        public void SetUp()
        {
            _oldStep = Time.fixedDeltaTime;
            Time.fixedDeltaTime = 0.02f;
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AstroMiner/Prefabs/ShipDrillPrototype.prefab");
            Assert.That(prefab, Is.Not.Null);
            _ship = Object.Instantiate(prefab, new Vector3(1000f,1000f), Quaternion.identity);
            _input = _ship.GetComponent<ShipKeyboardInput>();
            _input.enabled = false;
            _drill = _ship.GetComponent<ShipDrillController>();
            _flight = _ship.GetComponent<ShipFlightController>();
            _body = _ship.GetComponent<Rigidbody2D>();
            _hull = _ship.GetComponent<CapsuleCollider2D>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_ship != null) Object.DestroyImmediate(_ship);
            if (_rock != null) Object.DestroyImmediate(_rock);
            _inputFixture?.TearDown();
            _inputFixture = null;
            Time.fixedDeltaTime = _oldStep;
        }

        private static IEnumerator Steps(int count)
        {
            for (int i = 0; i < count; i++) yield return new WaitForFixedUpdate();
        }

        private IEnumerator EnterDrill()
        {
            _drill.ToggleMode();
            yield return Steps(15);
            Assert.That(_drill.Mode, Is.EqualTo(ShipControlMode.Drill));
            Assert.That(_flight.ControlSuspended, Is.True);
        }

        private void SpawnRock()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AstroMiner/Prefabs/DrillTestAsteroid.prefab");
            Assert.That(prefab, Is.Not.Null);
            _rock = Object.Instantiate(prefab, new Vector3(1000f,1000f), Quaternion.identity);
            _rockCollider = _rock.GetComponentInChildren<CompositeCollider2D>();
            _rockCollider.GetComponent<TilemapCollider2D>().ProcessTilemapChanges();
            _rockCollider.GenerateGeometry();
            Physics2D.SyncTransforms();
        }

        [UnityTest]
        public IEnumerator HighSpeedEntryBrakesWithBoundedRcsBeforeBecomingReady()
        {
            _body.linearVelocity = new Vector2(3f,-4f);
            _body.angularVelocity = 90f;
            _drill.ToggleMode();
            Assert.That(_drill.Mode, Is.EqualTo(ShipControlMode.Stabilizing));
            Assert.That(_body.linearVelocity.magnitude, Is.EqualTo(5f).Within(0.0001f));
            Assert.That(_body.angularVelocity, Is.EqualTo(90f).Within(0.0001f));
            _drill.SetBeamPreview(true);
            Assert.That(_drill.BeamPreviewActive, Is.False);
            yield return Steps(1);
            Assert.That(_body.linearVelocity.magnitude, Is.GreaterThan(4.9f));
            Assert.That(_body.angularVelocity, Is.GreaterThan(85f));
            for (int i=0; i<250 && _drill.Mode != ShipControlMode.Drill; i++)
            {
                yield return Steps(1);
                Assert.That(_flight.MainEngineActive, Is.False);
                Assert.That(_drill.RcsAcceleration.magnitude, Is.LessThanOrEqualTo(_drill.Profile.Acceleration+0.001f));
            }
            Assert.That(_drill.Mode, Is.EqualTo(ShipControlMode.Drill));
            Assert.That(_body.linearVelocity.magnitude, Is.LessThanOrEqualTo(_drill.Profile.ReadySpeed));
            Assert.That(Mathf.Abs(_body.angularVelocity), Is.LessThanOrEqualTo(_drill.Profile.ReadyTurnSpeed));
        }

        [UnityTest]
        public IEnumerator DrillMovesAndReversesAtLowSpeedAndCBrakesWithoutRetargetingTheNose()
        {
            _body.rotation = 37f;
            yield return EnterDrill();
            Vector2 nose = Quaternion.Euler(0f,0f,37f) * Vector2.up;
            _drill.SetInput(1f,0f);
            yield return Steps(120);
            Assert.That(Vector2.Dot(_body.linearVelocity,nose), Is.InRange(1.4f,1.51f));
            Assert.That(_flight.MainEngineActive, Is.False);
            _drill.SetInput(0f,0f);
            yield return Steps(100);
            Assert.That(_body.linearVelocity.magnitude, Is.LessThan(0.0001f));
            _drill.SetInput(-1f,0f);
            yield return Steps(70);
            Assert.That(Vector2.Dot(_body.linearVelocity,nose), Is.LessThan(-1.3f));
            float speed = _body.linearVelocity.magnitude;
            _drill.RequestStop();
            Assert.That(_body.linearVelocity.magnitude, Is.EqualTo(speed).Within(0.0001f));
            yield return Steps(100);
            Assert.That(_body.linearVelocity.magnitude, Is.LessThan(0.0001f));
            Assert.That(Mathf.Abs(Mathf.DeltaAngle(_body.rotation,37f)), Is.LessThan(0.05f));
        }

        [UnityTest]
        public IEnumerator PreparedCorridorAllowsAFullTurnWithoutContactOrTranslation()
        {
            SpawnRock();
            yield return EnterDrill();
            Vector2 pivot = _body.worldCenterOfMass;
            Assert.That(_drill.GetTurnRadius(), Is.EqualTo(1.4f).Within(0.01f));
            _drill.SetInput(0f,1f);
            float total = 0f, previous = _body.rotation;
            for (int i=0; i<340; i++)
            {
                yield return Steps(1);
                total += Mathf.DeltaAngle(previous,_body.rotation);
                previous = _body.rotation;
                Assert.That(Vector2.Distance(pivot,_body.worldCenterOfMass), Is.LessThan(0.03f));
                Assert.That(_hull.Distance(_rockCollider).distance, Is.GreaterThan(0.1f));
                Assert.That(Mathf.Abs(_body.angularVelocity), Is.LessThanOrEqualTo(60.1f));
            }
            Assert.That(total, Is.GreaterThan(330f));
            _drill.SetInput(0f,0f);
            yield return Steps(100);
            Assert.That(Mathf.Abs(_body.angularVelocity), Is.LessThan(0.001f));
        }

        [UnityTest]
        public IEnumerator ContactAtBlindEndSettlesAndAllowsReverseDeparture()
        {
            SpawnRock();
            Vector2 chamber = (Vector2)_rock.transform.position + new Vector2(6f,3f);
            var hit = Physics2D.Raycast(chamber,Vector2.right,6f);
            Assert.That(hit.collider, Is.EqualTo(_rockCollider));
            _body.position = hit.point - Vector2.right*1.72f;
            _body.rotation = -90f;
            _body.linearVelocity = Vector2.right*2f;
            Physics2D.SyncTransforms();
            _drill.ToggleMode();
            bool touched = false;
            for(int i=0;i<140;i++)
            {
                yield return Steps(1);
                touched |= _hull.IsTouching(_rockCollider);
                Assert.That(_hull.Distance(_rockCollider).distance, Is.GreaterThan(-0.05f));
            }
            Assert.That(touched, Is.True,"The scenario must include real contact, not just proximity.");
            Assert.That(_drill.Mode, Is.EqualTo(ShipControlMode.Drill));
            _drill.SetInput(1f,0f);
            yield return Steps(150);
            Assert.That(_body.linearVelocity.magnitude, Is.LessThan(0.1f));
            Assert.That(Mathf.Abs(_body.angularVelocity), Is.LessThan(1f));
            Assert.That(_hull.Distance(_rockCollider).distance, Is.GreaterThan(-0.05f));
            Vector2 before = _body.position;
            _drill.SetInput(-1f,0f);
            yield return Steps(90);
            Assert.That(_body.position.x, Is.LessThan(before.x-1f));
            Assert.That(_hull.Distance(_rockCollider).distance, Is.GreaterThan(0.2f));
        }

        [UnityTest]
        public IEnumerator LeavingDrillPreservesMomentumAndReturnsControlToFlight()
        {
            _body.linearVelocity = new Vector2(3f,1f);
            _drill.ToggleMode();
            yield return Steps(5);
            Vector2 before = _body.linearVelocity;
            _drill.ToggleMode();
            Assert.That(_drill.Mode, Is.EqualTo(ShipControlMode.Flight));
            Assert.That(_flight.ControlSuspended, Is.False);
            Assert.That(Vector2.Distance(_body.linearVelocity,before), Is.LessThan(0.0001f));
            yield return Steps(25);
            Assert.That(Vector2.Distance(_body.linearVelocity,before), Is.LessThan(0.0001f));
            _flight.SetMoveDirection(Vector2.up);
            yield return Steps(30);
            Assert.That(_body.linearVelocity.y, Is.GreaterThan(before.y+1f));
        }

        private static int CountCells(Tilemap map)
        {
            int count = 0;
            foreach (Vector3Int cell in map.cellBounds.allPositionsWithin)
                if (map.HasTile(cell)) count++;
            return count;
        }

        private void UseSyntheticInput()
        {
            // Isolate device state and focus from the actual Editor, then restore it in TearDown.
            _inputFixture = new InputTestFixture();
            _inputFixture.Setup();
            _keyboard = InputSystem.AddDevice<Keyboard>();
            _mouse = InputSystem.AddDevice<Mouse>();
            _input.enabled = true;
            _ship.SendMessage("OnApplicationFocus",true);
        }

        private IEnumerator Keys(params Key[] keys)
        {
            InputSystem.QueueStateEvent(_keyboard,new KeyboardState(keys));
            InputSystem.Update();
            yield return null;
            yield return null;
        }

        [UnityTest]
        public IEnumerator RealInputRequiresNeutralAfterRAndHeldWDoesNotCancelC()
        {
            UseSyntheticInput();
            yield return Keys();
            yield return Keys(Key.W);
            yield return Steps(30);
            Assert.That(_body.linearVelocity.y, Is.GreaterThan(2f));
            yield return Keys(Key.W,Key.R);
            Assert.That(_drill.IsEngaged, Is.True);
            yield return Keys(Key.W);
            yield return Steps(180);
            Assert.That(_drill.Mode, Is.EqualTo(ShipControlMode.Drill));
            Assert.That(_body.linearVelocity.magnitude, Is.LessThan(0.0001f),
                "Held W must not become drill thrust during the transition.");
            yield return Keys();
            yield return Keys(Key.W);
            yield return Steps(30);
            Assert.That(_body.linearVelocity.y, Is.GreaterThan(0.5f));
            float heading = _body.rotation;
            yield return Keys(Key.W,Key.C);
            yield return Keys(Key.W);
            yield return Steps(100);
            Assert.That(_body.linearVelocity.magnitude, Is.LessThan(0.0001f));
            Assert.That(Mathf.Abs(Mathf.DeltaAngle(heading,_body.rotation)), Is.LessThan(0.05f));
            yield return Keys();
            yield return Keys(Key.R);
            Assert.That(_drill.Mode, Is.EqualTo(ShipControlMode.Flight));
            Assert.That(_flight.ControlSuspended, Is.False);
        }

        [UnityTest]
        public IEnumerator FocusLossStopsPreviewAndHeldInputCannotRestartItOnReturn()
        {
            SpawnRock();
            UseSyntheticInput();
            yield return Keys();
            yield return Keys(Key.R);
            yield return Keys();
            yield return Steps(20);
            var map = _rockCollider.GetComponent<Tilemap>();
            int before = CountCells(map);
            InputSystem.QueueStateEvent(_mouse,new MouseState { buttons = 1 });
            InputSystem.Update();
            yield return Keys(Key.W);
            yield return Steps(30);
            Assert.That(_drill.BeamPreviewActive, Is.True);
            _ship.SendMessage("OnApplicationFocus",false);
            Assert.That(_drill.BeamPreviewActive, Is.False);
            yield return Steps(100);
            _ship.SendMessage("OnApplicationFocus",true);
            yield return Steps(20);
            Assert.That(_body.linearVelocity.magnitude, Is.LessThan(0.0001f));
            Assert.That(_drill.BeamPreviewActive, Is.False);
            Assert.That(CountCells(map), Is.EqualTo(before),"A preview must not excavate.");
        }
    }
}
#endif
