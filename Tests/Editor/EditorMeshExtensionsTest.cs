using UnityEngine;
using NUnit.Framework;

namespace de.JochenHeckl.MeshUtil.Editor.Test
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

            var weldResult = mesh.vertices.WeldVertices( 0.001f );

            Assert.AreEqual( mesh.vertices.Length - weldResult.remappedVertices.Count, 2 );
            Assert.AreEqual( weldResult.remappedVertices.Count, 3 );
        }

        //// A UnityTest behaves like a coroutine in PlayMode
        //// and allows you to yield null to skip a frame in EditMode
        //[UnityTest]
        //public IEnumerator EditorSampleTestWithEnumeratorPasses() 
        //{
        //	// Use the Assert class to test conditions.
        //	// yield to skip a frame
        //	yield return null;
        //}
    }
}