using AstroMiner.Mining;
using UnityEngine;

namespace AstroMiner.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ShipDrillController))]
    public sealed class ShipDrillFeedback : MonoBehaviour
    {
        [SerializeField] private LineRenderer _modeRing;
        [SerializeField] private LineRenderer _beamPreview;
        [SerializeField] private LineRenderer[] _rcsJets;
        private ShipDrillController _drill;
        private readonly Vector3[] _beamCorners = new Vector3[4];
        private readonly Vector2[] _circle = new Vector2[48];
        private static readonly Color ReadyColor = new Color(0.15f, 0.85f, 1f, 0.75f);
        private static readonly Color BrakingColor = new Color(1f, 0.65f, 0.15f, 0.85f);

        private void Awake()
        {
            _drill = GetComponent<ShipDrillController>();
            for (int i = 0; i < _circle.Length; i++)
            {
                float angle = i * Mathf.PI * 2f / _circle.Length;
                _circle[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            }
            if (_modeRing != null && _beamPreview != null && _rcsJets != null && _rcsJets.Length == 4) return;
            Debug.LogError("Drill feedback needs the mode ring, beam and four RCS jets.", this);
            enabled = false;
        }

        private void LateUpdate()
        {
            _modeRing.enabled = _drill.IsEngaged;
            Color color = _drill.Mode == ShipControlMode.Stabilizing ? BrakingColor : ReadyColor;
            _modeRing.startColor = _modeRing.endColor = color;
            if (_drill.IsEngaged)
            {
                float radius = _drill.GetTurnRadius() + 0.12f;
                for (int i = 0; i < _circle.Length; i++)
                    _modeRing.SetPosition(i, (Vector3)(_drill.Center + _circle[i] * radius));
            }
            _beamPreview.enabled = _drill.BeamPreviewActive;
            if (_beamPreview.enabled)
            {
                _drill.WriteBeamCorners(_beamCorners);
                for (int i = 0; i < _beamCorners.Length; i++) _beamPreview.SetPosition(i, _beamCorners[i]);
            }
            Vector2 local = transform.InverseTransformDirection(_drill.RcsAcceleration) /
                _drill.Profile.Acceleration;
            for (int i = 0; i < _rcsJets.Length; i++)
            {
                float effort = i == 0 ? local.y : i == 1 ? local.x : i == 2 ? -local.y : -local.x;
                _rcsJets[i].enabled = _drill.IsEngaged && effort > 0.03f;
            }
        }

        private void OnDisable()
        {
            if (_modeRing != null) _modeRing.enabled = false;
            if (_beamPreview != null) _beamPreview.enabled = false;
            if (_rcsJets == null) return;
            foreach (var jet in _rcsJets) if (jet != null) jet.enabled = false;
        }
    }
}
