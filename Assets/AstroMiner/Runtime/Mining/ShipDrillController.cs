using AstroMiner.Flight;
using UnityEngine;

namespace AstroMiner.Mining
{
    public enum ShipControlMode { Flight, Stabilizing, Drill }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D), typeof(ShipFlightController))]
    public sealed class ShipDrillController : MonoBehaviour
    {
        [SerializeField] private DrillModeProfile _profile;
        [SerializeField] private Transform _emitter;
        private Rigidbody2D _body;
        private CapsuleCollider2D _hull;
        private ShipFlightController _flight;
        private ShipLaserMiner _miner;
        private readonly ContactPoint2D[] _contacts = new ContactPoint2D[16];
        private readonly RaycastHit2D[] _castHits = new RaycastHit2D[16];
        private ContactFilter2D _solidFilter;
        private float _forward;
        private float _turn;
        private bool _previewHeld;

        public DrillModeProfile Profile => _profile;
        public ShipControlMode Mode { get; private set; }
        public bool IsEngaged => Mode != ShipControlMode.Flight;
        public bool IsStopping { get; private set; }
        public Vector2 RcsAcceleration { get; private set; }
        public float TurnEffort { get; private set; }
        public int ContactCount { get; private set; }
        public bool BeamPreviewActive => Mode == ShipControlMode.Drill && _previewHeld;
        // Presentation follows the interpolated transform; physics uses worldCenterOfMass directly.
        public Vector2 Center => _body != null ?
            (Vector2)_body.transform.TransformPoint(_body.centerOfMass) : (Vector2)transform.position;

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _hull = GetComponent<CapsuleCollider2D>();
            _flight = GetComponent<ShipFlightController>();
            _miner = GetComponent<ShipLaserMiner>();
            _solidFilter = new ContactFilter2D { useTriggers = false };
            if (_profile != null && _emitter != null) return;
            Debug.LogError("Drill controller needs its profile and emitter.", this);
            enabled = false;
        }

        public void ToggleMode()
        {
            if (!enabled || _profile == null) return;
            ClearInput();
            IsStopping = false;
            Mode = IsEngaged ? ShipControlMode.Flight : ShipControlMode.Stabilizing;
            _flight.SetControlSuspended(IsEngaged);
        }

        // Called only when axes change; a held command cannot immediately cancel C.
        public void SetInput(float forward, float turn)
        {
            if (Mode != ShipControlMode.Drill) return;
            _forward = Mathf.Clamp(forward, -1f, 1f);
            _turn = Mathf.Clamp(turn, -1f, 1f);
            if (_forward != 0f || _turn != 0f) IsStopping = false;
        }

        public void SetBeamPreview(bool held) => _previewHeld = IsEngaged && held;

        public void RequestStop()
        {
            if (!IsEngaged) return;
            _forward = _turn = 0f;
            IsStopping = true;
        }

        public void ClearInput()
        {
            _forward = _turn = 0f;
            _previewHeld = false;
        }

        private void OnDisable()
        {
            ClearInput();
            Mode = ShipControlMode.Flight;
            IsStopping = false;
            RcsAcceleration = Vector2.zero;
            TurnEffort = 0f;
            ContactCount = 0;
            if (_flight != null) _flight.SetControlSuspended(false);
        }

        private void FixedUpdate()
        {
            RcsAcceleration = Vector2.zero;
            TurnEffort = 0f;
            ContactCount = 0;
            if (!IsEngaged) return;
            float dt = Time.fixedDeltaTime;
            Vector2 velocity = _body.linearVelocity;
            if (Mode == ShipControlMode.Stabilizing &&
                velocity.magnitude <= _profile.ReadySpeed &&
                Mathf.Abs(_body.angularVelocity) <= _profile.ReadyTurnSpeed)
                Mode = ShipControlMode.Drill;

            bool canMove = Mode == ShipControlMode.Drill && !IsStopping;
            Vector2 nose = Quaternion.Euler(0f, 0f, _body.rotation) * Vector2.up;
            Vector2 desiredVelocity = canMove ? nose * (_forward * _profile.MoveSpeed) : Vector2.zero;
            float desiredTurnRate = canMove ? _turn * _profile.TurnSpeed : 0f;

            // While cutting, clear the complete turning corridor before its near plane passes the shoulders.
            if (_miner != null && _miner.isActiveAndEnabled && _forward > 0f && BeamPreviewActive)
            {
                float speed = desiredVelocity.magnitude;
                float safe = _miner.GetSafeForwardSpeed(speed, _profile.Acceleration,
                    _profile.ContactSkin, 2f * (GetTurnRadius() + _profile.TurnClearance));
                if (speed > 0f) desiredVelocity *= safe / speed;
            }

            // Full-hull cast limits the commanded approach speed, not the actual momentum.
            if (desiredVelocity.sqrMagnitude > 0f)
            {
                float speed = desiredVelocity.magnitude;
                float stopDistance = speed * speed / (2f * _profile.Acceleration);
                int hits = _hull.Cast(desiredVelocity / speed, _solidFilter, _castHits,
                    stopDistance + _profile.ContactSkin);
                float safeDistance = float.PositiveInfinity;
                for (int i = 0; i < hits; i++)
                    safeDistance = Mathf.Min(safeDistance,
                        Mathf.Max(0f, _castHits[i].distance - _profile.ContactSkin));
                if (!float.IsPositiveInfinity(safeDistance))
                    desiredVelocity *= Mathf.Min(1f, Mathf.Sqrt(
                        2f * _profile.Acceleration * safeDistance) / speed);
            }

            ContactCount = _hull.GetContacts(_solidFilter, _contacts);
            Vector2 acceleration = Vector2.ClampMagnitude(
                (desiredVelocity - velocity) / _profile.ResponseTime, _profile.Acceleration);
            if (desiredVelocity == Vector2.zero && velocity.magnitude <= Mathf.Min(
                0.02f, _profile.Acceleration * dt))
                acceleration = -velocity / dt;

            // Do not keep driving into a wall. Retreat and tangential motion remain possible.
            for (int i = 0; i < ContactCount; i++)
            {
                Vector2 normal = _contacts[i].normal;
                float intoWall = Vector2.Dot(acceleration, normal);
                if (intoWall < 0f) acceleration -= normal * intoWall;
                Vector2 arm = _contacts[i].point - _body.worldCenterOfMass;
                Vector2 turningMotion = new Vector2(-arm.y, arm.x) * desiredTurnRate;
                if (Vector2.Dot(turningMotion, normal) < -0.01f) desiredTurnRate = 0f;
            }

            float turnAcceleration = Mathf.Clamp(
                (desiredTurnRate - _body.angularVelocity) / _profile.ResponseTime,
                -_profile.TurnAcceleration, _profile.TurnAcceleration);
            if (desiredTurnRate == 0f && Mathf.Abs(_body.angularVelocity) <= Mathf.Min(
                0.1f, _profile.TurnAcceleration * dt))
                turnAcceleration = -_body.angularVelocity / dt;

            _body.AddForce(acceleration * _body.mass);
            _body.AddTorque(turnAcceleration * Mathf.Deg2Rad * _body.inertia);
            RcsAcceleration = acceleration;
            TurnEffort = turnAcceleration / _profile.TurnAcceleration;
            if (IsStopping && velocity.magnitude <= 0.02f &&
                Mathf.Abs(_body.angularVelocity) <= 0.1f) IsStopping = false;
        }

        public MiningBeam GetMiningBeam() => new MiningBeam(_emitter.position, _emitter.up,
            _profile.BeamRange, _profile.BeamNearWidth, _profile.BeamHalfAngle);

        public ContactPoint2D GetContact(int index) => _contacts[index];

        public void WriteBeamCorners(Vector3[] corners)
        {
            if (_profile == null || _emitter == null || corners == null || corners.Length < 4) return;
            float near = _profile.BeamNearWidth * 0.5f;
            float far = near + _profile.BeamRange * Mathf.Tan(_profile.BeamHalfAngle * Mathf.Deg2Rad);
            corners[0] = _emitter.TransformPoint(new Vector3(-near, 0f));
            corners[1] = _emitter.TransformPoint(new Vector3(near, 0f));
            corners[2] = _emitter.TransformPoint(new Vector3(far, _profile.BeamRange));
            corners[3] = _emitter.TransformPoint(new Vector3(-far, _profile.BeamRange));
        }

        // Capsule segment plus radius, measured around the actual physical center of mass.
        public float GetTurnRadius()
        {
            var hull = _hull != null ? _hull : GetComponent<CapsuleCollider2D>();
            if (hull == null) return 0f;
            var body = _body != null ? _body : GetComponent<Rigidbody2D>();
            Vector2 scale = new Vector2(Mathf.Abs(hull.transform.lossyScale.x),
                Mathf.Abs(hull.transform.lossyScale.y));
            Vector2 size = Vector2.Scale(hull.size, scale);
            bool vertical = hull.direction == CapsuleDirection2D.Vertical;
            float radius = (vertical ? size.x : size.y) * 0.5f;
            float halfSegment = Mathf.Max(0f, (vertical ? size.y : size.x) * 0.5f - radius);
            Vector2 axis = vertical ? hull.transform.up : hull.transform.right;
            Vector2 center = hull.transform.TransformPoint(hull.offset);
            Vector2 pivot = body.transform.TransformPoint(body.centerOfMass);
            return Mathf.Max((center + axis * halfSegment - pivot).magnitude,
                (center - axis * halfSegment - pivot).magnitude) + radius;
        }
    }
}
