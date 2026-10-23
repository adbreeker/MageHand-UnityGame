using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Destroyable : MagicImpactTarget
{
    bool _isSkinned;
    Renderer _renderer;
    Mesh _originalMesh;

    [Header("Optimizing:")]

    [Header("Grouping splited meshes")]
    [Tooltip("Maximum of splited meshes in every merged group")]
    [Min(1)]
    public int groupingBy = 1;

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

    public override void ImpactInteraction(GameObject impactingSpell)
    {
        if(impactingSpell.GetComponent<FireSpellBehavior>() != null)
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        int[] triangles = _originalMesh.triangles;
        int triangleCount = triangles.Length / 3;
        int groupSizes = groupingBy;

        if(GameSettings.graphicsQuality == GameSettings.GraphicsQuality.High)
        {
            groupSizes = Mathf.Max(1, groupingBy, Mathf.RoundToInt(triangleCount / 1000.0f));
        }
        else if (GameSettings.graphicsQuality == GameSettings.GraphicsQuality.Medium)
        {
            groupSizes = Mathf.Max(2, groupingBy, Mathf.RoundToInt(triangleCount / 500.0f));
        }
        else if (GameSettings.graphicsQuality == GameSettings.GraphicsQuality.Low)
        {
            groupSizes = Mathf.Max(4, groupingBy, Mathf.RoundToInt(triangleCount / 250.0f));
        }
        Debug.Log("Destroyable group size: " + groupSizes);
        Mesh[] fragments = CreateFragmentMeshes(_originalMesh, triangles, groupSizes);
        ReplaceOriginalWithFragments(fragments);
    }

    Mesh[] CreateFragmentMeshes(Mesh originalMesh, int[] triangles, int groupSize)
    {
        Vector3[] vertices = originalMesh.vertices;
        Vector3[] normals = originalMesh.normals;
        Vector2[] uv = originalMesh.uv;
        List<Mesh> combinedMeshes = new List<Mesh>();
        bool[] used = new bool[triangles.Length / 3];

        for (int i = 0; i < used.Length; i++)
        {
            if (used[i])
                continue;

            List<Vector3> fragmentVertices = new List<Vector3>();
            List<int> fragmentTriangles = new List<int>();
            List<Vector3> fragmentNormals = new List<Vector3>();
            List<Vector2> fragmentUv = new List<Vector2>();
            HashSet<Vector3> adjacentVertices = new HashSet<Vector3>();
            used[i] = true;
            int combinedCount = 1;
            AddTriangleToFragment(i, vertices, triangles, normals, uv, fragmentVertices, fragmentTriangles, fragmentNormals, fragmentUv, adjacentVertices);

            for (int j = i + 1; j < used.Length && combinedCount < groupSize; j++)
            {
                if (used[j])
                    continue;

                int triangleIndex = j * 3;
                if (adjacentVertices.Contains(vertices[triangles[triangleIndex]]) ||
                    adjacentVertices.Contains(vertices[triangles[triangleIndex + 1]]) ||
                    adjacentVertices.Contains(vertices[triangles[triangleIndex + 2]]))
                {
                    used[j] = true;
                    combinedCount++;
                    AddTriangleToFragment(j, vertices, triangles, normals, uv, fragmentVertices, fragmentTriangles, fragmentNormals, fragmentUv, adjacentVertices);
                }
            }

            Mesh combinedMesh = new Mesh
            {
                name = "DestroyableFragment",
                vertices = fragmentVertices.ToArray(),
                triangles = fragmentTriangles.ToArray(),
                normals = fragmentNormals.ToArray(),
                uv = fragmentUv.ToArray()
            };

            combinedMesh.RecalculateBounds();
            combinedMeshes.Add(combinedMesh);
        }

        return combinedMeshes.ToArray();
    }

    void AddTriangleToFragment(int triangleNumber, Vector3[] sourceVertices, int[] sourceTriangles, Vector3[] sourceNormals, Vector2[] sourceUv, List<Vector3> vertices, List<int> triangles, List<Vector3> normals, List<Vector2> uv, HashSet<Vector3> adjacentVertices)
    {
        int sourceIndex = triangleNumber * 3;
        int vertexOffset = vertices.Count;

        for (int i = 0; i < 3; i++)
        {
            int vertexIndex = sourceTriangles[sourceIndex + i];
            Vector3 vertex = sourceVertices[vertexIndex];
            vertices.Add(vertex);
            triangles.Add(vertexOffset + i);
            normals.Add(sourceNormals[vertexIndex]);
            uv.Add(sourceUv[vertexIndex]);
            adjacentVertices.Add(vertex);
        }
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

        Material[] materials = _renderer.sharedMaterials;
        for (int i = 0; i < originalFragments.Length; i++)
        {
            GameObject splitObject = new GameObject("SplitObject_" + i);
            MeshFilter splitMeshFilter = splitObject.AddComponent<MeshFilter>();
            MeshRenderer splitMeshRenderer = splitObject.AddComponent<MeshRenderer>();
            splitMeshFilter.sharedMesh = originalFragments[i];
            // Set appropriate materials, shaders, etc., for the split objects

            splitMeshRenderer.sharedMaterials = materials;
            // You can position and parent the split objects however you like
            splitObject.transform.position = replacePosition;
            splitObject.transform.rotation = replaceRotation;
            splitObject.transform.localScale = replaceScale;

            splitObject.AddComponent<BoxCollider>();
            splitObject.AddComponent<Rigidbody>();
            float lifetime = Random.Range(1.5f, 3f);
            Destroy(originalFragments[i], lifetime);
            Destroy(splitObject, lifetime);
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
        modelImporter.SaveAndReimport();
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

        GUILayout.Space(20);

        if (GUILayout.Button("Reimport mesh"))
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
