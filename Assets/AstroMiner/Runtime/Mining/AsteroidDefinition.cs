using System;
using UnityEngine;

namespace AstroMiner.Mining
{
    [CreateAssetMenu(menuName = "Astro Miner/Asteroid Definition")]
    public sealed class AsteroidDefinition : ScriptableObject
    {
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private float _spacing = 0.125f;
        [SerializeField] private Vector2 _origin;
        [SerializeField] private float[] _density;
        [Header("One prototype material; independent of surface resolution")]
        [SerializeField, Min(0.01f)] private float _hardness = 1f;
        [SerializeField] private Color _rockColor = new Color(0.34f, 0.38f, 0.42f);
        [SerializeField] private Color _interiorColor = new Color(0.035f, 0.055f, 0.07f);

        public int Width => _width;
        public int Height => _height;
        public float Spacing => _spacing;
        public Vector2 Origin => _origin;
        public float Hardness => _hardness;
        public Color RockColor => _rockColor;
        public Color InteriorColor => _interiorColor;

        public AsteroidField CreateState() => new AsteroidField(_width, _height, _spacing, _origin, _density);

#if UNITY_EDITOR
        public void SetAuthoringData(int width, int height, float spacing, Vector2 origin, float[] density)
        {
            // Validate through the same boundary used at runtime. The asset never owns mutable play state.
            var state = new AsteroidField(width, height, spacing, origin, density);
            _width = width;
            _height = height;
            _spacing = spacing;
            _origin = origin;
            _density = state.CopyDensity();
        }
#endif
    }

    public readonly struct MiningBeam
    {
        public readonly Vector2 Origin, Forward, Right;
        public readonly float Range, NearHalfWidth, Slope;

        public MiningBeam(Vector2 origin, Vector2 forward, float range, float nearWidth, float halfAngle)
        {
            Origin = origin;
            Forward = forward.normalized;
            Right = new Vector2(Forward.y, -Forward.x);
            Range = range;
            NearHalfWidth = nearWidth * 0.5f;
            Slope = Mathf.Tan(halfAngle * Mathf.Deg2Rad);
        }

        public Vector2 Point(float lateral, float depth) =>
            Origin + Forward * depth + Right * (lateral * (NearHalfWidth + Slope * depth));

        public bool Contains(Vector2 p)
        {
            Vector2 d = p - Origin;
            float depth = Vector2.Dot(d, Forward);
            return depth >= 0f && depth <= Range &&
                Mathf.Abs(Vector2.Dot(d, Right)) <= NearHalfWidth + Slope * depth;
        }
    }
}
