using UnityEngine;

namespace AstroMiner.Mining
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter),typeof(MeshRenderer),typeof(PolygonCollider2D))]
    public sealed class AsteroidChunk : MonoBehaviour
    {
        [SerializeField] private RectInt _cells;
        private AsteroidSurface _surface;
        private Mesh _runtimeMesh;
        private MeshFilter _filter;
        private PolygonCollider2D _collider;
        public RectInt Cells => _cells;

#if UNITY_EDITOR
        public void SetAuthoringCells(RectInt cells) => _cells=cells;
#endif

        public void Rebuild(AsteroidField field,Color color)
        {
            if(_surface==null)
            {
                _surface=new AsteroidSurface();
                _filter=GetComponent<MeshFilter>();
                _collider=GetComponent<PolygonCollider2D>();
                _runtimeMesh=new Mesh { name="Asteroid chunk - runtime" };
                _runtimeMesh.MarkDynamic();
                _filter.sharedMesh=_runtimeMesh;
            }
            _surface.Build(field,_cells);
            _surface.WriteMesh(_runtimeMesh,color,true);
            _surface.WriteCollider(_collider);
        }

        private void OnDestroy()
        {
            if(_runtimeMesh==null) return;
            if(Application.isPlaying) Destroy(_runtimeMesh); else DestroyImmediate(_runtimeMesh);
        }
    }
}
