using System;
using AstroMiner.Mining;
using NUnit.Framework;
using UnityEngine;

namespace AstroMiner.Tests
{
    public sealed class AsteroidMiningTests
    {
        private static AsteroidField Field(float spacing, Func<Vector2,float> density)
        {
            int count=Mathf.RoundToInt(8f/spacing)+1;
            var values=new float[count*count];
            for(int y=0;y<count;y++) for(int x=0;x<count;x++)
            {
                Vector2 p=new Vector2(-4f+x*spacing,-4f+y*spacing);
                float d=density(p);
                values[x+y*count]=Mathf.Abs(d)<0.000001f?-0.000001f:d;
            }
            return new AsteroidField(count,count,spacing,new Vector2(-4f,-4f),values);
        }

        private static float Box(Vector2 p) => Mathf.Clamp(3f-Mathf.Max(Mathf.Abs(p.x),Mathf.Abs(p.y)),-0.25f,0.25f);

        [Test]
        public void WorkRemovesActualAreaWithinBudgetAndDoesNotMutateTheDefinition()
        {
            var field=Field(0.125f,Box);
            float initial=field.TotalArea(), removed=0f;
            var beam=new MiningBeam(new Vector2(0f,-3.6f),Vector2.up,2.5f,2f,15f);
            for(int i=0;i<35;i++)
            {
                float step=field.Excavate(beam,0.12f);
                Assert.That(step,Is.InRange(0f,0.1201f)); removed+=step;
            }
            Assert.That(removed,Is.GreaterThan(1f));
            Assert.That(initial-field.TotalArea(),Is.EqualTo(removed).Within(0.0002f));
            Assert.That(field.Sample(new Vector2(0f,-2.8f)),Is.LessThan(0f));
            Assert.That(field.Sample(new Vector2(2f,-2.8f)),Is.GreaterThan(0f));
            var fresh=Field(0.125f,Box);
            Assert.That(fresh.TotalArea(),Is.EqualTo(initial));
        }

        [Test]
        public void FrontLayerShieldsTheRockBehindItAndOutOfRangeWorkDoesNothing()
        {
            var field=Field(0.125f,p=>
            {
                float front=Mathf.Min(p.y+1f,-p.y);
                float back=Mathf.Min(p.y-1f,2f-p.y);
                return Mathf.Min(3f-Mathf.Abs(p.x),Mathf.Max(front,back));
            });
            float backBefore=field.Sample(new Vector2(0f,1.5f));
            var beam=new MiningBeam(new Vector2(0f,-2f),Vector2.up,5f,2f,10f);
            for(int i=0;i<5;i++) field.Excavate(beam,0.02f);
            Assert.That(field.Sample(new Vector2(0f,1.5f)),Is.EqualTo(backBefore));
            Assert.That(field.Sample(new Vector2(0f,-0.9f)),Is.LessThan(0.1f));
            int revision=field.Revision;
            float none=field.Excavate(new MiningBeam(new Vector2(20f,20f),Vector2.up,1f,1f,15f),10f);
            Assert.That(none,Is.Zero); Assert.That(field.Revision,Is.EqualTo(revision));
        }

        [Test]
        public void ExactTriangleTraceDetectsAThinLayerRatherThanSteppingThroughIt()
        {
            var field=Field(0.125f,p=>0.003f-Mathf.Abs(p.y-0.125f));
            Assert.That(field.TraceSurface(new Vector2(0.02f,-1f),new Vector2(0.02f,1f),out Vector2 hit),Is.True);
            Assert.That(hit.y,Is.EqualTo(0.122f).Within(0.0001f));
        }

        [Test]
        public void DensityIsOwnedByEachInstanceAndZeroWorkIsReadOnly()
        {
            var values=new float[]{-1f,-1f,-1f,1f};
            var a=new AsteroidField(2,2,1f,Vector2.zero,values);
            values[3]=99f;
            Assert.That(a.Density(1,1),Is.EqualTo(1f));
            Assert.That(a.Excavate(new MiningBeam(Vector2.zero,Vector2.up,1f,1f,15f),0f),Is.Zero);
            Assert.That(a.Revision,Is.Zero);
            Assert.Throws<ArgumentException>(()=>new AsteroidField(1,2,1f,Vector2.zero,values));
        }

        [Test]
        public void MeshAndNativeColliderAgreeForAClosedHoleAndAcrossChunkSeams()
        {
            var field=Field(0.125f,p=>Mathf.Min(Box(p),p.magnitude-1f));
            var root=new GameObject("Geometry proof",typeof(Rigidbody2D),typeof(CompositeCollider2D));
            Mesh mesh=new Mesh();
            try
            {
                root.transform.position=new Vector3(5000f,5000f);
                root.GetComponent<Rigidbody2D>().bodyType=RigidbodyType2D.Static;
                var composite=root.GetComponent<CompositeCollider2D>();
                composite.generationType=CompositeCollider2D.GenerationType.Manual;
                composite.geometryType=CompositeCollider2D.GeometryType.Polygons;
                composite.vertexDistance=0.003f; composite.offsetDistance=0f;
                double meshArea=0d;
                for(int y=0;y<64;y+=32) for(int x=0;x<64;x+=32)
                {
                    var child=new GameObject("Chunk",typeof(PolygonCollider2D));
                    child.transform.SetParent(root.transform,false);
                    var collider=child.GetComponent<PolygonCollider2D>();
                    collider.compositeOperation=Collider2D.CompositeOperation.Merge;
                    var surface=new AsteroidSurface(); surface.Build(field,new RectInt(x,y,32,32));
                    surface.WriteMesh(mesh,Color.white,false); surface.WriteCollider(collider);
                    Vector3[] vertices=mesh.vertices; int[] triangles=mesh.triangles;
                    for(int i=0;i<triangles.Length;i+=3)
                    {
                        Vector3 a=vertices[triangles[i]],b=vertices[triangles[i+1]],c=vertices[triangles[i+2]];
                        meshArea+=Vector3.Cross(b-a,c-a).magnitude*0.5f;
                    }
                }
                composite.GenerateGeometry(); Physics2D.SyncTransforms();
                Assert.That(meshArea,Is.EqualTo(field.TotalArea()).Within(0.0002f));
                for(int y=-25;y<=25;y++) for(int x=-25;x<=25;x++)
                {
                    Vector2 p=new Vector2(x*0.1f+0.013f,y*0.1f+0.017f);
                    if(Mathf.Abs(field.Sample(p))<0.01f) continue;
                    Assert.That(composite.OverlapPoint((Vector2)root.transform.position+p),
                        Is.EqualTo(field.Sample(p)>0f),"Native overlap at "+p);
                }
            }
            finally { UnityEngine.Object.DestroyImmediate(root); UnityEngine.Object.DestroyImmediate(mesh); }
        }
    }
}
