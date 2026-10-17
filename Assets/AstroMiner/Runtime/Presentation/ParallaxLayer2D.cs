using System;
using UnityEngine;

namespace AstroMiner.Presentation
{
    [DisallowMultipleComponent]
    public sealed class ParallaxLayer2D : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform;
        [Tooltip("Visible motion relative to the camera: 0 = distant, 1 = world space.")]
        [SerializeField, Range(0f, 1f)] private float _motionRatio = 0.1f;
        [SerializeField, Min(1f)] private float _tileSize = 64f;

        private Vector3 _layerOrigin;
        private Vector3 _cameraOrigin;

        private void Awake()
        {
            if (_cameraTransform == null)
            {
                Debug.LogError("Parallax layer requires an assigned camera.", this);
                enabled = false;
                return;
            }

            _layerOrigin = transform.position;
            _cameraOrigin = _cameraTransform.position;
        }

        private void LateUpdate()
        {
            transform.position = CalculatePosition(
                _layerOrigin, _cameraTransform.position - _cameraOrigin, _motionRatio, _tileSize);
        }

        // Moving by a whole tile keeps the repeated image identical while limiting distance from the camera.
        public static Vector3 CalculatePosition(Vector3 origin, Vector3 cameraDelta, float motionRatio, float tileSize)
        {
            if (tileSize <= 0f) throw new ArgumentOutOfRangeException(nameof(tileSize));
            float ratio = Mathf.Clamp01(motionRatio);
            float x = cameraDelta.x * ratio;
            float y = cameraDelta.y * ratio;
            return new Vector3(
                origin.x + cameraDelta.x - x + Mathf.Round(x / tileSize) * tileSize,
                origin.y + cameraDelta.y - y + Mathf.Round(y / tileSize) * tileSize,
                origin.z);
        }
    }
}
