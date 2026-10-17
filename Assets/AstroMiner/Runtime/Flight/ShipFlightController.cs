using UnityEngine;

namespace AstroMiner.Flight
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class ShipFlightController : MonoBehaviour
    {
        [Header("Prototype tuning - not final balance")]
        [SerializeField, Min(0.01f)] private float _mainAcceleration = 4f;
        [SerializeField, Min(1f)] private float _maxTurnSpeed = 160f;
        [SerializeField, Min(1f)] private float _turnAcceleration = 600f;
        [SerializeField, Range(0.1f, 15f)] private float _alignmentTolerance = 5f;
        [Tooltip("Only residual speeds below this AND one thrust step are settled without rotating.")]
        [SerializeField, Min(0f)] private float _stopEpsilon = 0.02f;

        private Rigidbody2D _body;
        private Vector2 _moveDirection;
        public bool IsStopping { get; private set; }
        public bool MainEngineActive { get; private set; }
        public float TurnEffort { get; private set; }

        private void Awake() => _body = GetComponent<Rigidbody2D>();

        public void SetMoveDirection(Vector2 worldDirection)
        {
            _moveDirection = Vector2.ClampMagnitude(worldDirection, 1f);
            if (_moveDirection.sqrMagnitude > 0f) IsStopping = false;
        }

        public void RequestFullStop()
        {
            _moveDirection = Vector2.zero;
            IsStopping = true;
        }

        private void OnDisable()
        {
            _moveDirection = Vector2.zero;
            IsStopping = false;
            MainEngineActive = false;
            TurnEffort = 0f;
            // Removing control never removes existing physical momentum.
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            MainEngineActive = false;
            Vector2 velocity = _body.linearVelocity;
            float speed = velocity.magnitude;
            Vector2 target = IsStopping ? -velocity.normalized : _moveDirection.normalized;

            if (IsStopping && speed <= Mathf.Min(_stopEpsilon, _mainAcceleration * dt))
            {
                // Bounded final correction: at most one normal thrust step, never a velocity snap.
                _body.AddForce(-velocity * (_body.mass / dt));
                IsStopping = false;
                target = Vector2.zero;
            }

            float error = target == Vector2.zero ? 0f : Mathf.DeltaAngle(
                _body.rotation, Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg - 90f);
            float desiredRate = Mathf.Clamp(error * 4f, -_maxTurnSpeed, _maxTurnSpeed);
            float angularAcceleration = Mathf.Clamp((desiredRate - _body.angularVelocity) * 8f,
                -_turnAcceleration, _turnAcceleration);
            _body.AddTorque(angularAcceleration * Mathf.Deg2Rad * _body.inertia);
            TurnEffort = angularAcceleration / _turnAcceleration;

            if (target == Vector2.zero || Mathf.Abs(error) > _alignmentTolerance) return;
            Vector2 nose = Quaternion.Euler(0f, 0f, _body.rotation) * Vector2.up;
            float acceleration = _mainAcceleration;
            if (IsStopping)
            {
                // Do not reverse the component along the nozzle in the last braking step.
                float approachSpeed = Mathf.Max(0f, -Vector2.Dot(velocity, nose));
                acceleration = Mathf.Min(acceleration, approachSpeed / dt);
            }
            _body.AddForce(nose * (acceleration * _body.mass));
            MainEngineActive = acceleration > 0.001f;
        }
    }
}
