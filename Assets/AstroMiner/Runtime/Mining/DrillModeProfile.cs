using UnityEngine;

namespace AstroMiner.Mining
{
    [CreateAssetMenu(menuName = "Astro Miner/Drill Mode Profile")]
    public sealed class DrillModeProfile : ScriptableObject
    {
        [Header("RCS movement - prototype values")]
        [SerializeField, Min(0.01f)] private float _moveSpeed = 1.5f;
        [SerializeField, Min(0.01f)] private float _acceleration = 3f;
        [SerializeField, Min(0.02f)] private float _responseTime = 0.25f;
        [SerializeField, Min(1f)] private float _turnSpeed = 60f;
        [SerializeField, Min(1f)] private float _turnAcceleration = 240f;
        [Header("Entry stabilization")]
        [SerializeField, Min(0f)] private float _readySpeed = 0.1f;
        [SerializeField, Min(0f)] private float _readyTurnSpeed = 2f;
        [Header("Contact and clearance")]
        [SerializeField, Min(0.001f)] private float _contactSkin = 0.08f;
        [SerializeField, Min(0f)] private float _turnClearance = 0.35f;
        [Header("Beam working profile")]
        [SerializeField, Min(0.01f)] private float _beamRange = 2.5f;
        [SerializeField, Min(0.01f)] private float _beamNearWidth = 3.2f;
        [SerializeField, Range(1f, 60f)] private float _beamHalfAngle = 15f;

        public float MoveSpeed => _moveSpeed;
        public float Acceleration => _acceleration;
        public float ResponseTime => _responseTime;
        public float TurnSpeed => _turnSpeed;
        public float TurnAcceleration => _turnAcceleration;
        public float ReadySpeed => _readySpeed;
        public float ReadyTurnSpeed => _readyTurnSpeed;
        public float ContactSkin => _contactSkin;
        public float TurnClearance => _turnClearance;
        public float BeamRange => _beamRange;
        public float BeamNearWidth => _beamNearWidth;
        public float BeamHalfAngle => _beamHalfAngle;
    }
}
