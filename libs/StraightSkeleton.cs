// Adapted from:
// https://github.com/AggroBird/StraightSkeleton

#define WITH_DEBUG

using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public sealed class StraightSkeletonException : Exception
{
    public StraightSkeletonException(string msg) : base(msg)
    {

    }
}

// xy = coordinate, z = depth
public sealed class StraightSkeletonOutput
{
    private Vector3[] vertices = new Vector3[128];
    private readonly List<System.Range> polygons = new List<System.Range>();

    internal void AddVertex(Vector3 vertex)
    {
        if (TotalVertexCount == vertices.Length)
        {
            Vector3[] newBuffer = new Vector3[vertices.Length << 1];
            Array.Copy(vertices, newBuffer, vertices.Length);
            vertices = newBuffer;
        }
        vertices[TotalVertexCount++] = vertex;
    }
    internal void AddPolygon(System.Range polygon)
    {
        polygons.Add(polygon);
    }

    // Total amount of polygons
    public int TotalPolygonCount => polygons.Count;
    // Center polygons will occupy the first slots in the array
    public int CenterPolygonCount { get; internal set; }
    // Total depth (z axis)
    public float Depth { get; internal set; }
    // Total amount of vertices for all polygons
    public int TotalVertexCount { get; internal set; }

    // Get polygon
    public ReadOnlySpan<Vector3> this[int index] => vertices.AsSpan(polygons[index]);

    public void Clear()
    {
        polygons.Clear();
        CenterPolygonCount = 0;
        TotalVertexCount = 0;
        Depth = 0;

#if WITH_DEBUG
        debugOutput.Clear();
#endif
    }

#if WITH_DEBUG
    private readonly List<(Vector2, Vector2, Color)> debugOutput = new List<(Vector2, Vector2, Color)>();
    public IReadOnlyList<(Vector2 from, Vector2 to, Color color)> DebugOutput => debugOutput;
    internal void DrawDebugLine(Vector2 from, Vector2 to, Color color)
    {
        debugOutput.Add((from, to, color));
    }
    internal void DrawDebugSquare(Vector2 position, float size, Color color)
    {
        Vector2 p0 = position - size.ToVector2() * 0.5f;
        Vector2 p1 = p0 + new Vector2(0, size);
        Vector2 p2 = p1 + new Vector2(size, 0);
        Vector2 p3 = p2 - new Vector2(0, size);
        debugOutput.Add((p0, p1, color));
        debugOutput.Add((p1, p2, color));
        debugOutput.Add((p2, p3, color));
        debugOutput.Add((p3, p0, color));
    }
#else
        public IReadOnlyList<(Vector2 from, Vector2 to, Color color)> DebugOutput => Array.Empty<(Vector2, Vector2, Color)>();
#endif
}

public sealed class StraightSkeletonGenerator
{
    private const float HighPrecisionEpsilon = Mathf.Epsilon;
    private const float MediumPrecisionEpsilon = 0.00001f;
    private const float LowPrecisionEpsilon = 0.001f;

#if WITH_DEBUG
    private static readonly Color[] DebugColors = new Color[]
    {
            Colors.Red,
            Colors.Green,
            Colors.Blue,
            Colors.Yellow,
            Colors.Magenta,
            Colors.Cyan,
            new Color(255 / 255, 174 / 255, 201 / 255, 255 / 255), // Pink
            new Color(255 / 255, 127 / 255, 39 / 255, 255 / 255), // Orange
            new Color(163 / 255, 73 / 255, 164 / 255, 255 / 255), // Purple
    };
#endif


    private enum PolygonSide
    {
        Unknown,
        Left = 1,
        Right = 2,
    }

    private struct PolygonVertex
    {
        public PolygonVertex(Vector2 position, float depth, int pass = 0)
        {
            index = -1;

            this.position = position;
            this.depth = depth;
            this.pass = pass;

            prevPolyVert = -1;
            nextPolyVert = -1;
        }

        public int index;

        // Depth of the polygon vertex into the shrinking
        public float depth;

        // The index of the pass this vertex was inserted
        public int pass;

        // 2D position of the vertex
        public Vector2 position;

        // Neighbours in the polygon linked list
        public int prevPolyVert;
        public int nextPolyVert;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Link(ref PolygonVertex prev, ref PolygonVertex next)
        {
            LinkPrev(ref prev);
            LinkNext(ref next);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LinkPrev(ref PolygonVertex prev)
        {
            prev.nextPolyVert = index;
            prevPolyVert = prev.index;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LinkNext(ref PolygonVertex next)
        {
            next.prevPolyVert = index;
            nextPolyVert = next.index;
        }


        public override string ToString()
        {
            return $"polygon vertex #{index}";
        }
    }

    private struct ChainVertex
    {
        public int index;

        // Depth of the polygon vertex into the shrinking
        public float depth;

        // The index of the pass this vertex was inserted
        public int pass;

        // 2D position of the vertex
        public Vector2 position;

        // Direction of the segment to the next vertex (normalized)
        public Vector2 direction;
        // Distance to the next vertex
        public float length;

        // Angle of the vertex between next and previous
        public float angle;
        // Direction of the bisector (normalized)
        public Vector2 bisector;
        // Speed at which the bisector shrinks
        public float velocity;

        public bool IsReflex { get; private set; }

        // Neighbours in the chain
        public int prevChainVert;
        public int nextChainVert;

        // Neighbouring polygons on this axis
        public int lhsPolyVert;
        public int rhsPolyVert;


        // Recalculate direction and length
        public void RecalculateSegment(Vector2 nextPosition)
        {
            direction = nextPosition - position;
            length = direction.Length();
            if (length <= HighPrecisionEpsilon)
            {
                direction.X = direction.Y = 0;
                length = 0;
            }
            else
            {
                direction /= length;
            }
        }
        // Recalculate bisector and angle
        public void RecalculateBisector(Vector2 prevDirection)
        {
            Vector2 a = direction;
            Vector2 b = -prevDirection;

            float signedAngle = Mathf.Atan2(b.X * a.Y - b.Y * a.X, b.Dot(a));
            float PI2 = Mathf.Pi * 2;
            angle = (signedAngle + PI2) % PI2;
            float angleHalf = angle * 0.5f;
            bisector = Rotate(a, angleHalf);
            velocity = Mathf.Sin(angleHalf);
            IsReflex = angle > Mathf.Pi;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Link(ref ChainVertex prev, ref ChainVertex next)
        {
            prev.nextChainVert = index;
            prevChainVert = prev.index;
            next.prevChainVert = index;
            nextChainVert = next.index;
        }


        public override string ToString()
        {
            return $"chain vertex #{index}";
        }
    }

    private static void EnsureCapacity<T>(ref T[] buffer, int size, bool copyData)
    {
        int currentCapacity = buffer.Length;
        if (size > currentCapacity)
        {
            int newCapacity = currentCapacity;
        IncreaseCapacity:
            newCapacity <<= 1;
            if (newCapacity <= 0)
            {
                throw new StraightSkeletonException("Buffer allocation caused an overflow");
            }
            if (size > newCapacity)
            {
                goto IncreaseCapacity;
            }

            T[] newBuffer = new T[newCapacity];
            if (copyData)
            {
                Array.Copy(buffer, newBuffer, currentCapacity);
            }
            buffer = newBuffer;
        }
    }

    // Chain vertices
    private ChainVertex[] chainVertices = new ChainVertex[16];
    private int chainVertexCount = 0;
    private ref ChainVertex AddChainVertex(ChainVertex chainVertex)
    {
        chainVertex.index = chainVertexCount;
        EnsureCapacity(ref chainVertices, chainVertexCount + 1, true);
        ref ChainVertex result = ref chainVertices[chainVertexCount++];
        result = chainVertex;
        return ref result;
    }

    // Polygon vertices
    private PolygonVertex[] polygonVertices = new PolygonVertex[32];
    private int polygonVertexCount = 0;
    private ref PolygonVertex AddPolygonVertex(PolygonVertex polygonVertex)
    {
        polygonVertex.index = polygonVertexCount;
        EnsureCapacity(ref polygonVertices, polygonVertexCount + 1, true);
        ref PolygonVertex result = ref polygonVertices[polygonVertexCount++];
        result = polygonVertex;
        return ref result;
    }

    // Buffers used for keeping track of chains
    private readonly List<int> activeChains = new List<int>();
    private readonly List<int> splitChains = new List<int>();
    private readonly List<int> abortedChains = new List<int>();

    // Buffers used for intersection events
    private readonly List<int> incidentVertices = new List<int>();
    private readonly List<int> unresolvedChains = new List<int>();
    private readonly List<int> resolvedChains = new List<int>();


    // Allows reusage of straight skeleton buffer data
    public void Generate(ReadOnlySpan<Vector2> points, StraightSkeletonOutput output, float maxDepth = float.MaxValue)
    {
        output.Clear();

        if (points == null || points.Length < 3)
        {
            throw new StraightSkeletonException("Invalid input polygon provided");
        }

        int pointCount = points.Length;

#if WITH_DEBUG
        for (int i = 0; i < pointCount; i++)
        {
            int j = (i + 1) % pointCount;
            output.DrawDebugLine(points[i], points[j], Colors.White);
        }
#endif

        // Max depth less than zero yields one single polygon
        if (maxDepth <= 0)
        {
            for (int i = 0; i < pointCount; i++)
            {
                output.AddVertex(new Vector3(points[i].X, points[i].Y, 0));
            }
            output.AddPolygon(new System.Range(0, pointCount));
            return;
        }

        // Create initial chain
        chainVertexCount = pointCount;
        EnsureCapacity(ref chainVertices, chainVertexCount, false);
        for (int i = 0; i < chainVertexCount; i++)
        {
            chainVertices[i] = new ChainVertex { index = i, position = points[i] };
        }
        for (int i = 0; i < chainVertexCount; i++)
        {
            int j = (i + 1) % chainVertexCount;
            ref ChainVertex prev = ref chainVertices[i];
            ref ChainVertex next = ref chainVertices[j];
            prev.nextChainVert = j;
            next.prevChainVert = i;
            prev.RecalculateSegment(next.position);
        }
        for (int i = 0; i < chainVertexCount; i++)
        {
            int j = (i + 1) % chainVertexCount;
            ref ChainVertex prev = ref chainVertices[i];
            ref ChainVertex next = ref chainVertices[j];
            next.RecalculateBisector(prev.direction);
        }
        activeChains.Clear();
        activeChains.Add(0);
        abortedChains.Clear();

        // Create initial starting polygons
        polygonVertexCount = chainVertexCount * 2;
        EnsureCapacity(ref polygonVertices, polygonVertexCount, false);
        for (int i = 0, idx = 0; i < chainVertexCount; i++)
        {
            int j = (i + 1) % chainVertexCount;
            ref ChainVertex prev = ref chainVertices[i];
            ref ChainVertex next = ref chainVertices[j];
            int lhsIdx = idx++, rhsIdx = idx++;
            PolygonVertex lhs = new PolygonVertex(next.position, 0);
            PolygonVertex rhs = new PolygonVertex(prev.position, 0);
            lhs.index = lhsIdx;
            rhs.index = rhsIdx;
            lhs.prevPolyVert = rhsIdx;
            rhs.nextPolyVert = lhsIdx;
            next.rhsPolyVert = lhsIdx;
            prev.lhsPolyVert = rhsIdx;
            polygonVertices[lhsIdx] = lhs;
            polygonVertices[rhsIdx] = rhs;
        }

        int pass = 0;
        while (activeChains.Count > 0)
        {
            if (pass++ > pointCount)
            {
                throw new StraightSkeletonException("Max polygon shrink iteration count reached");
            }

            splitChains.Clear();
            for (int i = 0; i < activeChains.Count; i++)
            {
                int activeChain = activeChains[i];

                // Find shortest shrink distance
                if (!CalculateShortestShrinkDistance(activeChain, out float distance))
                {
                    throw new StraightSkeletonException("Failed to find distance shortest shrink distance");
                }

                // Clamp to specified max height
                float currentDepth = chainVertices[activeChain].depth;
                bool maxDepthReached = currentDepth + distance >= maxDepth;
                if (maxDepthReached)
                {
                    distance = maxDepth - currentDepth;
                }

                // Apply shrink distance
                if (distance > 0)
                {
                    ApplyShrinkDistance(activeChain, distance);
                }

                // Process intersection events
                ProcessIntersectionEvents(activeChain, pass, maxDepthReached ? abortedChains : splitChains);

                // Append depth
                output.Depth = Mathf.Max(currentDepth + distance, output.Depth);
            }

            activeChains.Clear();
            if (splitChains.Count > 0)
            {
                activeChains.AddRange(splitChains);
            }

#if WITH_DEBUG
            foreach (var chain in activeChains)
            {
                int vertIdx = chain;
                do
                {
                    ref ChainVertex vert = ref chainVertices[vertIdx];
                    ref ChainVertex next = ref chainVertices[vert.nextChainVert];
                    output.DrawDebugLine(vert.position, next.position, Colors.Grey);
                    vertIdx = vert.nextChainVert;
                }
                while (vertIdx != chain);
            }
#endif
        }

        // Link up aborted chains
        for (int i = 0; i < abortedChains.Count; i++)
        {
            int currentVertexCount = output.TotalVertexCount;
            int aborted = abortedChains[i];
            int vertIdx = aborted;
            do
            {
                ref ChainVertex prev = ref chainVertices[vertIdx];
                ref ChainVertex next = ref chainVertices[prev.nextChainVert];
                int lhsIdx = AddPolygonVertex(new PolygonVertex(next.position, next.depth)).index;
                int rhsIdx = AddPolygonVertex(new PolygonVertex(prev.position, prev.depth)).index;
                ref PolygonVertex lhs = ref polygonVertices[lhsIdx];
                ref PolygonVertex rhs = ref polygonVertices[rhsIdx];
                lhs.LinkPrev(ref polygonVertices[next.rhsPolyVert]);
                rhs.LinkNext(ref polygonVertices[prev.lhsPolyVert]);
                lhs.LinkNext(ref rhs);
                vertIdx = next.index;
                output.AddVertex(new Vector3(prev.position.X, prev.position.Y, prev.depth));
            }
            while (vertIdx != aborted);
            output.AddPolygon(new System.Range(currentVertexCount, output.TotalVertexCount));
            output.CenterPolygonCount++;
        }

#if WITH_DEBUG
        List<Vector2> verts = new List<Vector2>();
        List<Vector2> bisectors = new List<Vector2>();
        bool isValid = true;
        for (int i = 0; i < pointCount; i++)
        {
            verts.Clear();
            bisectors.Clear();

            int first = i * 2;
            int index = first;
            int iter = 0;
            bool hasInfiniteLoop = false;

            // Try and find the initial vertex (in case of unconnected loops)
            do
            {
                if (iter++ > pointCount)
                {
                    GD.PrintErr($"polygon #{i} contains an infinite loop");
                    hasInfiniteLoop = true;
                    isValid = false;
                    break;
                }
                ref PolygonVertex vertex = ref polygonVertices[index];
                if (vertex.prevPolyVert == -1)
                {
                    break;
                }
                index = vertex.prevPolyVert;
            }
            while (!index.Equals(first));
            first = index;

            // Fetch all vertices
            iter = 0;
            do
            {
                if (iter++ > pointCount)
                {
                    if (!hasInfiniteLoop)
                    {
                        GD.PrintErr($"polygon #{i} contains an infinite loop");
                        hasInfiniteLoop = true;
                    }
                    isValid = false;
                    break;
                }
                ref PolygonVertex vertex = ref polygonVertices[index];
                verts.Add(vertex.position);
                if (vertex.nextPolyVert == -1)
                {
                    GD.PrintErr($"polygon #{i} contains null connections");
                    isValid = false;
                    break;
                }
                index = vertex.nextPolyVert;
            }
            while (index != first);

            if (verts.Count >= 3)
            {
                // Generate bisectors for the debug shapes
                for (int j = 0; j < verts.Count; j++)
                {
                    int k = (j + 1) % verts.Count;
                    int l = (j + verts.Count - 1) % verts.Count;
                    Vector2 a = (verts[k] - verts[j]).Normalized();
                    Vector2 b = -(verts[j] - verts[l]).Normalized();
                    float signedAngle = Mathf.Atan2(b.X * a.Y - b.Y * a.X, b.Dot(a));
                    float PI2 = Mathf.Pi * 2;
                    float angle = (signedAngle + PI2) % PI2;
                    float angleHalf = angle * 0.5f;
                    Vector2 bisector = Rotate(a, angleHalf);
                    float velocity = Mathf.Sin(angleHalf);
                    bisectors.Add(bisector * (LowPrecisionEpsilon / velocity));
                }

                // Output polygons
                Color debugColor = DebugColors[i % DebugColors.Length];
                for (int j = 0; j < verts.Count; j++)
                {
                    int k = (j + 1) % verts.Count;
                    output.DrawDebugLine(verts[j] + bisectors[j], verts[k] + bisectors[k], debugColor);
                }
            }
            else
            {
                GD.PrintErr($"Polygon {i} does not contain enough vertices");
                isValid = false;
            }
        }
        if (!isValid) return;
#endif

        // Output result
        for (int i = 0; i < pointCount; i++)
        {
            int currentVertexCount = output.TotalVertexCount;
            int first = i * 2;
            int index = first;
            do
            {
                ref PolygonVertex vertex = ref polygonVertices[index];
                output.AddVertex(new Vector3(vertex.position.X, vertex.position.Y, vertex.depth));
                index = vertex.nextPolyVert;
            }
            while (index != first);
            output.AddPolygon(new System.Range(currentVertexCount, output.TotalVertexCount));
        }
    }
    public StraightSkeletonOutput Generate(ReadOnlySpan<Vector2> points, float maxDepth = float.MaxValue)
    {
        StraightSkeletonOutput output = new StraightSkeletonOutput();
        Generate(points, output, maxDepth);
        return output;
    }

    // Calculates the shortest shrink distance at which point a collision will occur
    private bool CalculateShortestShrinkDistance(int chain, out float distance)
    {
        distance = float.PositiveInfinity;

        int vertIdx = chain;
        do
        {
            ref ChainVertex vert = ref chainVertices[vertIdx];
            ref ChainVertex next = ref chainVertices[vert.nextChainVert];

            // Project the two bisectors onto each other and find the shortest collapse distance (edge event)
            Vector2 n0 = new Vector2(-vert.bisector.Y, vert.bisector.X);
            Vector2 n1 = new Vector2(next.bisector.Y, -next.bisector.X);

            float entry0 = (next.position - vert.position).Dot(n1) / vert.bisector.Dot(n1);
            float entry1 = (vert.position - next.position).Dot(n0) / next.bisector.Dot(n0);

            float entry = Mathf.Min(entry0 * vert.velocity, entry1 * next.velocity);
            if (!float.IsInfinity(entry) && entry >= 0)
            {
                if (entry < distance)
                {
                    distance = entry;
                }
            }

            if (vert.IsReflex)
            {
                // Find the earliest segment intersect with a reflex vertex (split event)
                int segIdx = vertIdx;
                do
                {
                    ref ChainVertex segBeg = ref chainVertices[segIdx];
                    ref ChainVertex segEnd = ref chainVertices[segBeg.nextChainVert];
                    if (segIdx != vertIdx && segBeg.nextChainVert != vertIdx)
                    {
                        Vector2 perp = new Vector2(segBeg.direction.Y, -segBeg.direction.X);
                        if (TracePlane(segBeg.position, perp, vert.position, vert.bisector, out entry))
                        {
                            // Calculate time of collision (magic)
                            float angleBetween = AngleBetween(vert.bisector, segBeg.direction);
                            if (angleBetween > 0)
                            {
                                entry /= (1 / Mathf.Sin(angleBetween) + 1 / vert.velocity);
                                if (!float.IsInfinity(entry) && entry >= 0)
                                {
                                    // Grow the segment along the two bisectors of its vertices and project
                                    // the collision point to make sure this vertex will actually collide with
                                    // this segment in the future.
                                    Vector2 s0 = segBeg.position + segBeg.bisector * (entry / segBeg.velocity);
                                    Vector2 s1 = segEnd.position + segEnd.bisector * (entry / segEnd.velocity);
                                    float length = (s1 - s0).Dot(segBeg.direction);
                                    if (length > 0)
                                    {
                                        Vector2 point = vert.position + vert.bisector * (entry / vert.velocity);
                                        float project = (point - s0).Dot(segBeg.direction);
                                        if (project >= -MediumPrecisionEpsilon && project <= length + MediumPrecisionEpsilon)
                                        {
                                            if (entry < distance)
                                            {
                                                distance = entry;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    segIdx = segBeg.nextChainVert;
                }
                while (segIdx != vertIdx);
            }

            vertIdx = vert.nextChainVert;
        }
        while (vertIdx != chain);

        return distance != float.PositiveInfinity;
    }

    // Shrink the polygon
    private void ApplyShrinkDistance(int chain, float distance)
    {
        int vertIdx = chain;
        do
        {
            ref ChainVertex vert = ref chainVertices[vertIdx];
            vert.position += vert.bisector * (distance / vert.velocity);
            vert.depth += distance;
            vertIdx = vert.nextChainVert;
        }
        while (vertIdx != chain);
        do
        {
            ref ChainVertex prev = ref chainVertices[vertIdx];
            ref ChainVertex next = ref chainVertices[prev.nextChainVert];
            prev.RecalculateSegment(next.position);
            vertIdx = prev.nextChainVert;
        }
        while (vertIdx != chain);
        do
        {
            ref ChainVertex prev = ref chainVertices[vertIdx];
            ref ChainVertex next = ref chainVertices[prev.nextChainVert];
            next.RecalculateBisector(prev.direction);
            vertIdx = prev.nextChainVert;
        }
        while (vertIdx != chain);
    }

    // Check for any intersections (results in one or more new chains)
    private void ProcessIntersectionEvents(int chain, int pass, List<int> appendResult)
    {
        // This phase scans over all the vertices and checks for split events.
        // In the case of a split event, it will emit a new vertex parallel to the segment.
        int vertIdx = chain;
        do
        {
            ref ChainVertex vert = ref chainVertices[vertIdx];

            // No need to resolve intersections with inserted split polygons
            if (vert.pass != pass)
            {
                // Find closest intersect
                IntersectResult bestResult = IntersectResult.None;
                int segIdx = vertIdx;
                do
                {
                    ref ChainVertex seg = ref chainVertices[segIdx];
                    if (segIdx != vertIdx && seg.nextChainVert != vertIdx)
                    {
                        TraceSegment(ref vert, ref seg, ref chainVertices[seg.nextChainVert], LowPrecisionEpsilon, ref bestResult);
                    }

                    segIdx = seg.nextChainVert;
                }
                while (segIdx != vertIdx);

                if (bestResult)
                {
                    // Snap to intersect location
                    vert.position = bestResult.point;
                    // Insert new vertex on segment
                    ref ChainVertex insert = ref AddChainVertex(new ChainVertex { position = bestResult.point, depth = vert.depth, pass = pass });
                    ref ChainVertex segBeg = ref chainVertices[bestResult.segment];
                    ref ChainVertex segEnd = ref chainVertices[segBeg.nextChainVert];
                    insert.Link(ref segBeg, ref segEnd);
                    RecalculateSegments(ref insert);
                    // Create an empty polygon vertex at the split position
                    insert.lhsPolyVert = insert.rhsPolyVert = AddPolygonVertex(new PolygonVertex(bestResult.point, segBeg.depth, pass)).index;
                }
            }

            vertIdx = vert.nextChainVert;
        }
        while (vertIdx != chain);

        // This stage takes all incident vertices and splits them up into seperate chains.
        // It also emits polygon vertices for the skeleton in the case of an edge or split event.
        resolvedChains.Clear();
        unresolvedChains.Clear();
        unresolvedChains.Add(chain);
    TraceNext:
        while (unresolvedChains.Count > 0)
        {
            int lastIndex = unresolvedChains.Count - 1;
            int unresolved = unresolvedChains[lastIndex];
            unresolvedChains.RemoveAt(lastIndex);

            vertIdx = unresolved;
            do
            {
                ref ChainVertex vert = ref chainVertices[vertIdx];

                // No need to resolve vertices that we have already checked for incident in this pass
                if (vert.pass != pass)
                {
                    vert.pass = pass;

                    incidentVertices.Clear();

                    // Collect all vertices that are incident to the current vertex
                    int otherIdx = vertIdx;
                    do
                    {
                        ref ChainVertex other = ref chainVertices[otherIdx];
                        if (vertIdx != otherIdx && IncidentVertices(vert.position, other.position, LowPrecisionEpsilon * 2))
                        {
                            other.pass = pass;

                            incidentVertices.Add(otherIdx);
                        }

                        otherIdx = other.nextChainVert;
                    }
                    while (otherIdx != vertIdx);

                    if (incidentVertices.Count > 0)
                    {
                        // Resolve the current vertex as well
                        incidentVertices.Add(vertIdx);

                        // Split into subshapes
                        int incidentCount = incidentVertices.Count;
                        for (int i = 0; i < incidentCount; i++)
                        {
                            ref ChainVertex prevIncident = ref chainVertices[incidentVertices[i]];
                            ref ChainVertex nextIncident = ref chainVertices[incidentVertices[(i + 1) % incidentCount]];
                            float depth = prevIncident.depth;

                            // If there are vertices between the previous and next incident vertices,
                            // we have to split them off into a new chain
                            if (prevIncident.nextChainVert != nextIncident.index)
                            {
                                ref ChainVertex insert = ref AddChainVertex(new ChainVertex { position = vert.position, depth = depth, pass = pass });

                                int nextPolygon = prevIncident.lhsPolyVert;
                                if (polygonVertices[nextPolygon].pass != pass)
                                {
                                    // Insert a polygon vertex on the right side of the left polygon
                                    ref PolygonVertex lhs = ref AddPolygonVertex(new PolygonVertex(vert.position, depth, pass));
                                    lhs.LinkNext(ref polygonVertices[nextPolygon]);
                                    insert.lhsPolyVert = lhs.index;
                                }
                                else
                                {
                                    // If the polygon point inserted through a split event, we dont have to insert a new vertex
                                    insert.lhsPolyVert = nextPolygon;
                                }

                                int prevPolygon = nextIncident.rhsPolyVert;
                                if (polygonVertices[prevPolygon].pass != pass)
                                {
                                    // Insert a polygon vertex on the left side of the right polygon
                                    ref PolygonVertex rhs = ref AddPolygonVertex(new PolygonVertex(vert.position, depth, pass));
                                    rhs.LinkPrev(ref polygonVertices[prevPolygon]);
                                    insert.rhsPolyVert = rhs.index;
                                }
                                else
                                {
                                    // If the polygon point inserted through a split event, we dont have to insert a new vertex
                                    insert.rhsPolyVert = prevPolygon;
                                }

                                insert.Link(ref chainVertices[nextIncident.prevChainVert], ref chainVertices[prevIncident.nextChainVert]);
                                RecalculateSegments(ref insert);
                                unresolvedChains.Add(insert.index);
                            }
                            else
                            {
                                // Close into a triangle
                                ref PolygonVertex end = ref AddPolygonVertex(new PolygonVertex(vert.position, depth, pass));
                                end.Link(ref polygonVertices[nextIncident.rhsPolyVert], ref polygonVertices[prevIncident.lhsPolyVert]);
                            }
                        }

                        goto TraceNext;
                    }
                }

                vertIdx = vert.nextChainVert;
            }
            while (vertIdx != unresolved);

            resolvedChains.Add(unresolved);
        }

        // Final stage: rebuild new chains from the split results
        for (int i = 0; i < resolvedChains.Count; i++)
        {
            int vertCount = 0;
            int first = resolvedChains[i];
            vertIdx = first;
            do
            {
                ref ChainVertex vert = ref chainVertices[vertIdx];
                vertIdx = vert.nextChainVert;
                vertCount++;
            }
            while (vertIdx != first);

            if (vertCount > 2)
            {
                // New valid chain
                appendResult.Add(first);
            }
            else if (vertCount == 2)
            {
                // If we have only two vertices we can collapse the segment and close the polygons
                ref ChainVertex prev = ref chainVertices[first];
                ref ChainVertex next = ref chainVertices[prev.nextChainVert];

                ref PolygonVertex prevLhs = ref polygonVertices[prev.lhsPolyVert];
                ref PolygonVertex nextRhs = ref polygonVertices[next.rhsPolyVert];
                if (prevLhs.prevPolyVert == -1 && nextRhs.nextPolyVert == -1)
                {
                    prevLhs.LinkPrev(ref nextRhs);
                }

                ref PolygonVertex prevRhs = ref polygonVertices[prev.rhsPolyVert];
                ref PolygonVertex nextLhs = ref polygonVertices[next.lhsPolyVert];
                if (prevRhs.nextPolyVert == -1 && nextLhs.prevPolyVert == -1)
                {
                    prevRhs.LinkNext(ref nextLhs);
                }
            }
        }
    }


    // Recalculate a node and its neighbours
    private void RecalculateSegments(ref ChainVertex vert)
    {
        ref ChainVertex prev = ref chainVertices[vert.prevChainVert];
        ref ChainVertex next = ref chainVertices[vert.nextChainVert];

        prev.RecalculateSegment(vert.position);
        vert.RecalculateSegment(next.position);

        prev.RecalculateBisector(chainVertices[prev.prevChainVert].direction);
        vert.RecalculateBisector(prev.direction);
        next.RecalculateBisector(vert.direction);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float AngleBetween(Vector2 from, Vector2 to)
    {
        float sqrt = Mathf.Sqrt(from.LengthSquared() * to.LengthSquared());
        if (sqrt < 1e-15f) return 0f;
        return Mathf.Acos(Mathf.Clamp(from.Dot(to) / sqrt, -1f, 1f));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector2 Rotate(Vector2 vec, float angle)
    {
        float xr = Mathf.Sin(-angle);
        float yr = Mathf.Cos(-angle);
        float x = vec.X * yr - vec.Y * xr;
        float y = vec.X * xr + vec.Y * yr;
        return new Vector2(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IncidentVertices(Vector2 a, Vector2 b, float accuracy)
    {
        return (a - b).LengthSquared() < accuracy * accuracy;
    }

    // Trace a plane (position, normal) from a ray (origin, direction).
    // Returns false if the ray starts below the plane or the ray is facing away from the plane.
    private static bool TracePlane(Vector2 position, Vector2 normal, Vector2 origin, Vector2 direction, out float entry)
    {
        if ((origin - position).Dot(normal) >= 0)
        {
            float denominator = direction.Dot(normal);
            if (denominator.Abs() >= HighPrecisionEpsilon)
            {
                entry = (position - origin).Dot(normal) / denominator;
                return entry >= 0;
            }
        }
        entry = 0;
        return false;
    }

    private struct IntersectResult
    {
        public static readonly IntersectResult None = new IntersectResult { distanceSqr = float.PositiveInfinity, segment = -1 };

        public Vector2 point;
        public float distanceSqr;
        public int segment;

        public static implicit operator bool(IntersectResult intersectResult)
        {
            return intersectResult.segment != -1;
        }
    }

    // Calculate the intersection of a vertex on a segment, and if the distance is closer than the previous, update the result.
    private void TraceSegment(ref ChainVertex vert, ref ChainVertex segBeg, ref ChainVertex segEnd, float accuracy, ref IntersectResult result)
    {
        float accuracySqr = accuracy * accuracy;

        // See if we are not near the segment origin
        float dist = vert.position.DistanceSquaredTo(segBeg.position);
        if (dist > accuracySqr)
        {
            // See if we are not near the segment end
            dist = vert.position.DistanceSquaredTo(segEnd.position);
            if (dist > accuracySqr)
            {
                // Project the point onto the segment (if we are currently not incident)
                if (segBeg.length > 0)
                {
                    // Project on the segment along the length
                    Vector2 relative = vert.position - segBeg.position;
                    float project = relative.Dot(segBeg.direction);
                    if (project >= 0 && project <= segBeg.length)
                    {
                        // Project on the segment along the width (with an error margin)
                        Vector2 perp = new Vector2(segBeg.direction.Y, -segBeg.direction.X);
                        float dot = perp.Dot(relative);
                        if (dot.Abs() <= accuracy)
                        {
                            // The result point will be projected along the segment to ensure 
                            // the result intersection point will not warp the segment
                            Vector2 point = segBeg.position + segBeg.direction * project;
                            dist = vert.position.DistanceSquaredTo(point);
                            if (dist < result.distanceSqr)
                            {
                                result.point = point;
                                result.distanceSqr = dist;
                                result.segment = segBeg.index;
                            }
                        }
                    }
                }
            }
        }
    }
}