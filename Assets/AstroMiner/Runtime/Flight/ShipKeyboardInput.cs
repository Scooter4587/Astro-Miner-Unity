using UnityEngine;
using UnityEngine.InputSystem;

namespace AstroMiner.Flight
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ShipFlightController))]
    public sealed class ShipKeyboardInput : MonoBehaviour
    {
        private ShipFlightController _flight;
        private Vector2 _previousDirection;
        private bool _stopWasHeld;
        private bool _hasFocus = true;

        private void Awake() => _flight = GetComponent<ShipFlightController>();

        private void Update()
        {
            if (!_hasFocus) return;
            Keyboard keyboard = Keyboard.current;
            Vector2 direction = keyboard == null ? Vector2.zero : new Vector2(
                (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f),
                (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f)).normalized;
            // A fresh direction cancels stopping; a key already held when C was pressed does not.
            if (direction != _previousDirection)
            {
                _flight.SetMoveDirection(direction);
                _previousDirection = direction;
            }
            bool stopHeld = keyboard != null && keyboard.cKey.isPressed;
            if (stopHeld && !_stopWasHeld) _flight.RequestFullStop();
            _stopWasHeld = stopHeld;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            _hasFocus = hasFocus;
            if (!hasFocus) ClearInput();
        }

        private void OnDisable() => ClearInput();

        private void ClearInput()
        {
            if (_flight != null) _flight.SetMoveDirection(Vector2.zero);
            _previousDirection = Vector2.zero;
            _stopWasHeld = false;
        }
    }
}
