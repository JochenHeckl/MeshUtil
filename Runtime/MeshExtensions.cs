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
        public static ( IList<Vector3> remappedVertices, IDictionary<int, int> vertexRemaps ) WeldVertices( this Vector3[] vertices, float threshold )
        {
            var vertexRemaps = new Dictionary<int, int>();
            var remappedVertices = new List<Vector3>() { vertices[0] };

            vertexRemaps[0] = 0;

            var sqrThreshold = threshold * threshold;

            for (var vertexIdx = 1; vertexIdx < vertices.Length; vertexIdx++)
            {
                var isDuplicate = false;

                for( var remappedVertexIdx = 0; remappedVertexIdx < remappedVertices.Count; remappedVertexIdx++)
                {
                    if (Vector3.SqrMagnitude( vertices[vertexIdx] - remappedVertices[remappedVertexIdx] ) <= sqrThreshold)
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
                    remappedVertices.Add( vertices[vertexIdx] );
                }
            }

            return (remappedVertices, vertexRemaps);
        }

        public static Vector2[] RemapUvs( this Vector2[] uvs, IDictionary<int,int> vertexRemaps )
        {
            var remappedUvs = new Vector2[vertexRemaps.Values.Max() + 1];

            foreach( var vertexRemap in vertexRemaps )
            {
                remappedUvs[vertexRemap.Value] = uvs[vertexRemap.Key];
            }

            return remappedUvs;
        }

        public static int[] RemapTriangleIndices( this int[] triangleIndices, IDictionary<int, int> vertexRemaps )
        {
            return triangleIndices.Select( x => vertexRemaps[x] ).ToArray();
        }

        public static int[] RemoveDuplicateTriangles( this int[] triangleIndices )
        {
            var triangles = new List<int[]>();

            for( var vertexIndex = 0; vertexIndex < triangleIndices.Length; vertexIndex +=3 )
            {
                var nextTriangle = triangleIndices.Skip( vertexIndex ).Take( 3 );

                if( !triangles.Any( x => TestIdenticalTriangles( x, nextTriangle ) ))
                {
                    triangles.Add( nextTriangle.ToArray() );
                }
            }
            
            return triangles.SelectMany( x => x ).ToArray();
        }

        private static bool TestIdenticalTriangles( int[] one, IEnumerable<int> other )
        {
            return one.OrderBy( x => x ).SequenceEqual( other.OrderBy( x => x) );
        }
    }
}