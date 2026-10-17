using AstroMiner.Mining;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AstroMiner.Flight
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ShipFlightController))]
    public sealed class ShipKeyboardInput : MonoBehaviour
    {
        private ShipFlightController _flight;
        private ShipDrillController _drill;
        private Vector2 _previousAxes;
        private bool _stopWasHeld;
        private bool _modeWasHeld;
        private bool _awaitNeutral;
        private bool _hasFocus = true;

        private void Awake()
        {
            _flight = GetComponent<ShipFlightController>();
            _drill = GetComponent<ShipDrillController>();
        }

        private void Update()
        {
            if (!_hasFocus) return;
            Keyboard keyboard = Keyboard.current;
            Vector2 axes = keyboard == null ? Vector2.zero : new Vector2(
                (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f),
                (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f));
            bool stopHeld = keyboard != null && keyboard.cKey.isPressed;
            bool modeHeld = keyboard != null && keyboard.rKey.isPressed;
            bool previewHeld = Mouse.current != null && Mouse.current.leftButton.isPressed;
            if (_awaitNeutral)
            {
                // A mode/focus transition must not reinterpret a held flight command as drill input.
                _awaitNeutral = axes != Vector2.zero || stopHeld || modeHeld || previewHeld;
                _stopWasHeld = stopHeld;
                _modeWasHeld = modeHeld;
                return;
            }
            if (_drill != null && _drill.enabled && modeHeld && !_modeWasHeld)
            {
                _drill.ToggleMode();
                _previousAxes = Vector2.zero;
                _awaitNeutral = true;
                _modeWasHeld = true;
                return;
            }
            bool drilling = _drill != null && _drill.IsEngaged;
            if (axes != _previousAxes)
            {
                if (drilling) _drill.SetInput(axes.y, -axes.x);
                else _flight.SetMoveDirection(axes.normalized);
                _previousAxes = axes;
            }
            if (stopHeld && !_stopWasHeld)
            {
                if (drilling) _drill.RequestStop();
                else _flight.RequestFullStop();
            }
            if (_drill != null) _drill.SetBeamPreview(previewHeld);
            _stopWasHeld = stopHeld;
            _modeWasHeld = modeHeld;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!isActiveAndEnabled) return;
            _hasFocus = hasFocus;
            if (!hasFocus) ClearInput();
        }

        private void OnDisable() => ClearInput();

        private void ClearInput()
        {
            if (_flight != null) _flight.SetMoveDirection(Vector2.zero);
            if (_drill != null) _drill.ClearInput();
            _previousAxes = Vector2.zero;
            _stopWasHeld = false;
            _modeWasHeld = false;
            _awaitNeutral = true;
        }
    }
}
