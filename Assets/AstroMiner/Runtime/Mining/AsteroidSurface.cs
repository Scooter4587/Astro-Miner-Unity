using System;
using System.Collections.Generic;
using UnityEngine;

namespace AstroMiner.Mining
{
    // Clipped grid triangles produce both mesh triangles and oriented collision loops.
    public sealed class AsteroidSurface
    {
        private struct Edge { public int A,B; public Edge(int a,int b){A=a;B=b;} }
        private readonly List<Vector3> _vertices = new List<Vector3>(4096);
        private readonly List<int> _triangles = new List<int>(8192);
        private readonly List<Color> _colors = new List<Color>(4096);
        private readonly Dictionary<long,int> _vertexIds = new Dictionary<long,int>(4096);
        private readonly Dictionary<long,Edge> _boundary = new Dictionary<long,Edge>(2048);
        private readonly Dictionary<int,int> _firstEdge = new Dictionary<int,int>(256);
        private readonly List<Edge> _edges = new List<Edge>(256);
        private readonly List<int> _nextEdge = new List<int>(256);
        private readonly List<List<Vector2>> _paths = new List<List<Vector2>>();
        private bool[] _visited = Array.Empty<bool>();
        private readonly int[] _polygon = new int[6];
        private readonly int[] _nodes = new int[3];
        private readonly float[] _values = new float[3];
        private AsteroidField _field;
        public int PathCount { get; private set; }
        public int VertexCount => _vertices.Count;

        public void Build(AsteroidField field, RectInt cells)
        {
            _field=field;
            _vertices.Clear(); _triangles.Clear(); _vertexIds.Clear(); _boundary.Clear();
            for(int y=cells.yMin;y<cells.yMax;y++)
                for(int x=cells.xMin;x<cells.xMax;x++)
                {
                    int a=x+y*field.Width, b=a+1, d=a+field.Width, c=d+1;
                    Clip(a,b,c); Clip(a,c,d);
                }
            BuildPaths();
        }

        private Vector2 Point(int node) => _field.Position(node%_field.Width,node/_field.Width);
        private float Value(int node) => _field.Density(node%_field.Width,node/_field.Width);

        private int Vertex(long key, Vector2 point)
        {
            if(_vertexIds.TryGetValue(key,out int index)) return index;
            index=_vertices.Count; _vertices.Add(point); _vertexIds.Add(key,index); return index;
        }

        private int NodeVertex(int node) => Vertex(-1L-node,Point(node));

        private int Crossing(int a,int b,float da,float db)
        {
            float t=da/(da-db);
            if(t<=0.000001f) return NodeVertex(a);
            if(t>=0.999999f) return NodeVertex(b);
            int low=Mathf.Min(a,b), high=Mathf.Max(a,b);
            return Vertex(((long)low<<32)|(uint)high,Vector2.Lerp(Point(a),Point(b),t));
        }

        private void Clip(int a,int b,int c)
        {
            _nodes[0]=a; _nodes[1]=b; _nodes[2]=c;
            for(int i=0;i<3;i++) _values[i]=Value(_nodes[i]);
            int count=0;
            for(int i=0;i<3;i++)
            {
                int previous=(i+2)%3;
                float before=_values[previous], current=_values[i];
                if((before>0f)!=(current>0f))
                    _polygon[count++]=Crossing(_nodes[previous],_nodes[i],before,current);
                if(current>0f) _polygon[count++]=NodeVertex(_nodes[i]);
            }
            // A zero crossing at an original node can appear twice.
            for(int i=count-1;i>=0 && count>1;i--)
                if(_polygon[i]==_polygon[(i+1)%count])
                {
                    for(int j=i;j<count-1;j++) _polygon[j]=_polygon[j+1];
                    count--;
                }
            if(count<3) return;
            for(int i=1;i<count-1;i++)
            {
                Vector2 p=_vertices[_polygon[i]]-_vertices[_polygon[0]];
                Vector2 q=_vertices[_polygon[i+1]]-_vertices[_polygon[0]];
                if(p.x*q.y-p.y*q.x<=0.00000001f) continue;
                _triangles.Add(_polygon[0]); _triangles.Add(_polygon[i]); _triangles.Add(_polygon[i+1]);
                Boundary(_polygon[0],_polygon[i]);
                Boundary(_polygon[i],_polygon[i+1]);
                Boundary(_polygon[i+1],_polygon[0]);
            }
        }

        private void Boundary(int a,int b)
        {
            long key=((long)Mathf.Min(a,b)<<32)|(uint)Mathf.Max(a,b);
            if(!_boundary.Remove(key)) _boundary.Add(key,new Edge(a,b));
        }

        private void BuildPaths()
        {
            _edges.Clear(); _nextEdge.Clear(); _firstEdge.Clear(); PathCount=0;
            foreach(var pair in _boundary)
            {
                Edge e=pair.Value; int index=_edges.Count;
                _nextEdge.Add(_firstEdge.TryGetValue(e.A,out int next)?next:-1);
                _firstEdge[e.A]=index; _edges.Add(e);
            }
            if(_visited.Length<_edges.Count) _visited=new bool[Mathf.NextPowerOfTwo(_edges.Count)];
            Array.Clear(_visited,0,_edges.Count);
            for(int seed=0;seed<_edges.Count;seed++)
            {
                if(_visited[seed]) continue;
                if(_paths.Count<=PathCount) _paths.Add(new List<Vector2>(128));
                var path=_paths[PathCount]; path.Clear();
                int current=seed, start=_edges[seed].A;
                for(int guard=0;guard<=_edges.Count;guard++)
                {
                    Edge edge=_edges[current]; _visited[current]=true;
                    path.Add(_vertices[edge.A]);
                    if(edge.B==start) break;
                    if(!_firstEdge.TryGetValue(edge.B,out int candidate))
                        throw new InvalidOperationException("Open asteroid contour.");
                    int selected=-1; float best=float.NegativeInfinity;
                    Vector2 incoming=_vertices[edge.B]-_vertices[edge.A];
                    for(int e=candidate;e>=0;e=_nextEdge[e])
                    {
                        if(_visited[e]) continue;
                        Vector2 outgoing=_vertices[_edges[e].B]-_vertices[edge.B];
                        float angle=Mathf.Atan2(incoming.x*outgoing.y-incoming.y*outgoing.x,
                            Vector2.Dot(incoming,outgoing));
                        if(angle>best) { best=angle; selected=e; }
                    }
                    if(selected<0) throw new InvalidOperationException("Broken asteroid contour.");
                    current=selected;
                }
                Simplify(path);
                if(path.Count>=3) PathCount++;
            }
        }

        private static void Simplify(List<Vector2> points)
        {
            for(int i=points.Count-1;i>=0 && points.Count>3;i--)
            {
                Vector2 a=points[(i+points.Count-1)%points.Count], b=points[i], c=points[(i+1)%points.Count];
                Vector2 u=b-a,v=c-b;
                if(Mathf.Abs(u.x*v.y-u.y*v.x)<0.0000001f && Vector2.Dot(u,v)>=0f) points.RemoveAt(i);
            }
        }

        public void WriteMesh(Mesh mesh, Color color, bool texture)
        {
            _colors.Clear();
            foreach(Vector3 p in _vertices)
            {
                float shade=texture?0.92f+0.08f*Mathf.Sin(p.x*2.8f+p.y*1.7f):1f;
                _colors.Add(new Color(color.r*shade,color.g*shade,color.b*shade,color.a));
            }
            mesh.Clear();
            mesh.indexFormat=_vertices.Count>65535?UnityEngine.Rendering.IndexFormat.UInt32:
                UnityEngine.Rendering.IndexFormat.UInt16;
            mesh.SetVertices(_vertices); mesh.SetTriangles(_triangles,0);
            mesh.SetColors(_colors); mesh.RecalculateBounds();
        }

        public void WriteCollider(PolygonCollider2D collider)
        {
            collider.enabled=PathCount>0;
            if(PathCount==0) return;
            collider.pathCount=PathCount;
            for(int i=0;i<PathCount;i++) collider.SetPath(i,_paths[i]);
        }
    }
}
