using UnityEngine;
using de.JochenHeckl.MeshUtil;

public class ApplyMeshCleanUp : MonoBehaviour
{
    void Start()
    {
        foreach( var meshFilter in GetComponents< MeshFilter >() )
        {
            var baseMesh = meshFilter.sharedMesh;

            meshFilter.mesh = baseMesh.WeldVertices( 0.01f, out var vertexRemaps );
            meshFilter.mesh.uv = baseMesh.RemapUvs( vertexRemaps );
            meshFilter.mesh.RemoveDuplicateTriangles();

            meshFilter.mesh.RecalculateNormals();

            Debug.Log( $"Mesh cleanup reduced vertex count from {baseMesh.vertexCount} to {meshFilter.mesh.vertexCount}." );
            Debug.Log( $"Mesh cleanup reduced triangle count from {baseMesh.triangles.Length/3} to {meshFilter.mesh.triangles.Length / 3}." );
        }
    }
}
