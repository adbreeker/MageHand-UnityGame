using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    bool _isSkinned;
    Renderer _renderer;
    Mesh _originalMesh;

    private void Awake()
    {
        if ((_renderer = GetComponent<SkinnedMeshRenderer>()) != null) 
        { 
            _isSkinned = true;
            _originalMesh = ((SkinnedMeshRenderer)_renderer).sharedMesh;
        }
        else if ((_renderer = GetComponent<MeshRenderer>()) != null)
        { 
            _isSkinned = false; 
            _originalMesh = GetComponent<MeshFilter>().sharedMesh;
        }
        else
        {
            Debug.Log("Destroyable object have no renderer");
            this.enabled = false;
        }
    }

    public void Destroy()
    {
        Mesh[] splitMeshes = SplitMesh(_originalMesh);
        splitMeshes = CombineAdjacentMeshes(splitMeshes, 3);
        ReplaceOriginalWithFragments(splitMeshes);
    }

    Mesh[] SplitMesh(Mesh originalMesh)
    {
        // Get the vertices, triangles, and other data from the original mesh
        Vector3[] vertices = originalMesh.vertices;
        int[] triangles = originalMesh.triangles;
        Vector3[] normals = originalMesh.normals;
        Vector2[] uv = originalMesh.uv;

        // Create an array to hold the split meshes
        Mesh[] splitMeshes = new Mesh[triangles.Length / 3];

        // Split the mesh into smaller meshes
        for (int i = 0; i < splitMeshes.Length; i++)
        {
            splitMeshes[i] = new Mesh();
            splitMeshes[i].name = "SplitMesh_" + i;

            int triangleIndex = i * 3;

            // Assign vertices, triangles, normals, and UVs to the split mesh
            splitMeshes[i].vertices = new Vector3[]
            {
                vertices[triangles[triangleIndex]],
                vertices[triangles[triangleIndex + 1]],
                vertices[triangles[triangleIndex + 2]]
            };

            splitMeshes[i].triangles = new int[] { 0, 1, 2 };
            splitMeshes[i].normals = new Vector3[] { normals[triangles[triangleIndex]], normals[triangles[triangleIndex + 1]], normals[triangles[triangleIndex + 2]] };
            splitMeshes[i].uv = new Vector2[] { uv[triangles[triangleIndex]], uv[triangles[triangleIndex + 1]], uv[triangles[triangleIndex + 2]] };

            // You can perform any additional modifications to the split mesh here (e.g., scaling, rotating, etc.)

            splitMeshes[i].RecalculateBounds();
        }
        return splitMeshes;
    }



    Mesh[] CombineAdjacentMeshes(Mesh[] splitMeshes, int groupSize)
    {
        List<Mesh> combinedMeshes = new List<Mesh>();
        bool[] used = new bool[splitMeshes.Length];

        for (int i = 0; i < splitMeshes.Length; i++)
        {
            if (used[i])
                continue;

            List<Vector3> vertices = new List<Vector3>(splitMeshes[i].vertices);
            List<int> triangles = new List<int>(splitMeshes[i].triangles);
            List<Vector3> normals = new List<Vector3>(splitMeshes[i].normals);
            List<Vector2> uv = new List<Vector2>(splitMeshes[i].uv);

            used[i] = true;
            int combinedCount = 1;

            for (int j = i + 1; j < splitMeshes.Length && combinedCount < groupSize; j++)
            {
                if (used[j])
                    continue;

                // Check if splitMeshes[j] shares any vertices with the current combined mesh
                bool areAdjacent = false;
                for (int k = 0; k < splitMeshes[j].vertices.Length; k++)
                {
                    if (vertices.Contains(splitMeshes[j].vertices[k]))
                    {
                        areAdjacent = true;
                        break;
                    }
                }

                if (areAdjacent)
                {
                    int offset = vertices.Count;

                    vertices.AddRange(splitMeshes[j].vertices);
                    for (int t = 0; t < splitMeshes[j].triangles.Length; t++)
                    {
                        triangles.Add(splitMeshes[j].triangles[t] + offset);
                    }

                    normals.AddRange(splitMeshes[j].normals);
                    uv.AddRange(splitMeshes[j].uv);

                    used[j] = true;
                    combinedCount++;
                }
            }

            Mesh combinedMesh = new Mesh
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                normals = normals.ToArray(),
                uv = uv.ToArray()
            };

            combinedMesh.RecalculateBounds();
            combinedMeshes.Add(combinedMesh);
        }

        return combinedMeshes.ToArray();
    }

    void ReplaceOriginalWithFragments(Mesh[] originalFragments)
    {
        Vector3 replacePosition;
        Quaternion replaceRotation;
        Vector3 replaceScale;
        if (_isSkinned) 
        { 
            replacePosition = ((SkinnedMeshRenderer)_renderer).rootBone.position;
            replaceRotation = ((SkinnedMeshRenderer)_renderer).rootBone.rotation;
            replaceScale = ((SkinnedMeshRenderer)_renderer).rootBone.lossyScale;
        }
        else 
        { 
            replacePosition = transform.position; 
            replaceRotation = transform.rotation;
            replaceScale = transform.localScale;
        }

        Material[] materials = _renderer.materials;
        for (int i = 0; i < originalFragments.Length; i++)
        {
            GameObject splitObject = new GameObject("SplitObject_" + i);
            MeshFilter splitMeshFilter = splitObject.AddComponent<MeshFilter>();
            MeshRenderer splitMeshRenderer = splitObject.AddComponent<MeshRenderer>();
            splitMeshFilter.mesh = originalFragments[i];
            // Set appropriate materials, shaders, etc., for the split objects

            splitMeshRenderer.materials = materials;
            // You can position and parent the split objects however you like
            splitObject.transform.position = replacePosition;
            splitObject.transform.rotation = replaceRotation;
            splitObject.transform.localScale = replaceScale;

            splitObject.AddComponent<BoxCollider>();
            splitObject.AddComponent<Rigidbody>();
            splitObject.AddComponent<VanishDestroyed>().Initialize();
        }

        // Destroy the original mesh if desired
        Destroy(gameObject);
    }

    Mesh JoinMeshes(Mesh mesh1, Mesh mesh2)
    {
        // Combine vertices, triangles, normals, and UVs
        Vector3[] combinedVertices = CombineArrays(mesh1.vertices, mesh2.vertices);
        int[] combinedTriangles = CombineArrays(mesh1.triangles, mesh2.triangles);
        Vector3[] combinedNormals = CombineArrays(mesh1.normals, mesh2.normals);
        Vector2[] combinedUVs = CombineArrays(mesh1.uv, mesh2.uv);

        // Create a new mesh and assign the combined data
        Mesh combinedMesh = new Mesh()
        {
            name = "CombinedMesh",
            vertices = combinedVertices,
            triangles = combinedTriangles,
            normals = combinedNormals,
            uv = combinedUVs
        };

        combinedMesh.RecalculateBounds();

        // Assign the combined mesh to the MeshFilter component
        mesh1 = combinedMesh;
        Destroy(mesh2);
        // Optionally, you can destroy the second mesh if desired
        return mesh1;
    }

    private T[] CombineArrays<T>(T[] array1, T[] array2)
    {
        T[] combinedArray = new T[array1.Length + array2.Length];
        array1.CopyTo(combinedArray, 0);
        array2.CopyTo(combinedArray, array1.Length);
        return combinedArray;
    }

#if UNITY_EDITOR
    private void Reset()
    {
        gameObject.isStatic = false;

        ModelImporter modelImporter;
        if (GetComponent<MeshFilter>() != null)
        {
            modelImporter = (ModelImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(gameObject.GetComponent<MeshFilter>().sharedMesh));
        }
        else
        {
            modelImporter = (ModelImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh));
        }

        modelImporter.isReadable = true;
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(Destroyable))]
public class DestroyableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Destroyable script = (Destroyable)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Reimport destroyable mesh"))
        {
            ModelImporter modelImporter;
            if(script.gameObject.GetComponent<MeshFilter>() != null)
            {
                modelImporter = (ModelImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(script.gameObject.GetComponent<MeshFilter>().sharedMesh));
            }
            else
            {
               modelImporter = (ModelImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(script.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh));
            }
            modelImporter.SaveAndReimport();
        }
    }
}
#endif

public class VanishDestroyed : MonoBehaviour
{
    public void Initialize()
    {
        StartCoroutine(Vanish());
    }
    IEnumerator Vanish()
    {
        yield return new WaitForSeconds(Random.Range(1.5f, 3f));
        Destroy(gameObject);
    }
}
