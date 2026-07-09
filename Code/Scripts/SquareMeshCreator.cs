using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquareMeshCreator : BaseFrameworkView
{
    [SerializeField] private MeshFilter meshFilter;

    [SerializeField] private Vector3[] meshVertices = new Vector3[]
    {
        new Vector3(-5, 0, -5),
        new Vector3(5, 0, -5),
        new Vector3(-5, 0, 5),
        new Vector3(5, 0, 5),
    };

    [SerializeField] private float scale;

    public override void LogCustom()
    {
        base.LogCustom();
        
        var mesh = new Mesh {name = "Procedural Grid"};

        var vertices = meshVertices.Select(x => x * scale).ToArray();
        
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];

        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, y = 0; y <= 1; y++)
        {
            for (int x = 0; x <= 1; x++, i++)
            {
                uv[i] = new Vector2((float) x / 1, (float) y / 1);
                tangents[i] = tangent;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;

        int[] triangles = new int[1 * 1 * 6];
        for (int ti = 0, vi = 0, y = 0; y < 1; y++, vi++)
        {
            for (int x = 0; x < 1; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + 1 + 1;
                triangles[ti + 5] = vi + 1 + 2;
            }
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }
}
