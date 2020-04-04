using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Text;
using System.IO;

public class ExportTool 
{
    private static string SceneName = "SampleScene";
    private static int StartIndex = 1;

    [MenuItem("Navmesh/Export")]
    public static void Export()
    {
        string objpath = Application.dataPath + "/scene.obj";
        Debug.LogError("path" + objpath);
        MeshFilter[] meshs = GetAllNavMeshs();
        ExportMeshsToObj(meshs, objpath);
    }

    static void ExportMeshsToObj(MeshFilter[] meshs, string objPath)
    {
        StreamWriter writer = new StreamWriter(objPath);

        for (int i = 0; i < meshs.Length; i++)
        {
            float progress = i * 1.0f / meshs.Length;
            EditorUtility.DisplayProgressBar("obj", "导出", progress);
            int numVertices = 0;
            GameObject obj = meshs[i].gameObject;
            Transform t = obj.transform;
            Mesh m = meshs[i].sharedMesh;

            writer.WriteLine("g " + obj.name);
            writer.WriteLine(MeshToString(meshs[i], t));
        }

        EditorUtility.DisplayProgressBar("obj", "正在写入", 1);
        writer.Close();
        EditorUtility.ClearProgressBar();
    }

    static string ConstructFaceString(int i1, int i2, int i3)
    {
        return "" + i1 + "/" + i2 + "/" + i3;
    }

    static Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle)
    {
        return angle * (point - pivot) + pivot;
    }
    static Vector3 MultiplyVec3(Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }
    static MeshFilter[] GetAllNavMeshs()
    {
        List<MeshFilter> meshFilters = new List<MeshFilter>();
        Scene scene = SceneManager.GetSceneByName(SceneName);
        if(scene != null)
        {
            GameObject[] objects = GetGameObjectsInScene(scene);
            for(int i = 0; i< objects.Length; i++)
            {
                MeshFilter meshfilter = objects[i].GetComponent<MeshFilter>();
                if (meshfilter!= null)
                {
                    meshFilters.Add(meshfilter);
                }

            }
        }
        return meshFilters.ToArray();
    }

    static GameObject[] GetGameObjectsInScene(Scene scene)
    {
        List<GameObject> result = new List<GameObject>();
        Stack<GameObject> objStack = new Stack<GameObject>();
        GameObject[] rootobjs = scene.GetRootGameObjects();
        foreach(var go in rootobjs)
        {
            objStack.Push(go);
            
        }

        while(objStack.Count > 0)
        {
            GameObject curobj = objStack.Pop();
            result.Add(curobj);
            for(int i = 0; i< curobj.transform.childCount; i++)
            {
                GameObject child = curobj.transform.GetChild(i).gameObject;
                if(child.activeInHierarchy)
                {
                    objStack.Push(child);
                }
            }
        }
        return result.ToArray();
    }

    static string MeshToString(MeshFilter mf, Transform t)
    {
        Vector3 s = t.localScale;
        Vector3 p = t.localPosition;
        Quaternion r = t.localRotation;


        int numVertices = 0;
        Mesh m = mf.sharedMesh;
        

        StringBuilder sb = new StringBuilder();

        foreach (Vector3 vv in m.vertices)
        {
            Vector3 v = t.TransformPoint(vv);
            numVertices++;
            sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, -v.z));
        }
        sb.Append("\n");
        foreach (Vector3 nn in m.normals)
        {
            Vector3 v = r * nn;
            sb.Append(string.Format("vn {0} {1} {2}\n", -v.x, -v.y, v.z));
        }
        sb.Append("\n");
        foreach (Vector3 v in m.uv)
        {
            sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        }
        for (int material = 0; material < m.subMeshCount; material++)
        {
            sb.Append("\n");
            int[] triangles = m.GetTriangles(material);
            for (int i = 0; i < triangles.Length; i += 3)
            {
                sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                    triangles[i] + 1 + StartIndex, triangles[i + 1] + 1 + StartIndex, triangles[i + 2] + 1 + StartIndex));
            }
        }

        StartIndex += numVertices;
        return sb.ToString();
    }

}
