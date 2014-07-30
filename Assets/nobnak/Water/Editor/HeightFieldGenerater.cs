using UnityEngine;
using System.Collections;
using UnityEditor;

public static class HeightFieldGenerater {
	public static int density = 128;

	[MenuItem("Custom/GenerateHeightField")]
	public static void Generate() {
		var n1 = density + 1;
		var vertices = new Vector3[n1 * n1];
		var normals = new Vector3[vertices.Length];
		var uv = new Vector2[vertices.Length];
		var triangles = new int[6 * density * density];

		var dx = 1f / density;
		var counter = 0;
		for (var y = 0; y < n1; y++) {
			for (var x = 0; x < n1; x++) {
				vertices[counter] = new Vector3(x * dx - 0.5f, 0f, y * dx - 0.5f);
				normals[counter] = Vector3.up;
				uv[counter] = new Vector2(x * dx, y * dx);
				counter++;
			}
		}

		counter = 0;
		for (var y = 0; y < density; y++) {
			for (var x = 0; x < density; x++) {
				var i = x + y * n1;
				triangles[counter++] = i;
				triangles[counter++] = i + n1 + 1;
				triangles[counter++] = i + 1;
				triangles[counter++] = i;
				triangles[counter++] = i + n1;
				triangles[counter++] = i + n1 + 1;
			}
		}

		var mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uv;
		mesh.triangles = triangles;
		mesh.RecalculateBounds();

		AssetDatabase.CreateAsset(mesh, "Assets/HeightField.asset");
	}
}