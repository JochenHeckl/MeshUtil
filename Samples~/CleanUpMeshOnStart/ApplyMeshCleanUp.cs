using UnityEngine;
using de.JochenHeckl.MeshUtil;
using System.Linq;

public class ApplyMeshCleanUp : MonoBehaviour
{
    void Start()
    {
        foreach( var meshFilter in GetComponents< MeshFilter >() )
        {
            var baseMesh = meshFilter.sharedMesh;

            var weldResult = baseMesh.vertices.WeldVertices( 0.001f );

            var cleanedMesh = new Mesh();
            cleanedMesh.vertices = weldResult.remappedVertices.ToArray();
            cleanedMesh.uv = baseMesh.uv.RemapUvs( weldResult.vertexRemaps );

            cleanedMesh.subMeshCount = baseMesh.subMeshCount;

            foreach ( var subMeshIdx in Enumerable.Range( 0, baseMesh.subMeshCount ) )
            {
                var remappedTrianlges = baseMesh.GetTriangles( subMeshIdx ).RemapTriangleIndices( weldResult.vertexRemaps );

                cleanedMesh.SetTriangles( remappedTrianlges.RemoveDuplicateTriangles(), subMeshIdx );
            }

            cleanedMesh.RecalculateNormals();

            meshFilter.mesh = cleanedMesh;

            Debug.Log( $"Mesh cleanup reduced vertex count from {baseMesh.vertexCount} to {cleanedMesh.vertexCount}." );
            Debug.Log( $"Mesh cleanup reduced triangle count from {baseMesh.triangles.Length/3} to {cleanedMesh.triangles.Length / 3}." );
        }
    }
}
