using UnityEngine;
using NUnit.Framework;

namespace de.JochenHeckl.Unity.MeshUtil.Editor.Test
{
    class EditorMeshExtensionsTest
    {
        [Test]
        public void TestMeshExtensionWeldVertices3SimilarVertices()
        {
            var mesh = new Mesh();

            mesh.SetVertices(
                new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.right, Vector3.down }
            );
            mesh.SetTriangles(new int[] { 0, 3, 4 }, 0);

            Assert.AreEqual(mesh.vertexCount, 5);

            var weldResult = MeshExtensions.WeldVertices(mesh.vertices, 0.001f);

            Assert.AreEqual(mesh.vertices.Length - weldResult.remappedVertices.Count, 2);
            Assert.AreEqual(weldResult.remappedVertices.Count, 3);
        }

        [Test]
        public void TestTrianglesAreEqualIfIndexOffsetVaries()
        {
            var duplicateTriangleIndices = new int[] { 0, 1, 2, 1, 2, 0 };

            var cleanedUpIndices = MeshExtensions.RemoveDuplicateTriangles(
                duplicateTriangleIndices
            );

            Assert.AreEqual(cleanedUpIndices, new int[] { 0, 1, 2 });
        }
    }
}
