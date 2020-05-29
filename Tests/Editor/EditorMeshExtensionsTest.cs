using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace de.JochenHeckl.Unity.MeshUtil.Editor.Test
{
    class EditorMeshExtensionsTest
    {
        [Test]
        public void TestMeshExtensionWeldVertices3SimilarVertices()
        {
            var mesh = new Mesh();

            mesh.SetVertices( new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.right, Vector3.down } );
            mesh.SetTriangles( new int[] { 0, 3, 4 }, 0 );

            Assert.AreEqual( mesh.vertexCount, 5 );

            var duplicates = mesh.WeldVertices( 0.001f );

            Assert.AreEqual( duplicates, 2 );
            Assert.AreEqual( mesh.vertices.Length, 3 );
            Assert.AreEqual( mesh.triangles, new int[] { 0, 1, 2 } );
        }
    }
}