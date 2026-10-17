using AstroMiner.Mining;
using UnityEngine;

namespace AstroMiner.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ShipDrillController))]
    public sealed class ShipDrillDebug : MonoBehaviour
    {
        [SerializeField] private bool _showZones = true;
        [SerializeField] private bool _showContacts = true;
        private ShipDrillController _drill;
        private readonly Vector3[] _corners = new Vector3[4];

        private void OnDrawGizmos()
        {
            if (!_showZones || !enabled) return;
            if (_drill == null) _drill = GetComponent<ShipDrillController>();
            if (_drill == null || _drill.Profile == null) return;
            _drill.WriteBeamCorners(_corners);
            Gizmos.color = _drill.BeamPreviewActive ? Color.cyan : new Color(0.2f, 0.65f, 0.7f, 0.6f);
            for (int i = 0; i < 4; i++) Gizmos.DrawLine(_corners[i], _corners[(i + 1) % 4]);
            Vector3 center = Application.isPlaying ? (Vector3)_drill.Center :
                transform.TransformPoint(GetComponent<Rigidbody2D>().centerOfMass);
            float radius = _drill.GetTurnRadius() + _drill.Profile.TurnClearance;
            Gizmos.color = Color.yellow;
            Vector3 previous = center + Vector3.right * radius;
            for (int i = 1; i <= 48; i++)
            {
                float angle = i * Mathf.PI * 2f / 48f;
                Vector3 next = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                Gizmos.DrawLine(previous, next);
                previous = next;
            }
            if (!Application.isPlaying) return;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(center, center + (Vector3)_drill.RcsAcceleration * 0.4f);
            if (!_showContacts) return;
            Gizmos.color = Color.red;
            for (int i = 0; i < _drill.ContactCount; i++)
            {
                var contact = _drill.GetContact(i);
                Gizmos.DrawLine(contact.point, contact.point + contact.normal * 0.7f);
            }
        }
    }
}
