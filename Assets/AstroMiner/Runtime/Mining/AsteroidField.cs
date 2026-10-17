using System;
using System.Collections.Generic;
using UnityEngine;

namespace AstroMiner.Mining
{
    // Positive density is solid. A piecewise-linear zero contour is shared by mining, mesh and physics.
    public sealed class AsteroidField
    {
        private readonly float[] _density, _weights, _before;
        private readonly List<int> _touched = new List<int>(512);
        public int Width { get; }
        public int Height { get; }
        public float Spacing { get; }
        public Vector2 Origin { get; }
        public int Revision { get; private set; }
        public RectInt DirtyCells { get; private set; }
        public float RemovedArea { get; private set; }

        public AsteroidField(int width, int height, float spacing, Vector2 origin, float[] density)
        {
            if (width < 2 || height < 2 || spacing <= 0f || !float.IsFinite(spacing) ||
                density == null || density.Length != width * height)
                throw new ArgumentException("Invalid asteroid field dimensions or samples.");
            foreach (float value in density)
                if (!float.IsFinite(value)) throw new ArgumentException("Density must be finite.");
            Width = width; Height = height; Spacing = spacing; Origin = origin;
            _density = (float[])density.Clone();
            _weights = new float[density.Length];
            _before = new float[density.Length];
        }

        public float[] CopyDensity() => (float[])_density.Clone();
        public float Density(int x, int y) => _density[x + y * Width];
        public Vector2 Position(int x, int y) => Origin + new Vector2(x, y) * Spacing;

        public float Sample(Vector2 point)
        {
            Vector2 p = (point - Origin) / Spacing;
            if (p.x < 0f || p.y < 0f || p.x >= Width - 1 || p.y >= Height - 1) return -Spacing;
            int x = Mathf.FloorToInt(p.x), y = Mathf.FloorToInt(p.y);
            float u = p.x - x, v = p.y - y;
            float a = Density(x,y), b = Density(x+1,y), c = Density(x+1,y+1), d = Density(x,y+1);
            return u >= v ? a * (1f-u) + b * (u-v) + c * v :
                a * (1f-v) + c * u + d * (v-u);
        }

        // Traverse each grid triangle, so even a thin remaining layer stops the beam.
        public bool TraceSurface(Vector2 start, Vector2 end, out Vector2 hit)
        {
            hit = default;
            Vector2 p = (start - Origin) / Spacing, direction = (end - start) / Spacing;
            float enter = 0f, exit = 1f;
            if (!ClipAxis(p.x, direction.x, Width-1, ref enter, ref exit) ||
                !ClipAxis(p.y, direction.y, Height-1, ref enter, ref exit)) return false;
            float t = enter;
            for (int step = 0; step < Width + Height + 4 && t < exit; step++)
            {
                Vector2 probe = p + direction * Mathf.Min(exit, t + 0.000001f);
                int x = Mathf.Clamp(Mathf.FloorToInt(probe.x), 0, Width-2);
                int y = Mathf.Clamp(Mathf.FloorToInt(probe.y), 0, Height-2);
                float tx = Mathf.Abs(direction.x) < 0.000001f ? float.PositiveInfinity :
                    ((direction.x > 0f ? x+1 : x) - p.x) / direction.x;
                float ty = Mathf.Abs(direction.y) < 0.000001f ? float.PositiveInfinity :
                    ((direction.y > 0f ? y+1 : y) - p.y) / direction.y;
                float next = Mathf.Min(exit, Mathf.Min(tx, ty));
                if (next <= t + 0.0000001f) { t += 0.000001f; continue; }
                float diagonal = Mathf.Abs(direction.x-direction.y) < 0.000001f ?
                    float.PositiveInfinity : (x-y-p.x+p.y)/(direction.x-direction.y);
                if (diagonal > t && diagonal < next)
                {
                    if (TraceSegment(start,end,t,diagonal,out hit) ||
                        TraceSegment(start,end,diagonal,next,out hit)) return true;
                }
                else if (TraceSegment(start,end,t,next,out hit)) return true;
                t = next;
            }
            return false;
        }

        private bool TraceSegment(Vector2 a, Vector2 b, float start, float end, out Vector2 hit)
        {
            Vector2 p = Vector2.Lerp(a,b,start), q = Vector2.Lerp(a,b,end);
            float dp = Sample(p), dq = Sample(q);
            if (dp > 0f) { hit = p; return true; }
            if (dq > 0f) { hit = Vector2.Lerp(p,q,dp/(dp-dq)); return true; }
            hit = default; return false;
        }

        private static bool ClipAxis(float p, float d, float max, ref float enter, ref float exit)
        {
            if (Mathf.Abs(d) < 0.000001f) return p >= 0f && p < max;
            float a = -p/d, b = (max-p)/d;
            enter = Mathf.Max(enter, Mathf.Min(a,b));
            exit = Mathf.Min(exit, Mathf.Max(a,b));
            return enter < exit;
        }

        public float TotalArea() => Area(new RectInt(0,0,Width-1,Height-1));

        public float Area(RectInt cells)
        {
            double sum = 0d;
            float triangle = Spacing * Spacing * 0.5f;
            for (int y=cells.yMin; y<cells.yMax; y++)
                for (int x=cells.xMin; x<cells.xMax; x++)
                {
                    float a=Density(x,y), b=Density(x+1,y), c=Density(x+1,y+1), d=Density(x,y+1);
                    sum += SolidFraction(a,b,c)*triangle + SolidFraction(a,c,d)*triangle;
                }
            return (float)sum;
        }

        private static float SolidFraction(float a, float b, float c)
        {
            int count = (a>0f?1:0)+(b>0f?1:0)+(c>0f?1:0);
            if (count==0) return 0f;
            if (count==3) return 1f;
            if (count==2) return 1f-SolidFraction(-a,-b,-c);
            if (a>0f) return a/(a-b)*a/(a-c);
            if (b>0f) return b/(b-a)*b/(b-c);
            return c/(c-a)*c/(c-b);
        }

        // Work is capped by actual area change, rather than granting a whole coarse cell.
        public float Excavate(MiningBeam beam, float areaBudget)
        {
            DirtyCells = default;
            if (areaBudget <= 0f || !float.IsFinite(areaBudget)) return 0f;
            _touched.Clear();
            int lanes = Mathf.Max(2, Mathf.CeilToInt(
                2f*(beam.NearHalfWidth+beam.Slope*beam.Range)/(Spacing*0.5f)));
            float radius = Spacing * 1.6f;
            int xmin=Width-1, ymin=Height-1, xmax=0, ymax=0;
            for (int i=0; i<lanes; i++)
            {
                float lateral = (i+0.5f)/lanes*2f-1f;
                if (!TraceSurface(beam.Point(lateral,0f),beam.Point(lateral,beam.Range),out Vector2 hit)) continue;
                Vector2 grid = (hit-Origin)/Spacing;
                int cx=Mathf.RoundToInt(grid.x), cy=Mathf.RoundToInt(grid.y);
                for (int y=Mathf.Max(0,cy-2); y<=Mathf.Min(Height-1,cy+2); y++)
                    for (int x=Mathf.Max(0,cx-2); x<=Mathf.Min(Width-1,cx+2); x++)
                    {
                        Vector2 point=Position(x,y);
                        if (!beam.Contains(point)) continue;
                        float weight = Mathf.Max(0f,1f-Vector2.Distance(point,hit)/radius);
                        if (weight<=0f) continue;
                        int index=x+y*Width;
                        if (_weights[index]==0f)
                        {
                            _touched.Add(index); _before[index]=_density[index];
                            xmin=Mathf.Min(xmin,x); ymin=Mathf.Min(ymin,y);
                            xmax=Mathf.Max(xmax,x); ymax=Mathf.Max(ymax,y);
                        }
                        _weights[index]=Mathf.Max(_weights[index],weight);
                    }
            }
            if (_touched.Count==0) return 0f;
            int left=Mathf.Max(0,xmin-1), bottom=Mathf.Max(0,ymin-1);
            var dirty=new RectInt(left,bottom,Mathf.Min(Width-1,xmax+1)-left,
                Mathf.Min(Height-1,ymax+1)-bottom);
            float before=Area(dirty);
            // Advance the exposed contour by at most two sample bands, then fit the actual area budget.
            // Estimating work from node count underuses power on steep or partly empty gradients.
            float delta=Spacing*2f;
            Apply(delta);
            float removed=Mathf.Max(0f,before-Area(dirty));
            if (removed>areaBudget)
            {
                float low=0f, high=delta;
                for(int i=0;i<8;i++)
                {
                    float mid=(low+high)*0.5f; Apply(mid);
                    if(before-Area(dirty)>areaBudget) high=mid; else low=mid;
                }
                Apply(low); removed=Mathf.Max(0f,before-Area(dirty));
            }
            bool changed=false;
            foreach(int index in _touched)
            {
                changed |= _density[index]!=_before[index];
                _weights[index]=0f;
            }
            if(changed) { Revision++; DirtyCells=dirty; RemovedArea+=removed; }
            return removed;
        }

        private void Apply(float delta)
        {
            foreach(int index in _touched)
                _density[index]=Mathf.Max(-Spacing*2f,_before[index]-delta*_weights[index]);
        }
    }
}
