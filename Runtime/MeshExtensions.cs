using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace de.JochenHeckl.Unity.MeshUtil
{
    /// <summary>
    /// Extension class for unity meshes.
    /// </summary>
    public static class MeshExtensions
    {
        /// <summary>
        /// Apply WeldVertices and RemoveDuplicateTriangles to a mesh
        /// </summary>
        /// <param name="mesh">The source mesh to operate on</param>
        /// <param name="vertexWeldDistance">See MeshExtensions.WeldVertices</param>
        /// <returns>a copy of the source mesh with cleaned up vertices and triangles</returns>
        public static Mesh CleanUpDuplicates(this Mesh mesh, float vertexWeldDistance)
        {
            var weldResult = WeldVertices(mesh.vertices, vertexWeldDistance);
            var cleanedMesh = new Mesh();

            cleanedMesh.vertices = weldResult.remappedVertices.ToArray();
            cleanedMesh.uv = mesh.uv.RemapUvs(weldResult.vertexRemaps);

            cleanedMesh.subMeshCount = mesh.subMeshCount;

            foreach (var subMeshIdx in Enumerable.Range(0, mesh.subMeshCount))
            {
                var remappedTrianlges = RemapTriangleIndices(
                    mesh.GetTriangles(subMeshIdx),
                    weldResult.vertexRemaps
                );

                cleanedMesh.SetTriangles(remappedTrianlges.RemoveDuplicateTriangles(), subMeshIdx);
            }

            cleanedMesh.RecalculateNormals();

            return cleanedMesh;
        }

        public static void OffsetVertices(this Mesh mesh, Vector3 offset)
        {
            mesh.vertices = mesh.vertices.Select(x => x + offset).ToArray();
        }

        public static void FlipTriangles(this Mesh mesh)
        {
            for (var subMeshIdx = 0; subMeshIdx < mesh.subMeshCount; subMeshIdx++)
            {
                mesh.SetTriangles(FlipTriangles(mesh.GetTriangles(subMeshIdx)), subMeshIdx);
            }
        }

        private static int[] FlipTriangles(int[] indices)
        {
            var flippedIndices = new int[indices.Length];

            for (var index = 0; index < indices.Length; index += 3)
            {
                flippedIndices[index] = indices[index];
                flippedIndices[index + 1] = indices[index + 2];
                flippedIndices[index + 2] = indices[index + 1];
            }

            return flippedIndices;
        }

        /// <summary>
        /// Welds all vertices that are closer to each other than the given threshold.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="threshold"></param>
        public static (IList<Vector3> remappedVertices, IDictionary<
            int,
            int
        > vertexRemaps) WeldVertices(Vector3[] vertices, float vertexWeldThreshold)
        {
            var vertexRemaps = new Dictionary<int, int>();
            var remappedVertices = new List<Vector3>() { vertices[0] };

            vertexRemaps[0] = 0;

            var sqrThreshold = vertexWeldThreshold * vertexWeldThreshold;

            for (var vertexIdx = 1; vertexIdx < vertices.Length; vertexIdx++)
            {
                var isDuplicate = false;

                for (
                    var remappedVertexIdx = 0;
                    remappedVertexIdx < remappedVertices.Count;
                    remappedVertexIdx++
                )
                {
                    if (
                        Vector3.SqrMagnitude(
                            vertices[vertexIdx] - remappedVertices[remappedVertexIdx]
                        ) <= sqrThreshold
                    )
                    {
                        // this is a duplicate vertex
                        vertexRemaps[vertexIdx] = remappedVertexIdx;
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    vertexRemaps[vertexIdx] = remappedVertices.Count;
                    remappedVertices.Add(vertices[vertexIdx]);
                }
            }

            return (remappedVertices, vertexRemaps);
        }

        public static Vector2[] RemapUvs(this Vector2[] uvs, IDictionary<int, int> vertexRemaps)
        {
            var remappedUvs = new Vector2[vertexRemaps.Values.Max() + 1];

            foreach (var vertexRemap in vertexRemaps)
            {
                remappedUvs[vertexRemap.Value] = uvs[vertexRemap.Key];
            }

            return remappedUvs;
        }

        public static int[] RemapTriangleIndices(
            int[] triangleIndices,
            IDictionary<int, int> vertexRemaps
        )
        {
            return triangleIndices.Select(x => vertexRemaps[x]).ToArray();
        }

        public static int[] RemoveDuplicateTriangles(this int[] triangleIndices)
        {
            var triangles = new List<int[]>();

            for (var vertexIndex = 0; vertexIndex < triangleIndices.Length; vertexIndex += 3)
            {
                var nextTriangle = OrderIndices(
                    triangleIndices.Skip(vertexIndex).Take(3).ToArray()
                );

                if (!triangles.Any(x => TestIdenticalTriangles(x, nextTriangle)))
                {
                    triangles.Add(nextTriangle.ToArray());
                }
            }

            return triangles.SelectMany(x => x).ToArray();
        }

        private static int[] OrderIndices(int[] unorderedIndices)
        {
            if (unorderedIndices.Length > 1)
            {
                int offsetSmallestIndex = 0;
                int smallestIndex = unorderedIndices[0];

                for (var nextIndex = 1; nextIndex < unorderedIndices.Length; nextIndex++)
                {
                    if (unorderedIndices[nextIndex] < smallestIndex)
                    {
                        smallestIndex = unorderedIndices[nextIndex];
                        offsetSmallestIndex = nextIndex;
                    }
                }

                return unorderedIndices
                    .Skip(offsetSmallestIndex)
                    .Concat(unorderedIndices.Take(offsetSmallestIndex))
                    .ToArray();
            }

            return unorderedIndices;
        }

        private static bool TestIdenticalTriangles(int[] one, IEnumerable<int> other)
        {
            return one.OrderBy(x => x).SequenceEqual(other.OrderBy(x => x));
        }
    }
}
