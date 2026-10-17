using AstroMiner.Flight;
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

        private void Awake()
        {
            _flight = GetComponent<ShipFlightController>();
            if (_mainExhaust != null && _clockwiseExhaust != null && _counterClockwiseExhaust != null) return;
            Debug.LogError("Ship thrust visuals require all three exhaust references.", this);
            enabled = false;
        }

        private void LateUpdate()
        {
            _mainExhaust.SetActive(_flight.MainEngineActive);
            _clockwiseExhaust.SetActive(_flight.TurnEffort < -0.02f);
            _counterClockwiseExhaust.SetActive(_flight.TurnEffort > 0.02f);
        }

        private void OnDisable()
        {
            if (_mainExhaust != null) _mainExhaust.SetActive(false);
            if (_clockwiseExhaust != null) _clockwiseExhaust.SetActive(false);
            if (_counterClockwiseExhaust != null) _counterClockwiseExhaust.SetActive(false);
        }
    }
}
