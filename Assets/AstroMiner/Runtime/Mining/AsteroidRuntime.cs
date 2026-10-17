using System;
using System.Diagnostics;
using UnityEngine;

namespace AstroMiner.Mining
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D),typeof(CompositeCollider2D))]
    public sealed class AsteroidRuntime : MonoBehaviour
    {
        [SerializeField] private AsteroidDefinition _definition;
        [SerializeField] private AsteroidChunk[] _chunks;
        private CompositeCollider2D _composite;
        private readonly Stopwatch _timer=new Stopwatch();
        public AsteroidDefinition Definition => _definition;
        public AsteroidField State { get; private set; }
        public float LastGeometryMilliseconds { get; private set; }
        public float MaxGeometryMilliseconds { get; private set; }
        public int LastRebuiltChunks { get; private set; }
        public int GeometryRevision { get; private set; }

        private void Awake() => Initialize();

        public void Initialize()
        {
            if(State!=null) return;
            if(_definition==null || _chunks==null || _chunks.Length==0)
                throw new InvalidOperationException("Asteroid needs its definition and authored chunks.");
            if((transform.lossyScale-Vector3.one).sqrMagnitude>0.000001f)
                throw new InvalidOperationException("Prototype mining requires asteroid scale (1,1,1).");
            State=_definition.CreateState();
            _composite=GetComponent<CompositeCollider2D>();
            Rebuild(new RectInt(0,0,State.Width-1,State.Height-1));
        }

        public float Excavate(MiningBeam worldBeam,float work)
        {
            Initialize();
            var localBeam=new MiningBeam(transform.InverseTransformPoint(worldBeam.Origin),
                transform.InverseTransformDirection(worldBeam.Forward),worldBeam.Range,
                worldBeam.NearHalfWidth*2f,Mathf.Atan(worldBeam.Slope)*Mathf.Rad2Deg);
            int revision=State.Revision;
            float removed=State.Excavate(localBeam,work/_definition.Hardness);
            if(State.Revision!=revision) Rebuild(State.DirtyCells);
            return removed;
        }

        private void Rebuild(RectInt dirty)
        {
            _timer.Restart(); LastRebuiltChunks=0;
            foreach(var chunk in _chunks)
            {
                if(!chunk.Cells.Overlaps(dirty)) continue;
                chunk.Rebuild(State,_definition.RockColor);
                LastRebuiltChunks++;
            }
            // All modified chunks are committed together before the next physics simulation.
            _composite.GenerateGeometry();
            GeometryRevision=State.Revision;
            _timer.Stop();
            LastGeometryMilliseconds=(float)_timer.Elapsed.TotalMilliseconds;
            MaxGeometryMilliseconds=Mathf.Max(MaxGeometryMilliseconds,LastGeometryMilliseconds);
        }
    }
}
