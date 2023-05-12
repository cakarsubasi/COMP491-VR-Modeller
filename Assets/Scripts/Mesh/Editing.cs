﻿using System.Collections;
using Unity.Collections;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Meshes
{
    partial struct UMesh
    {
        [Obsolete("Just manipulate Vertex position directly")]
        /// <summary>
        /// Move a vertex of the given index to the specified position
        /// </summary>
        /// <param name="index">index of the vertex to move</param>
        /// <param name="position">position to move to</param>
        public void MoveVertex(int index, float3 position)
        {
            Vertices[index].Position = position;
            Stream0[] stream = Vertices[index].ToStream();
            int[] indices = Vertices[index].Indices();
            // should probably get rid of this
            NativeArray<Stream0> buffer = new(1, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < indices.Length; ++i)
            {
                buffer[0] = stream[i];
                mesh.SetVertexBufferData<Stream0>(buffer, 0, indices[i], 1);
            }
            buffer.Dispose();
        }

        /// <summary>
        /// Move the given vertices in the selection by the relative offset in position
        /// </summary>
        /// <param name="selection">vertices to move</param>
        /// <param name="position">relative offset</param>
        public void MoveSelectionRelative(IEnumerable<Vertex> selection, float3 position)
        {
            foreach (Vertex vert in selection)
            {
                vert.Position += position;
            }
        }

        /// <summary>
        /// Move the given vertices in the selection by the relative offset in position
        /// </summary>
        /// <param name="selection">vertices to move</param>
        /// <param name="position">relative offset</param>
        public void MoveSelectionRelative(IEnumerable<Edge> selection, float3 position)
        {
            HashSet<Vertex> vertices = extrusionHelper.vertices;
            vertices.Clear();
            foreach (Edge edge in selection)
            {
                vertices.Add(edge.one);
                vertices.Add(edge.two);
            }
            MoveSelectionRelative(vertices, position);
        }

        /// <summary>
        /// Move the given vertices in the selection by the relative offset in position
        /// </summary>
        /// <param name="selection">vertices to move</param>
        /// <param name="position">relative offset</param>
        public void MoveSelectionRelative(IEnumerable<Face> selection, float3 position)
        {
            HashSet<Vertex> vertices = extrusionHelper.vertices;
            vertices.Clear();
            foreach (Face face in selection)
            {
                foreach (Vertex vertex in face.VerticesIter)
                {
                    vertices.Add(vertex);
                }
            }
            MoveSelectionRelative(vertices, position);
        }

        public static float3 GetNormalVector(List<Vertex> vertices)
        {
            float3 norm = default;
            foreach(Vertex vert in vertices)
            {
                norm += vert.Normal;
            }
            return math.normalize(norm);
        }

        public enum TransformType
        {
            BoundingBoxCenter,
            IndividualCenter,
            // ...
        }

        public void TransformVertices(List<int> vertices, Matrix4x4 transform, TransformType type)
        {
            throw new NotImplementedException { };
        }

        public void SetFace()
        {
            throw new NotImplementedException { };
        }

        /// <summary>
        /// Recalculate normals
        /// </summary>
        public void RecalculateNormals()
        {
            foreach (Face face in Faces)
            {
                face.RecalculateNormalFast();
            }
            // vertices derive their normals from the faces
            // so their normals have to be calculated after
            foreach (Vertex vertex in Vertices)
            {
                vertex.RecalculateNormal();
            }
        }

        /// <summary>
        /// Recalculate tangents, RecalculateNormals should be called before this for tangents
        /// to be correct.
        /// </summary>
        public void RecalculateTangents()
        {
            foreach (Vertex vertex in Vertices)
            {
                vertex.RecalculateTangent();
            }
        }

        /// <summary>
        /// Execute all relevant tasks and write to the mesh renderer
        /// </summary>
        public void RecalculateAllAndWriteToMesh()
        {
            OptimizeIndices();
            RecalculateNormals();
            RecalculateTangents();
            WriteAllToMesh();
        }

        /// <summary>
        /// Flip normals
        /// </summary>
        public void FlipNormals()
        {
            foreach (Face face in Faces)
            {
                face.FlipFace(true);
            }
            foreach (Vertex vertex in Vertices)
            {
                vertex.Normal = -vertex.Normal;
            }
        }

        public void Triangulate()
        {
            throw new NotImplementedException { };
        }

        public void TrianglesToQuads()
        {
            throw new NotImplementedException { };
        }

    }
}