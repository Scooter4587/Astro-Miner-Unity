using UnityEngine;

namespace AstroMiner.Mining
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ShipDrillController))]
    public sealed class ShipLaserMiner : MonoBehaviour
    {
        [SerializeField] private AsteroidRuntime _asteroid;
        [SerializeField] private LaserMiningProfile _profile;
        private ShipDrillController _drill;
        private Rigidbody2D _body;
        private readonly RaycastHit2D[] _clearanceHits=new RaycastHit2D[32];
        private float _elapsed;
        private readonly System.Diagnostics.Stopwatch _timer=new System.Diagnostics.Stopwatch();
        public float RemovedArea { get; private set; }
        public float LastStepMilliseconds { get; private set; }
        public float MaxStepMilliseconds { get; private set; }

        private void Awake()
        {
            _drill=GetComponent<ShipDrillController>();
            _body=GetComponent<Rigidbody2D>();
        }

        public float GetSafeForwardSpeed(float speed,float acceleration,float skin,float width)
        {
            if(speed<=0f || !_drill.BeamPreviewActive) return speed;
            MiningBeam beam=_drill.GetMiningBeam();
            float distance=speed*speed/(2f*acceleration)+skin;
            int count=Physics2D.BoxCast(beam.Origin,new Vector2(width,0.02f),_body.rotation,
                beam.Forward,new ContactFilter2D {useTriggers=false},_clearanceHits,distance);
            float clear=distance;
            for(int i=0;i<count;i++)
            {
                if(_clearanceHits[i].rigidbody==_body) continue;
                clear=Mathf.Min(clear,Mathf.Max(0f,_clearanceHits[i].distance-skin));
            }
            return Mathf.Min(speed,Mathf.Sqrt(2f*acceleration*clear));
        }

        private void FixedUpdate()
        {
            if(_asteroid==null || _profile==null || !_drill.BeamPreviewActive)
            { _elapsed=0f; return; }
            _elapsed+=Time.fixedDeltaTime;
            if(_elapsed+0.000001f<_profile.UpdateInterval) return;
            float dt=_elapsed; _elapsed=0f;
            _timer.Restart();
            RemovedArea+=_asteroid.Excavate(_drill.GetMiningBeam(),_profile.WorkPerSecond*dt);
            _timer.Stop();
            LastStepMilliseconds=(float)_timer.Elapsed.TotalMilliseconds;
            MaxStepMilliseconds=Mathf.Max(MaxStepMilliseconds,LastStepMilliseconds);
        }

        private void OnDisable() => _elapsed=0f;
    }
}
