using System.Linq;
using UnityEngine;
using de.JochenHeckl.Unity.MeshUtil;

public class ApplyMeshCleanUp : MonoBehaviour
{
    void Start()
    {
        foreach (var meshFilter in GetComponents<MeshFilter>())
        {
            var baseMesh = meshFilter.sharedMesh;
            var weldResult = MeshExtensions.WeldVertices(
                baseMesh.vertices,
                vertexWeldThreshold: 0.001f
            );

            var cleanedMesh = new Mesh();
            cleanedMesh.vertices = weldResult.remappedVertices.ToArray();
            cleanedMesh.uv = baseMesh.uv.RemapUvs(weldResult.vertexRemaps);

            Debug.Log(
                $"Mesh cleanup reduced vertex count from {baseMesh.vertexCount} to {meshFilter.mesh.vertexCount}."
            );
            Debug.Log(
                $"Mesh cleanup reduced triangle count from {baseMesh.triangles.Length / 3} to {meshFilter.mesh.triangles.Length / 3}."
            );

            cleanedMesh.subMeshCount = baseMesh.subMeshCount;

            foreach (var subMeshIdx in Enumerable.Range(0, baseMesh.subMeshCount))
            {
                var remappedTrianlges = MeshExtensions.RemapTriangleIndices(
                    baseMesh.GetTriangles(subMeshIdx),
                    weldResult.vertexRemaps
                );

                cleanedMesh.SetTriangles(remappedTrianlges.RemoveDuplicateTriangles(), subMeshIdx);
            }

            cleanedMesh.RecalculateNormals();

            meshFilter.mesh = cleanedMesh;

            Debug.Log(
                $"Mesh cleanup reduced vertex count from {baseMesh.vertexCount} to {cleanedMesh.vertexCount}."
            );
            Debug.Log(
                $"Mesh cleanup reduced triangle count from {baseMesh.triangles.Length / 3} to {cleanedMesh.triangles.Length / 3}."
            );
        }
    }
}