﻿using System.Linq;

using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;

using Meshes;
using Meshes.Test;
using static Unity.Mathematics.math;
using Unity.Mathematics;
using UnityEngine;

public class ExtrusionTestScript
{
    /// <summary>
    /// From one vertex, extrude a single vertex
    /// Should end with 2 vertices, 1 edge, 0 faces
    /// </summary>
    [Test]
    public void TestExtrudeSingleVertex()
    {
        UMesh mesh = Creators.Empty;

        Vertex vert1 = mesh.CreateVertex();
        mesh.Extrude(vert1);
        vert1.Position = float3(1f, 0f, 0f);

        mesh.OptimizeIndices();
        mesh.RecalculateNormals();

        Assert.AreEqual(2, mesh.VertexCount);
        Assert.AreEqual(1, vert1.EdgeCount);
        Assert.AreEqual(0, mesh.FaceCount);
        Assert.AreEqual(float3(0f, 0f, 0f), vert1.GetConnectedVertices()[0].Position);
    }

    [Test]
    /// <summary>
    /// From two vertices and one edge, select one vertex, and extrude a single vertex
    /// Should end with 3 vertices, 2 edges, 0 faces
    /// </summary>
    public void TestExtrudeSingleVertex2()
    {
        UMesh mesh = Creators.Empty;

        Vertex vert1 = mesh.CreateVertex();
        Vertex vert2 = mesh.CreateVertexConnectedTo(vert1, out _);
        vert1.Position = float3(1f, 0f, 0f);
        mesh.Extrude(vert1);
        vert1.Position = float3(2f, 0f, 0f);

        mesh.OptimizeIndices();
        mesh.RecalculateNormals();

        Vertex extrudedVertex = vert1.GetConnectedVertices()[0];

        Assert.AreEqual(3, mesh.VertexCount);
        Assert.AreEqual(2, mesh.EdgeCount);
        Assert.AreEqual(0, mesh.FaceCount);

        Assert.AreEqual(1, vert1.EdgeCount);

        Assert.AreEqual(float3(2f, 0f, 0f), vert1.Position);
        Assert.AreEqual(float3(1f, 0f, 0f), extrudedVertex.Position);
        Assert.AreEqual(float3(0f, 0f, 0f), vert2.Position);
    }

    [Test]
    public void TestExtrudeSingleEdge()
    {
        UMesh mesh = Creators.Empty;

        Vertex vert1 = mesh.CreateVertex();
        Vertex vert2 = mesh.CreateVertexConnectedTo(vert1, out _);
        vert2.Position = float3(1f, 0f, 0f);
        Edge edge = vert1.GetEdgeTo(vert2);

        mesh.Extrude(edge);
        edge.MoveRelative(float3(0f, 1f, 0f));

        mesh.OptimizeIndices();
        mesh.RecalculateNormals();

        Assert.AreEqual(4, mesh.VertexCount);
        Assert.AreEqual(4, mesh.EdgeCount);
        Assert.AreEqual(2, vert1.EdgeCount);
        Assert.AreEqual(2, vert2.EdgeCount);
        Assert.AreEqual(1, mesh.FaceCount);

        Vertex vert3 = mesh.FindByPosition(float3(0f, 0f, 0f));
        Vertex vert4 = mesh.FindByPosition(float3(1f, 0f, 0f));

        Assert.AreEqual(2, vert3.EdgeCount);
        Assert.AreEqual(2, vert4.EdgeCount);

        Assert.IsTrue(vert1.IsConnected(vert2));
        Assert.IsTrue(vert1.IsConnected(vert3));
        Assert.IsTrue(vert2.IsConnected(vert4));
        Assert.IsTrue(vert3.IsConnected(vert4));
    }

    [Test]
    public void TestExtrudeSingleEdge2()
    {
        UMesh mesh = Creators.Empty;

        Vertex vert1 = mesh.CreateVertex();
        Vertex vert2 = mesh.CreateVertexConnectedTo(vert1, out _);
        vert2.Position = float3(1f, 0f, 0f);

        List<Vertex> selection = new List<Vertex> { vert1, vert2 };
        mesh.Extrude(selection);
        mesh.MoveSelectionRelative(selection, float3(0f, 1f, 0f));

        mesh.OptimizeIndices();
        mesh.RecalculateNormals();

        Assert.AreEqual(4, mesh.VertexCount);
        Assert.AreEqual(4, mesh.EdgeCount);
        Assert.AreEqual(2, vert1.EdgeCount);
        Assert.AreEqual(2, vert2.EdgeCount);
        Assert.AreEqual(1, mesh.FaceCount);

        Vertex vert3 = mesh.FindByPosition(float3(0f, 0f, 0f));
        Vertex vert4 = mesh.FindByPosition(float3(1f, 0f, 0f));

        Assert.AreEqual(2, vert3.EdgeCount);
        Assert.AreEqual(2, vert4.EdgeCount);

        Assert.IsTrue(vert1.IsConnected(vert2));
        Assert.IsTrue(vert1.IsConnected(vert3));
        Assert.IsTrue(vert2.IsConnected(vert4));
        Assert.IsTrue(vert3.IsConnected(vert4));
    }

    [Test]
    public void TestExtrudeSingleEdgeFromFace()
    {
        UMesh mesh = Creators.Quad;
        Vertex vert1 = mesh.FindByPosition(float3(0f, 1f, 0f));
        Vertex vert2 = mesh.FindByPosition(float3(1f, 1f, 0f));
        Face face = vert1.GetFaces()[0];

        List<Vertex> selected = new() { vert1, vert2 };

        mesh.Extrude(selected);
        mesh.MoveSelectionRelative(selected, float3(0f, 1f, 0f));

        mesh.OptimizeIndices();
        mesh.RecalculateNormals();

        Assert.AreEqual(6, mesh.VertexCount);
        Assert.AreEqual(7, mesh.EdgeCount);
        Assert.AreEqual(2, mesh.FaceCount);

        Assert.AreEqual(2, vert1.EdgeCount);
        Assert.AreEqual(1, vert1.FaceCount);
        Assert.AreEqual(2, vert2.EdgeCount);
        Assert.AreEqual(1, vert2.FaceCount);

        Assert.IsFalse(face.ContainsVertex(vert1));
        Assert.IsFalse(vert1.IsConnected(face));
        Assert.IsFalse(face.ContainsVertex(vert2));
        Assert.IsFalse(vert2.IsConnected(face));

        Vertex createdVert1 = mesh.FindByPosition(float3(0f, 1f, 0f));
        Vertex createdVert2 = mesh.FindByPosition(float3(1f, 1f, 0f));

        Assert.AreEqual(3, createdVert1.EdgeCount);
        Assert.AreEqual(2, createdVert1.FaceCount);
        Assert.AreEqual(3, createdVert2.EdgeCount);
        Assert.AreEqual(2, createdVert2.FaceCount);

        Assert.IsTrue(face.ContainsVertex(createdVert1));
        Assert.IsTrue(createdVert1.IsConnected(face));
        Assert.IsTrue(face.ContainsVertex(createdVert2));
        Assert.IsTrue(createdVert2.IsConnected(face));
    }

    [Test]
    public void TestExtrudeSingleEdgeFromFace2()
    {
        UMesh mesh = Creators.Quad;
        Vertex vert1 = mesh.FindByPosition(float3(0f, 1f, 0f));
        Vertex vert2 = mesh.FindByPosition(float3(0f, 0f, 0f));
        Face face = vert1.GetFaces()[0];

        List<Vertex> selected = new() { vert1, vert2 };

        mesh.Extrude(selected);
        mesh.MoveSelectionRelative(selected, float3(-1f, 0f, 0f));

        mesh.OptimizeIndices();
        mesh.RecalculateNormals();

        Assert.AreEqual(6, mesh.VertexCount);
        Assert.AreEqual(7, mesh.EdgeCount);
        Assert.AreEqual(2, mesh.FaceCount);

        Assert.AreEqual(2, vert1.EdgeCount);
        Assert.AreEqual(1, vert1.FaceCount);
        Assert.AreEqual(2, vert2.EdgeCount);
        Assert.AreEqual(1, vert2.FaceCount);

        Assert.IsFalse(face.ContainsVertex(vert1));
        Assert.IsFalse(vert1.IsConnected(face));
        Assert.IsFalse(face.ContainsVertex(vert2));
        Assert.IsFalse(vert2.IsConnected(face));

        Vertex createdVert1 = mesh.FindByPosition(float3(0f, 1f, 0f));
        Vertex createdVert2 = mesh.FindByPosition(float3(0f, 0f, 0f));

        Assert.AreEqual(3, createdVert1.EdgeCount);
        Assert.AreEqual(2, createdVert1.FaceCount);
        Assert.AreEqual(3, createdVert2.EdgeCount);
        Assert.AreEqual(2, createdVert2.FaceCount);

        Assert.IsTrue(face.ContainsVertex(createdVert1));
        Assert.IsTrue(createdVert1.IsConnected(face));
        Assert.IsTrue(face.ContainsVertex(createdVert2));
        Assert.IsTrue(createdVert2.IsConnected(face));
    }

    [Test]
    public void TestExtrudeTwoEdges()
    {
        UMesh mesh = Creators.Quad;
        Vertex vert1 = mesh.FindByPosition(float3(0f, 1f, 0f));
        Face face = vert1.GetFaces()[0];

        var selected = vert1.GetConnectedVertices();
        Assert.AreEqual(2, selected.Count);
        selected.Add(vert1);

        mesh.Extrude(selected);
        mesh.MoveSelectionRelative(selected, float3(-0.5f, 0.5f, 0f));

        mesh.OptimizeIndices();
        mesh.RecalculateNormals();

        Assert.AreEqual(7, mesh.VertexCount);
        Assert.AreEqual(9, mesh.EdgeCount);
        Assert.AreEqual(3, vert1.EdgeCount);
        Assert.AreEqual(2, vert1.FaceCount);
        Assert.AreEqual(3, mesh.FaceCount);
        Assert.IsFalse(face.ContainsVertex(vert1));
        Assert.IsFalse(vert1.IsConnected(face));

        Vertex created1 = mesh.FindByPosition(float3(0f, 0f, 0f));
        Vertex created2 = mesh.FindByPosition(float3(0f, 1f, 0f));
        Vertex created3 = mesh.FindByPosition(float3(1f, 1f, 0f));

        Assert.AreEqual(3, created1.EdgeCount);
        Assert.AreEqual(2, created1.FaceCount);
        Assert.IsTrue(created1.IsConnected(face));
        Assert.IsFalse(created1.IsConnected(vert1));

        Assert.AreEqual(3, created2.EdgeCount);
        Assert.AreEqual(3, created2.FaceCount);
        Assert.IsTrue(created2.IsConnected(face));
        Assert.IsTrue(created2.IsConnected(vert1));

        Assert.AreEqual(3, created3.EdgeCount);
        Assert.AreEqual(2, created3.FaceCount);
        Assert.IsTrue(created3.IsConnected(face));
        Assert.IsFalse(created3.IsConnected(vert1));
    }

    [Test]
    public void TestExtrudeTwoEdges2()
    {
        UMesh mesh = Creators.Quad;
        Vertex vert1 = mesh.FindByPosition(float3(0f, 1f, 0f));
        Face face = vert1.GetFaces()[0];

        var selected = vert1.Edges;
        Assert.AreEqual(2, selected.Count);

        mesh.Extrude(selected);
        mesh.MoveSelectionRelative(selected, float3(-0.5f, 0.5f, 0f));

        mesh.OptimizeIndices();
        mesh.RecalculateNormals();

        Assert.AreEqual(7, mesh.VertexCount);
        Assert.AreEqual(9, mesh.EdgeCount);
        Assert.AreEqual(3, vert1.EdgeCount);
        Assert.AreEqual(2, vert1.FaceCount);
        Assert.AreEqual(3, mesh.FaceCount);
        Assert.IsFalse(face.ContainsVertex(vert1));
        Assert.IsFalse(vert1.IsConnected(face));

        Vertex created1 = mesh.FindByPosition(float3(0f, 0f, 0f));
        Vertex created2 = mesh.FindByPosition(float3(0f, 1f, 0f));
        Vertex created3 = mesh.FindByPosition(float3(1f, 1f, 0f));

        Assert.AreEqual(3, created1.EdgeCount);
        Assert.AreEqual(2, created1.FaceCount);
        Assert.IsTrue(created1.IsConnected(face));
        Assert.IsFalse(created1.IsConnected(vert1));

        Assert.AreEqual(3, created2.EdgeCount);
        Assert.AreEqual(3, created2.FaceCount);
        Assert.IsTrue(created2.IsConnected(face));
        Assert.IsTrue(created2.IsConnected(vert1));

        Assert.AreEqual(3, created3.EdgeCount);
        Assert.AreEqual(2, created3.FaceCount);
        Assert.IsTrue(created3.IsConnected(face));
        Assert.IsFalse(created3.IsConnected(vert1));
    }

    [Test]
    public void TestExtrudeOneFace()
    {
        UMesh mesh = Creators.Quad;
        Face face = mesh.Faces[0];
        var verts = face.Vertices;
        Vertex vert1 = verts[0];
        Vertex vert2 = verts[1];
        Vertex vert3 = verts[2];
        Vertex vert4 = verts[3];

        mesh.Extrude(face);
        face.MoveRelative(float3(0f, 0f, 1f));

        mesh.OptimizeIndices();
        mesh.RecalculateNormals();

        Assert.AreEqual(8, mesh.VertexCount);
        Assert.AreEqual(12, mesh.EdgeCount);
        Assert.AreEqual(5, mesh.FaceCount);

        // stability tests
        Assert.AreEqual(3, vert1.EdgeCount);
        Assert.AreEqual(3, vert1.FaceCount);
        Assert.IsTrue(face.ContainsVertex(vert1));
        Assert.IsTrue(vert1.IsConnected(face));

        Assert.AreEqual(3, vert2.EdgeCount);
        Assert.AreEqual(3, vert2.FaceCount);
        Assert.IsTrue(face.ContainsVertex(vert2));
        Assert.IsTrue(vert2.IsConnected(face));

        Assert.AreEqual(3, vert3.EdgeCount);
        Assert.AreEqual(3, vert3.FaceCount);
        Assert.IsTrue(face.ContainsVertex(vert3));
        Assert.IsTrue(vert3.IsConnected(face));

        Assert.AreEqual(3, vert4.EdgeCount);
        Assert.AreEqual(3, vert4.FaceCount);
        Assert.IsTrue(face.ContainsVertex(vert4));
        Assert.IsTrue(vert4.IsConnected(face));

        HashSet<Vertex> created = new HashSet<Vertex>(mesh.Vertices);
        created.ExceptWith(verts);

        foreach (Vertex newVert in created)
        {
            Assert.AreEqual(3, newVert.EdgeCount);
            Assert.AreEqual(2, newVert.FaceCount);
            Assert.IsFalse(face.ContainsVertex(newVert));
            Assert.IsFalse(newVert.IsConnected(face));
        }
    }

    [Test]
    public void TestExtrudeOneFace2()
    {
        UMesh mesh = Creators.Quad;
        Face face = mesh.Faces[0];
        var verts = face.Vertices;
        Vertex vert1 = verts[0];
        Vertex vert2 = verts[1];
        Vertex vert3 = verts[2];
        Vertex vert4 = verts[3];

        mesh.Extrude(verts);
        mesh.MoveSelectionRelative(verts, float3(0f, 0f, 1f));

        mesh.OptimizeIndices();
        mesh.RecalculateNormals();

        Assert.AreEqual(8, mesh.VertexCount);
        Assert.AreEqual(12, mesh.EdgeCount);
        Assert.AreEqual(5, mesh.FaceCount);

        // stability tests
        Assert.AreEqual(3, vert1.EdgeCount);
        Assert.AreEqual(3, vert1.FaceCount);
        Assert.IsTrue(face.ContainsVertex(vert1));
        Assert.IsTrue(vert1.IsConnected(face));

        Assert.AreEqual(3, vert2.EdgeCount);
        Assert.AreEqual(3, vert2.FaceCount);
        Assert.IsTrue(face.ContainsVertex(vert2));
        Assert.IsTrue(vert2.IsConnected(face));

        Assert.AreEqual(3, vert3.EdgeCount);
        Assert.AreEqual(3, vert3.FaceCount);
        Assert.IsTrue(face.ContainsVertex(vert3));
        Assert.IsTrue(vert3.IsConnected(face));

        Assert.AreEqual(3, vert4.EdgeCount);
        Assert.AreEqual(3, vert4.FaceCount);
        Assert.IsTrue(face.ContainsVertex(vert4));
        Assert.IsTrue(vert4.IsConnected(face));

        HashSet<Vertex> created = new HashSet<Vertex>(mesh.Vertices);
        created.ExceptWith(verts);

        foreach (Vertex newVert in created)
        {
            Assert.AreEqual(3, newVert.EdgeCount);
            Assert.AreEqual(2, newVert.FaceCount);
            Assert.IsFalse(face.ContainsVertex(newVert));
            Assert.IsFalse(newVert.IsConnected(face));
        }
    }

    [Test]
    public void TestExtrudeOneFace3()
    {
        UMesh mesh = Creators.Quad;
        Face face = mesh.Faces[0];
        var verts = face.Vertices;
        Vertex vert1 = verts[0];
        Vertex vert2 = verts[1];
        Vertex vert3 = verts[2];
        Vertex vert4 = verts[3];
        var edges = face.Edges;

        mesh.Extrude(edges);
        mesh.MoveSelectionRelative(edges, float3(0f, 0f, 1f));

        mesh.OptimizeIndices();
        mesh.RecalculateNormals();

        Assert.AreEqual(8, mesh.VertexCount);
        Assert.AreEqual(12, mesh.EdgeCount);
        Assert.AreEqual(5, mesh.FaceCount);

        // stability tests
        Assert.AreEqual(3, vert1.EdgeCount);
        Assert.AreEqual(3, vert1.FaceCount);
        Assert.IsTrue(face.ContainsVertex(vert1));
        Assert.IsTrue(vert1.IsConnected(face));

        Assert.AreEqual(3, vert2.EdgeCount);
        Assert.AreEqual(3, vert2.FaceCount);
        Assert.IsTrue(face.ContainsVertex(vert2));
        Assert.IsTrue(vert2.IsConnected(face));

        Assert.AreEqual(3, vert3.EdgeCount);
        Assert.AreEqual(3, vert3.FaceCount);
        Assert.IsTrue(face.ContainsVertex(vert3));
        Assert.IsTrue(vert3.IsConnected(face));

        Assert.AreEqual(3, vert4.EdgeCount);
        Assert.AreEqual(3, vert4.FaceCount);
        Assert.IsTrue(face.ContainsVertex(vert4));
        Assert.IsTrue(vert4.IsConnected(face));

        HashSet<Vertex> created = new HashSet<Vertex>(mesh.Vertices);
        created.ExceptWith(verts);

        foreach (Vertex newVert in created)
        {
            Assert.AreEqual(3, newVert.EdgeCount);
            Assert.AreEqual(2, newVert.FaceCount);
            Assert.IsFalse(face.ContainsVertex(newVert));
            Assert.IsFalse(newVert.IsConnected(face));
        }
    }

    /// <summary>
    /// There is a shared edge that must not be extruded on this test
    /// </summary>
    [Test]
    public void TestExtrudeTwoFaces()
    {
        UMesh mesh = Creators.Quad;
        Vertex vert1 = mesh.FindByPosition(float3(0f, 0f, 0f));
        Vertex vert2 = mesh.FindByPosition(float3(0f, 1f, 0f));
        Edge edge = vert1.GetEdgeTo(vert2);
        mesh.Extrude(edge);
        edge.MoveRelative(float3(-1f, 0f, 0f));

        var verts = new List<Vertex>(mesh.Vertices);

        mesh.Extrude(verts);
        mesh.MoveSelectionRelative(verts, float3(0f, 0f, 1f));

        mesh.OptimizeIndices();
        mesh.RecalculateNormals();

        Assert.AreEqual(12, mesh.VertexCount);
        Assert.AreEqual(19, mesh.EdgeCount);
        Assert.AreEqual(8, mesh.FaceCount);

        Vertex keyVertex1 = mesh.FindByPosition(float3(0f, 0f, 1f));
        Vertex keyVertex2 = mesh.FindByPosition(float3(0f, 1f, 1f));

        Assert.AreEqual(4, keyVertex1.EdgeCount);
        Assert.AreEqual(4, keyVertex1.FaceCount);
        Assert.AreEqual(4, keyVertex2.EdgeCount);
        Assert.AreEqual(4, keyVertex2.FaceCount);

        Vertex keyVertex3 = mesh.FindByPosition(float3(0f, 0f, 0f));
        Vertex keyVertex4 = mesh.FindByPosition(float3(0f, 1f, 0f));

        Assert.AreEqual(3, keyVertex3.EdgeCount);
        Assert.AreEqual(2, keyVertex3.FaceCount);
        Assert.AreEqual(3, keyVertex4.EdgeCount);
        Assert.AreEqual(2, keyVertex4.FaceCount);
        Assert.IsFalse(keyVertex3.IsConnected(keyVertex4));
    }

    /// <summary>
    /// This one has an internal vertex that must not be extruded
    /// </summary>
    [Test]
    public void TestExtrudeFourFaces()
    {
        UMesh mesh = Creators.Empty;
        // .
        Vertex vertex1 = mesh.CreateVertex(float3(-1f, 1f, 0f));

        Vertex vertex2 = mesh.CreateVertexConnectedTo(vertex1, out Edge edge1);
        vertex2.Position = float3(0f, 1f, 0f);
        // . - .
        Vertex vertex3 = mesh.CreateVertexConnectedTo(vertex2, out Edge edge2);
        vertex3.Position = float3(1f, 1f, 0f);
        // . - . - .

        List<Edge> edges = new() { edge1, edge2 };
        mesh.Extrude(edges);
        mesh.MoveSelectionRelative(edges, float3(0f, -1f, 0f));
        // . - . - .
        // |   |   |
        // . - . - .
        mesh.Extrude(edges);
        mesh.MoveSelectionRelative(edges, float3(0f, -1f, 0f));
        // . - . - .
        // |   |   |
        // . - . - .
        // |   |   |
        // . - . - .

        List<Vertex> allVertices = new(mesh.Vertices);
        mesh.Extrude(allVertices);
        mesh.MoveSelectionRelative(allVertices, float3(0f, 0f, 1f));

        mesh.OptimizeIndices();
        mesh.RecalculateNormals();

        Assert.AreEqual(17, mesh.VertexCount);
        Assert.AreEqual(28, mesh.EdgeCount);
        Assert.AreEqual(12, mesh.FaceCount);
    }

    /// <summary>
    /// Sometimes you just gotta do some random things and see if any errors are thrown
    /// </summary>
    [Test]
    public void TestExtrudeStochastic2()
    {
        System.Random rng = new(0);
        List<Vertex> verts = new();
        for (int i = 0; i < 5; i++)
        {
            UMesh mesh = Creators.Cube;
            for (int j = 0; j < 5; j++)
            {
                verts = new(mesh.Vertices);
                mesh.Extrude(verts.OrderBy(index => rng.Next()).Take(5).ToList());
            }
        }
    }


    /// <summary>
    /// Sometimes you just gotta do some random things and see if any errors are thrown
    /// </summary>
    [Test]
    public void TestExtrudeStochastic1()
    {

        for (int i = 0; i < 8; i++)
        {
            UMesh mesh = Creators.Cube;
            List<Vertex> verts = new();
            verts.Add(mesh.Vertices[i % mesh.Vertices.Count]);
            verts.Add(mesh.Vertices[(i+1) % mesh.Vertices.Count]);
            verts.Add(mesh.Vertices[(i+4) % mesh.Vertices.Count]);
            verts.Add(mesh.Vertices[(i+5) % mesh.Vertices.Count]);
            mesh.Extrude(verts);
        }
        for (int i = 0; i < 8; i++)
        {
            UMesh mesh = Creators.Cube;
            List<Vertex> verts = new();
            verts.Add(mesh.Vertices[i % mesh.Vertices.Count]);
            verts.Add(mesh.Vertices[(i + 1) % mesh.Vertices.Count]);
            verts.Add(mesh.Vertices[(i + 2) % mesh.Vertices.Count]);
            verts.Add(mesh.Vertices[(i + 3) % mesh.Vertices.Count]);
            mesh.Extrude(verts);
        }
        for (int i = 0; i < 8; i++)
        {
            UMesh mesh = Creators.Cube;
            List<Vertex> verts = new();
            verts.Add(mesh.Vertices[i % mesh.Vertices.Count]);
            verts.Add(mesh.Vertices[(i + 2) % mesh.Vertices.Count]);
            verts.Add(mesh.Vertices[(i + 5) % mesh.Vertices.Count]);
            verts.Add(mesh.Vertices[(i + 7) % mesh.Vertices.Count]);
            mesh.Extrude(verts);
        }
    }

    [Test]
    public void TestExtrudeHole()
    {
        UMesh mesh = Creators.Cube;
        Face face = mesh.Faces.First();
        List<Vertex> verts = face.Vertices;

        mesh.DeleteGeometry(face);
        mesh.Extrude(verts);

        Assert.AreEqual(12, mesh.VertexCount);
        Assert.AreEqual(20, mesh.EdgeCount);
        Assert.AreEqual(9, mesh.FaceCount);

    }

    [Test]
    public void TestExtrudeAndDeleteAndExtrude()
    {
        UMesh mesh = Creators.Cube;

        List<Vertex> verts = new(); // = mesh.Faces.First().Vertices;
        float3 pos0 = float3(-1f, -1f, -1f);
        float3 pos1 = float3(-1f, -1f, 1f);
        float3 pos2 = float3(-1f, 1f, -1f);
        float3 pos3 = float3(-1f, 1f, 1f);

        Vertex ex0 = mesh.FindByPosition(pos0);
        Vertex ex1 = mesh.FindByPosition(pos1);
        Vertex ex2 = mesh.FindByPosition(pos2);
        Vertex ex3 = mesh.FindByPosition(pos3);
        verts.Add(ex0);
        verts.Add(ex1);
        verts.Add(ex2);
        verts.Add(ex3);

        mesh.Extrude(verts);
        mesh.MoveSelectionRelative(verts, float3(-1f, 0f, 0f));

        Assert.AreEqual(12, mesh.VertexCount);
        Assert.AreEqual(20, mesh.EdgeCount);
        Assert.AreEqual(10, mesh.FaceCount);

        mesh.DeleteGeometry(verts);
        verts.Clear();

        Assert.AreEqual(8, mesh.VertexCount);
        Assert.AreEqual(12, mesh.EdgeCount);
        Assert.AreEqual(5, mesh.FaceCount);

        Vertex ex0new = mesh.FindByPosition(pos0);
        Vertex ex1new = mesh.FindByPosition(pos1);
        Vertex ex2new = mesh.FindByPosition(pos2);
        Vertex ex3new = mesh.FindByPosition(pos3);

        Assert.AreEqual(2, ex0new.FaceCount);
        Assert.AreEqual(2, ex1new.FaceCount);
        Assert.AreEqual(2, ex2new.FaceCount);
        Assert.AreEqual(2, ex3new.FaceCount);

        verts.Add(ex0new);
        verts.Add(ex1new);
        verts.Add(ex2new);
        verts.Add(ex3new);

        Assert.IsFalse(ex0.IsConnected(ex0new));
        Assert.IsFalse(ex0new.IsConnected(ex0));
        Assert.IsFalse(ex1.IsConnected(ex1new));
        Assert.IsFalse(ex1new.IsConnected(ex1));
        Assert.IsFalse(ex2.IsConnected(ex2new));
        Assert.IsFalse(ex2new.IsConnected(ex2));
        Assert.IsFalse(ex3.IsConnected(ex3new));
        Assert.IsFalse(ex3new.IsConnected(ex3));

        mesh.Extrude(verts);
        mesh.MoveSelectionRelative(verts, float3(-1f, 0f, 0f));

        Assert.AreEqual(12, mesh.VertexCount);
        Assert.AreEqual(20, mesh.EdgeCount);
        Assert.AreEqual(9, mesh.FaceCount);
    }

}