using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Meshes;
using MeshesIO;

public class WavefrontImportTestComponent : MonoBehaviour
{

    private void OnEnable()
    {
        SceneDescription description = WavefrontIO.Parse(TestString);

        for (int i = 0; i < description.objects.Count; i++)
        {
            UMesh mesh = description.objects[i];
            var transform = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            transform.GetComponent<MeshFilter>().mesh = mesh.Mesh;
            mesh.RecalculateAllAndWriteToMesh();
        }

    }

    public static string TestString = @"
# Blender v3.0.1 OBJ File: ''
# www.blender.org
mtllib test_export4.mtl
o Cube_Cube.001
v -1.000000 -1.000000 1.000000
v -1.000000 1.000000 1.000000
v -1.000000 -1.000000 -1.000000
v -1.000000 1.000000 -1.000000
v 1.000000 -1.000000 1.000000
v 1.000000 1.000000 1.000000
v 1.000000 -1.000000 -1.000000
v 1.000000 1.000000 -1.000000
vt 0.375000 0.000000
vt 0.625000 0.000000
vt 0.625000 0.250000
vt 0.375000 0.250000
vt 0.625000 0.500000
vt 0.375000 0.500000
vt 0.625000 0.750000
vt 0.375000 0.750000
vt 0.625000 1.000000
vt 0.375000 1.000000
vt 0.125000 0.500000
vt 0.125000 0.750000
vt 0.875000 0.500000
vt 0.875000 0.750000
vn -1.0000 0.0000 0.0000
vn 0.0000 0.0000 -1.0000
vn 1.0000 0.0000 0.0000
vn 0.0000 0.0000 1.0000
vn 0.0000 -1.0000 0.0000
vn 0.0000 1.0000 0.0000
usemtl None
s off
f 1/1/1 2/2/1 4/3/1 3/4/1
f 3/4/2 4/3/2 8/5/2 7/6/2
f 7/6/3 8/5/3 6/7/3 5/8/3
f 5/8/4 6/7/4 2/9/4 1/10/4
f 3/11/5 7/6/5 5/8/5 1/12/5
f 8/5/6 4/13/6 2/14/6 6/7/6
o Cube.001_Cube.002
v -2.511939 -1.534362 5.081775
v -2.511939 0.465638 5.081775
v -2.511939 -1.534362 3.081775
v -2.511939 0.465638 3.081775
v -0.511938 -1.534362 5.081775
v -0.511938 0.465638 5.081775
v -0.511938 -1.534362 3.081775
v -0.511938 0.465638 3.081775
vt 0.375000 0.000000
vt 0.625000 0.000000
vt 0.625000 0.250000
vt 0.375000 0.250000
vt 0.625000 0.500000
vt 0.375000 0.500000
vt 0.625000 0.750000
vt 0.375000 0.750000
vt 0.625000 1.000000
vt 0.375000 1.000000
vt 0.125000 0.500000
vt 0.125000 0.750000
vt 0.875000 0.500000
vt 0.875000 0.750000
vn -1.0000 0.0000 0.0000
vn 0.0000 0.0000 -1.0000
vn 1.0000 0.0000 0.0000
vn 0.0000 0.0000 1.0000
vn 0.0000 -1.0000 0.0000
vn 0.0000 1.0000 0.0000
usemtl None
s off
f 9/15/7 10/16/7 12/17/7 11/18/7
f 11/18/8 12/17/8 16/19/8 15/20/8
f 15/20/9 16/19/9 14/21/9 13/22/9
f 13/22/10 14/21/10 10/23/10 9/24/10
f 11/25/11 15/20/11 13/22/11 9/26/11
f 16/19/12 12/27/12 10/28/12 14/21/12
";
}
