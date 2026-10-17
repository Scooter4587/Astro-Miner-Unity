using UnityEngine;

namespace AstroMiner.Presentation
{
    // Follow the interpolated ship before the parallax layers sample the camera.
    [DefaultExecutionOrder(-50)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public sealed class ShipCameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        private float _depth;

        private void Awake()
        {
            _depth = transform.position.z;
            if (_target != null) return;
            Debug.LogError("Ship camera requires an assigned target.", this);
            enabled = false;
        }

        private void LateUpdate()
        {
            if (_target == null) return;
            Vector3 position = _target.position;
            transform.position = new Vector3(position.x, position.y, _depth);
        }
    }
}
