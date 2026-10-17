using System.Collections;
using AstroMiner.Flight;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace AstroMiner.PlayModeTests
{
    public sealed class ShipFlightTests
    {
        private GameObject _ship;
        private Rigidbody2D _body;
        private ShipFlightController _flight;
        private float _previousFixedStep;

        [SetUp]
        public void SetUp()
        {
            _previousFixedStep = Time.fixedDeltaTime;
            Time.fixedDeltaTime = 0.02f;
            _ship = new GameObject("Flight rule test");
            _body = _ship.AddComponent<Rigidbody2D>();
            _body.gravityScale = 0f;
            _body.linearDamping = 0f;
            _body.angularDamping = 0f;
            _flight = _ship.AddComponent<ShipFlightController>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_ship);
            Time.fixedDeltaTime = _previousFixedStep;
        }

        private static IEnumerator Steps(int count)
        {
            for (int i = 0; i < count; i++) yield return new WaitForFixedUpdate();
        }

        [UnityTest]
        public IEnumerator ReleasedInputPreservesTheVelocityVector()
        {
            _flight.SetMoveDirection(Vector2.up);
            yield return Steps(40);
            _flight.SetMoveDirection(Vector2.zero);
            yield return Steps(2);
            Vector2 drifting = _body.linearVelocity;
            Assert.That(drifting.y, Is.GreaterThan(2f));
            yield return Steps(50);
            Assert.That(Vector2.Distance(_body.linearVelocity, drifting), Is.LessThan(0.0001f));
        }

        [UnityTest]
        public IEnumerator TurningWaitsForAlignmentAndDoesNotRedirectExistingVelocity()
        {
            _body.linearVelocity = new Vector2(3f, 0f);
            _flight.SetMoveDirection(Vector2.right);
            yield return Steps(10);
            Assert.That(_body.rotation, Is.LessThan(-2f), "RCS must actually turn the body.");
            Assert.That(_flight.MainEngineActive, Is.False);
            Assert.That(Vector2.Distance(_body.linearVelocity, new Vector2(3f, 0f)), Is.LessThan(0.0001f));
            yield return Steps(100);
            Assert.That(Mathf.Abs(Mathf.DeltaAngle(_body.rotation, -90f)), Is.LessThan(5f));
            Assert.That(_flight.MainEngineActive, Is.True);
            Assert.That(_body.linearVelocity.x, Is.GreaterThan(3.1f));
        }

        [UnityTest]
        public IEnumerator AlignedThrustChangesVelocityGradually()
        {
            _flight.SetMoveDirection(Vector2.up);
            yield return Steps(1);
            Assert.That(_body.linearVelocity.y, Is.InRange(0.001f, 0.09f));
            float firstSpeed = _body.linearVelocity.y;
            yield return Steps(30);
            Assert.That(_body.linearVelocity.y, Is.GreaterThan(firstSpeed + 2f));
        }

        [UnityTest]
        public IEnumerator FullStopRotatesThenBrakesAndRemainsStopped()
        {
            _body.linearVelocity = new Vector2(5f, 3f);
            _flight.RequestFullStop();
            yield return Steps(1);
            Assert.That(_flight.IsStopping, Is.True);
            Assert.That(_body.linearVelocity.magnitude, Is.GreaterThan(5f), "No instant Full Stop.");
            for (int i = 0; i < 600 && _flight.IsStopping; i++) yield return Steps(1);
            Assert.That(_flight.IsStopping, Is.False, "Stop must settle without permanent oscillation.");
            Assert.That(_body.linearVelocity.magnitude, Is.LessThan(0.0001f));
            yield return Steps(50);
            Assert.That(_body.linearVelocity.magnitude, Is.LessThan(0.0001f));
        }

        [UnityTest]
        public IEnumerator NewDirectionCancelsFullStop()
        {
            _body.linearVelocity = Vector2.up * 3f;
            _flight.RequestFullStop();
            yield return Steps(5);
            _flight.SetMoveDirection(Vector2.up);
            Assert.That(_flight.IsStopping, Is.False);
            yield return Steps(100);
            Assert.That(_body.linearVelocity.y, Is.GreaterThan(3f));
        }

        [UnityTest]
        public IEnumerator StopAtRestOrResidualSpeedDoesNotRotate()
        {
            _body.rotation = 37f;
            _flight.RequestFullStop();
            yield return Steps(10);
            Assert.That(_flight.IsStopping, Is.False);
            Assert.That(_body.rotation, Is.EqualTo(37f).Within(0.001f));
            _body.linearVelocity = new Vector2(0.01f, 0f);
            _flight.RequestFullStop();
            yield return Steps(10);
            Assert.That(_flight.IsStopping, Is.False);
            Assert.That(_body.linearVelocity.magnitude, Is.LessThan(0.0001f));
            Assert.That(_body.rotation, Is.EqualTo(37f).Within(0.001f));
        }
    }
}
