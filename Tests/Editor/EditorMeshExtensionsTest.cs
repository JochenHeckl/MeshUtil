using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using de.JochenHeckl;
using de.JochenHeckl.MeshUtil;

namespace de.JochenHeckl.MeshUtil.Editor.Test
{

    class EditorMeshExtensionsTest
    {

        [Test]
        public void TestMeshExtensionWeldVertices3SimilarVertices()
        {
            var mesh = new Mesh();
            
            mesh.SetVertices ( new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.right, Vector3.down } );
            mesh.SetTriangles( new int[] { 0, 3, 4 }, 0 );

            Assert.AreEqual( mesh.vertexCount, 5 );

            var duplicates = mesh.WeldVertices( 0.001f );

            Assert.AreEqual( duplicates, 2 );
            Assert.AreEqual( mesh.vertices.Length, 3 );
            Assert.AreEqual( mesh.triangles, new int[] { 0, 1, 2 } );
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