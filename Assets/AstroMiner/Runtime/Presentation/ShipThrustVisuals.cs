using AstroMiner.Flight;
using AstroMiner.Mining;
using UnityEngine;

namespace AstroMiner.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ShipFlightController))]
    public sealed class ShipThrustVisuals : MonoBehaviour
    {
        [SerializeField] private GameObject _mainExhaust;
        [SerializeField] private GameObject _clockwiseExhaust;
        [SerializeField] private GameObject _counterClockwiseExhaust;
        private ShipFlightController _flight;
        private ShipDrillController _drill;

        private void Awake()
        {
            _flight = GetComponent<ShipFlightController>();
            _drill = GetComponent<ShipDrillController>();
            if (_mainExhaust != null && _clockwiseExhaust != null && _counterClockwiseExhaust != null) return;
            Debug.LogError("Ship thrust visuals require all three exhaust references.", this);
            enabled = false;
        }

        private void LateUpdate()
        {
            _mainExhaust.SetActive(_flight.MainEngineActive);
            float turn = _drill != null && _drill.IsEngaged ? _drill.TurnEffort : _flight.TurnEffort;
            _clockwiseExhaust.SetActive(turn < -0.02f);
            _counterClockwiseExhaust.SetActive(turn > 0.02f);
        }

        private void OnDisable()
        {
            if (_mainExhaust != null) _mainExhaust.SetActive(false);
            if (_clockwiseExhaust != null) _clockwiseExhaust.SetActive(false);
            if (_counterClockwiseExhaust != null) _counterClockwiseExhaust.SetActive(false);
        }
    }
}
