#if UNITY_EDITOR
using System.Collections;
using AstroMiner.Flight;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;

namespace AstroMiner.PlayModeTests
{
    // Load the actual authored prefabs. These integration tests run in Editor PlayMode.
    public sealed class StaticAsteroidTests
    {
        private GameObject _asteroid;
        private GameObject _ship;
        private Tilemap _map;
        private CompositeCollider2D _rockCollider;
        private Rigidbody2D _body;
        private CapsuleCollider2D _shipCollider;
        private ShipFlightController _flight;
        private float _previousFixedStep;

        [SetUp]
        public void SetUp()
        {
            _previousFixedStep = Time.fixedDeltaTime;
            Time.fixedDeltaTime = 0.02f;
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AstroMiner/Prefabs/StaticAsteroid.prefab");
            Assert.That(prefab, Is.Not.Null);
            _asteroid = Object.Instantiate(prefab, new Vector3(1000f, 1000f), Quaternion.identity);
            _map = _asteroid.GetComponentInChildren<Tilemap>();
            _rockCollider = _asteroid.GetComponentInChildren<CompositeCollider2D>();
            _map.GetComponent<TilemapCollider2D>().ProcessTilemapChanges();
            _rockCollider.GenerateGeometry();
            Physics2D.SyncTransforms();
        }

        [TearDown]
        public void TearDown()
        {
            if (_ship != null) Object.DestroyImmediate(_ship);
            if (_asteroid != null) Object.DestroyImmediate(_asteroid);
            Time.fixedDeltaTime = _previousFixedStep;
        }

        private void SpawnShip(Vector2 direction, float clearance, float speed)
        {
            Vector2 origin = _asteroid.transform.position;
            RaycastHit2D hit = Physics2D.Raycast(origin - direction * 30f, direction, 40f);
            Assert.That(hit.collider, Is.EqualTo(_rockCollider));
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AstroMiner/Prefabs/ShipPlaceholder.prefab");
            Assert.That(prefab, Is.Not.Null);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            _ship = Object.Instantiate(prefab, hit.point - direction * clearance, Quaternion.Euler(0f, 0f, angle));
            _ship.GetComponent<ShipKeyboardInput>().enabled = false;
            _body = _ship.GetComponent<Rigidbody2D>();
            _shipCollider = _ship.GetComponent<CapsuleCollider2D>();
            _flight = _ship.GetComponent<ShipFlightController>();
            _body.linearVelocity = direction * speed;
            Physics2D.SyncTransforms();
        }

        private static IEnumerator Steps(int count)
        {
            for (int i = 0; i < count; i++) yield return new WaitForFixedUpdate();
        }

        [UnityTest]
        public IEnumerator SolidCellsAndEmptySpaceMatchTheSavedComposite()
        {
            yield return Steps(1);
            int solid = 0, empty = 0;
            foreach (Vector3Int cell in _map.cellBounds.allPositionsWithin)
            {
                bool expected = _map.HasTile(cell);
                Assert.That(_rockCollider.OverlapPoint(_map.GetCellCenterWorld(cell)), Is.EqualTo(expected), cell.ToString());
                if (expected) solid++; else empty++;
            }
            Assert.That(solid, Is.GreaterThan(1000));
            Assert.That(empty, Is.GreaterThan(100));
            Assert.That(_rockCollider.attachedRigidbody.bodyType, Is.EqualTo(RigidbodyType2D.Static));
            var sprite = _map.GetSprite(_map.WorldToCell(_asteroid.transform.position));
            Assert.That(sprite.bounds.size.x, Is.EqualTo(_map.layoutGrid.cellSize.x).Within(0.0001f));
        }

        [UnityTest]
        public IEnumerator LowSpeedContactAllowsMovementAlongTheSurfaceAndDeparture()
        {
            SpawnShip(Vector2.up, 2.4f, 2f);
            bool touched = false;
            Vector3 rockOrigin = _asteroid.transform.position;
            for (int i = 0; i < 80; i++)
            {
                yield return Steps(1);
                touched |= _shipCollider.IsTouching(_rockCollider);
                Assert.That(_shipCollider.Distance(_rockCollider).distance, Is.GreaterThan(-0.05f));
            }
            Assert.That(touched, Is.True, "The production ship must make real contact.");
            Vector2 contactPosition = _body.position;
            _flight.SetMoveDirection(Vector2.right);
            yield return Steps(120);
            Assert.That(_body.position.x - contactPosition.x, Is.GreaterThan(0.5f), "Contact must not glue the ship to the rock.");
            Assert.That(_shipCollider.Distance(_rockCollider).distance, Is.GreaterThan(-0.05f));
            Vector2 beforeDeparture = _body.position;
            _flight.SetMoveDirection(Vector2.down);
            yield return Steps(150);
            Assert.That(_body.position.y, Is.LessThan(beforeDeparture.y - 0.5f));
            Assert.That(_body.linearVelocity.y, Is.LessThan(-0.5f));
            Assert.That(_asteroid.transform.position, Is.EqualTo(rockOrigin));
        }

        [UnityTest]
        public IEnumerator FastDiagonalImpactDoesNotTunnelIntoSolidRock()
        {
            SpawnShip(new Vector2(1f, 1f).normalized, 15f, 80f);
            bool touched = false;
            for (int i = 0; i < 30; i++)
            {
                yield return Steps(1);
                touched |= _shipCollider.IsTouching(_rockCollider);
                Assert.That(_rockCollider.OverlapPoint(_body.position), Is.False, "Ship center entered solid rock.");
                Assert.That(_shipCollider.Distance(_rockCollider).distance, Is.GreaterThan(-0.05f));
            }
            Assert.That(touched, Is.True, "Continuous collision must catch the approach.");
        }

        [UnityTest]
        public IEnumerator FullStopCanFinishBeforeTheSurface()
        {
            SpawnShip(Vector2.up, 14f, 3f);
            _flight.RequestFullStop();
            for (int i = 0; i < 220; i++)
            {
                yield return Steps(1);
                Assert.That(_shipCollider.IsTouching(_rockCollider), Is.False);
            }
            Assert.That(_flight.IsStopping, Is.False);
            Assert.That(_body.linearVelocity.magnitude, Is.LessThan(0.0001f));
        }
    }
}
#endif
