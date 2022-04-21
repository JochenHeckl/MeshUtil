# MeshUtil

MeshUtil is a small package that extends the Unity Mesh class.

MeshUtil provides methods like WeldVertices() and RemoveDuplicateTriangles().

Simple Example usage is like this:

```csharp
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
        }
    }
}
```
