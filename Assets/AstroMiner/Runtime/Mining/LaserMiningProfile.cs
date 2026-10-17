using UnityEngine;

namespace AstroMiner.Mining
{
    [CreateAssetMenu(menuName="Astro Miner/Laser Mining Profile")]
    public sealed class LaserMiningProfile : ScriptableObject
    {
        [SerializeField,Min(0.01f)] private float _workPerSecond=6f;
        [SerializeField,Min(0.02f)] private float _updateInterval=0.06f;
        public float WorkPerSecond => _workPerSecond;
        public float UpdateInterval => _updateInterval;
    }
}
