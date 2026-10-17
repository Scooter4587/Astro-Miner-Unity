using UnityEngine;
using UnityEngine.InputSystem;

namespace AstroMiner.Presentation
{
    // Scene inspection only. This does not move the ship or implement the agreed flight model.
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public sealed class CameraPreviewPan : MonoBehaviour
    {
        [Header("SCENE PREVIEW ONLY - arrow keys, Shift, Home")]
        [SerializeField, Min(0f)] private float _panSpeed = 12f;
        [SerializeField] private Vector2 _travelLimit = new Vector2(48f, 48f);
        private Vector3 _origin;

        private void Awake() => _origin = transform.position;

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (keyboard.homeKey.isPressed)
            {
                transform.position = _origin;
                return;
            }

            Vector2 direction = new Vector2(
                (keyboard.rightArrowKey.isPressed ? 1f : 0f) - (keyboard.leftArrowKey.isPressed ? 1f : 0f),
                (keyboard.upArrowKey.isPressed ? 1f : 0f) - (keyboard.downArrowKey.isPressed ? 1f : 0f));
            float speed = _panSpeed * (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed ? 2f : 1f);
            Vector3 position = transform.position + (Vector3)Vector2.ClampMagnitude(direction, 1f) * (speed * Time.unscaledDeltaTime);
            position.x = Mathf.Clamp(position.x, _origin.x - Mathf.Abs(_travelLimit.x), _origin.x + Mathf.Abs(_travelLimit.x));
            position.y = Mathf.Clamp(position.y, _origin.y - Mathf.Abs(_travelLimit.y), _origin.y + Mathf.Abs(_travelLimit.y));
            transform.position = position;
        }
    }
}
