using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Geometry
{
    public struct Triangle
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;
        public float Area
        {
            get { return Vector3.Cross(b - a, c - a).magnitude * 0.5f; }
        }
        public Vector3 RandomPoint()
        {
            float weightA = Random.value;
            float weightB = Random.value;
            float weightC = Random.value;
            float weightInvertedSum = 1f / (weightA + weightB + weightC);
            weightA *= weightInvertedSum;
            weightB *= weightInvertedSum;
            weightC *= weightInvertedSum;
            return weightA * a + weightB * b + weightC * c;
        }
    }
    public struct WalkableMesh
    {
        private Triangle[] m_Triangles;
        private float m_Area;
        static public WalkableMesh FromNavMesh(NavMeshTriangulation triangulation)
        {
            int triangleCount = triangulation.indices.Length / 3;
            WalkableMesh mesh;
            mesh.m_Triangles = new Triangle[triangleCount];
            mesh.m_Area = 0f;
            for (int i = 0; i < triangleCount; i += 1)
            {
                var iIndice = i * 3;
                mesh.m_Triangles[i].a = triangulation.vertices[triangulation.indices[iIndice]];
                mesh.m_Triangles[i].b = triangulation.vertices[triangulation.indices[iIndice+1]];
                mesh.m_Triangles[i].c = triangulation.vertices[triangulation.indices[iIndice+2]];
                mesh.m_Area += mesh.m_Triangles[i].Area;
            }
            return mesh;
        }
        public Vector3 RandomPoint()
        {
            float randomArea = Random.value * m_Area;
            foreach (var triangle in m_Triangles)
            {
                randomArea -= triangle.Area;
                if (randomArea <= 0f)
                    return triangle.RandomPoint();
            }
            return new Vector3(0f, 0f, 0f);
        }
    }

}
