using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

public class MRUKMeshExporter : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ExportAnchorsAsMeshes());
    }

    IEnumerator ExportAnchorsAsMeshes()
    {
        yield return new WaitForSeconds(2f); // Wait for scene anchors to initialize

        // Try to find all objects tagged or named as scene anchors
        var anchors = GameObject.FindObjectsOfType<Transform>();
        int count = 0;

        foreach (var anchor in anchors)
        {
            if (!anchor.name.StartsWith("MRUKSceneAnchor")) continue; // Filter relevant anchors

            MeshFilter mf = anchor.GetComponentInChildren<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                ExportMeshToOBJ(mf, $"MRUKMesh_{anchor.name}_{count}.obj");
                count++;
            }
        }

        Debug.Log($"[MRUKExporter] Exported {count} scene mesh(es) to storage.");
    }

    void ExportMeshToOBJ(MeshFilter mf, string filename)
    {
        Mesh mesh = mf.sharedMesh;
        var sb = new StringBuilder();

        sb.AppendLine("# MRUK Scene Mesh Export");

        foreach (Vector3 v in mesh.vertices)
        {
            Vector3 wv = mf.transform.TransformPoint(v);
            sb.AppendLine($"v {wv.x} {wv.y} {wv.z}");
        }

        foreach (Vector3 n in mesh.normals)
        {
            Vector3 wn = mf.transform.TransformDirection(n);
            sb.AppendLine($"vn {wn.x} {wn.y} {wn.z}");
        }

        foreach (Vector2 uv in mesh.uv)
        {
            sb.AppendLine($"vt {uv.x} {uv.y}");
        }

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            int[] tris = mesh.GetTriangles(i);
            for (int j = 0; j < tris.Length; j += 3)
            {
                sb.AppendLine($"f {tris[j] + 1}/{tris[j] + 1}/{tris[j] + 1} " +
                              $"{tris[j + 1] + 1}/{tris[j + 1] + 1}/{tris[j + 1] + 1} " +
                              $"{tris[j + 2] + 1}/{tris[j + 2] + 1}/{tris[j + 2] + 1}");
            }
        }

        string path = Path.Combine(Application.persistentDataPath, filename);
        File.WriteAllText(path, sb.ToString());
        Debug.Log($"Saved {filename} to: {path}");
    }
}
