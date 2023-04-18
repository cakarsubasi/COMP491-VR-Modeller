﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;
// get rid of this TODO
using System.Linq;

using static Unity.Mathematics.math;
#nullable enable

namespace Meshes
{
    partial struct EditableMeshImpl
    {

        public void SelectLoop(Vertex vertex, float startingAngle, float maximumAngle)
        {
            throw new NotImplementedException { };
        }

        public void SelectShortestPath(Vertex vertex1, Vertex vertex2)
        {
            throw new NotImplementedException { };
        }

        public void SelectFacesFromVertices(in List<Vertex> vertices, out List<Face> faces)
        {
            throw new NotImplementedException { };
        }

        public void SelectMore(Vertex vertex)
        {
            // much easier when keeping track of edges
            throw new NotImplementedException { };
        }

        public void SelectLess(ArrayList vertex)
        {
            // much easier when keeping track of edges
            throw new NotImplementedException { };
        }

        public void SelectSeam(Vertex vertex, float startingAngle, float maximumAngle)
        {
            // adding this would require also keeping track of edges
            throw new NotImplementedException { };
        }

        /// <summary>
        /// Find a Vertex in the given position and return the first one if it exists
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>The Vertex</returns>
        private Vertex? FindByPosition(float3 position)
        {
            foreach (Vertex vertex in Vertices)
            {
                if (vertex.Position.Equals(position))
                {
                    return vertex;
                }
            }
            return null;
        }

        /// <summary>
        /// Find all Vertices in a given position and return them in a list. May be useful while
        /// eliminating duplicates.
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>List of vertices</returns>
        private List<Vertex> FindAllByPosition(float3 position)
        {
            return Vertices.FindAll(vert => vert.Position.Equals(position)).ToList();
        }
    }
}