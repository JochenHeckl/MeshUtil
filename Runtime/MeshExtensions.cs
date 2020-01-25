using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace de.JochenHeckl.MeshUtil
{
    /// <summary>
    /// Extension class for unity meshes.
    /// </summary>
    public static class MeshExtensions
    {
        /// <summary>
        /// Welds all vertices that are closer to each other than the given threshold.
        /// the index buffers of the mesh will also be update.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="threshold"></param>
        public static int WeldVertices( this Mesh mesh, float threshold )
        {
            // brute force match every vertex against every other

            var vertexRemaps = new Dictionary<int, int>();
            var remappedVertices = new List<Vector3>() { mesh.vertices[0] };

            var duplicates = 0;
            vertexRemaps[0] = 0;

            var sqrThreshold = threshold * threshold;

            for (var vertexIdx = 1; vertexIdx < mesh.vertexCount; vertexIdx++)
            {
                var isDuplicate = false;

                for (var otherVertexIdx = 0; otherVertexIdx < remappedVertices.Count; otherVertexIdx++)
                {
                    if (TestWeldable( mesh.vertices[vertexIdx], remappedVertices[otherVertexIdx], sqrThreshold ))
                    {
                        vertexRemaps[vertexIdx] = otherVertexIdx;
                        isDuplicate = true;
                        duplicates++;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    vertexRemaps[vertexIdx] = remappedVertices.Count;
                    remappedVertices.Add( mesh.vertices[vertexIdx] );
                }
            }

            var subMeshTriangles = new List<int[]>();

            for (var subMeshIdx = 0; subMeshIdx < mesh.subMeshCount; subMeshIdx++)
            {
                subMeshTriangles.Add( RecreateIndexBuffer( mesh.GetTriangles( subMeshIdx ), vertexRemaps ) );
            }

            mesh.Clear();

            mesh.SetVertices( remappedVertices.ToArray() );

            for (var subMeshIdx = 0; subMeshIdx < subMeshTriangles.Count; subMeshIdx++)
            {
                mesh.SetTriangles( subMeshTriangles[subMeshIdx], subMeshIdx );
            }

            return duplicates;
        }

        public static int RemoveDuplicateTriangles( this Mesh mesh )
        {
            throw new NotImplementedException();

            return 0;
        }


        private static bool TestWeldable( Vector3 one, Vector3 other, float sqrThreshold )
        {
            return Vector3.SqrMagnitude( one - other ) <= sqrThreshold;
        }

        private static int[] RecreateIndexBuffer( int[] vertices, Dictionary<int, int> vertexRemaps )
        {
            return vertices.Select( x => vertexRemaps[x] ).ToArray();
        }
    }
}