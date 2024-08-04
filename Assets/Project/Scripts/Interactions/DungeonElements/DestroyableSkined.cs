using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DestroyableSkined : MonoBehaviour
{
#if UNITY_EDITOR
    public void OnValidate()
    {
        gameObject.isStatic = false;
        ModelImporter modelImporter = (ModelImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh));
        modelImporter.isReadable = true;
    }
#endif

    public void SplitMesh()
    {
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer == null)
        {
            Debug.LogError("SkinnedMeshRenderer component not found!");
            return;
        }

        Mesh originalMesh = skinnedMeshRenderer.sharedMesh;

        // Get the vertices, triangles, and other data from the original mesh
        Vector3[] vertices = originalMesh.vertices;
        int[] triangles = originalMesh.triangles;
        Vector3[] normals = originalMesh.normals;
        Vector2[] uv = originalMesh.uv;
        BoneWeight[] boneWeights = originalMesh.boneWeights;
        Matrix4x4[] bindPoses = originalMesh.bindposes;
        Transform[] bones = skinnedMeshRenderer.bones;

        // Create an array to hold the split meshes
        Mesh[] splitMeshes = new Mesh[triangles.Length / 3];

        // Split the mesh into smaller meshes
        for (int i = 0; i < splitMeshes.Length; i++)
        {
            splitMeshes[i] = new Mesh();
            splitMeshes[i].name = "SplitMesh_" + i;

            int triangleIndex = i * 3;

            // Assign vertices, triangles, normals, UVs, and bone weights to the split mesh
            splitMeshes[i].vertices = new Vector3[]
            {
                vertices[triangles[triangleIndex]],
                vertices[triangles[triangleIndex + 1]],
                vertices[triangles[triangleIndex + 2]]
            };

            splitMeshes[i].triangles = new int[] { 0, 1, 2 };
            splitMeshes[i].normals = new Vector3[] { normals[triangles[triangleIndex]], normals[triangles[triangleIndex + 1]], normals[triangles[triangleIndex + 2]] };
            splitMeshes[i].uv = new Vector2[] { uv[triangles[triangleIndex]], uv[triangles[triangleIndex + 1]], uv[triangles[triangleIndex + 2]] };
            splitMeshes[i].boneWeights = new BoneWeight[] { boneWeights[triangles[triangleIndex]], boneWeights[triangles[triangleIndex + 1]], boneWeights[triangles[triangleIndex + 2]] };
            splitMeshes[i].bindposes = bindPoses;

            // You can perform any additional modifications to the split mesh here (e.g., scaling, rotating, etc.)

            splitMeshes[i].RecalculateBounds();
        }

        // Optionally, you can instantiate game objects with the split meshes
        for (int i = 0; i < splitMeshes.Length; i++)
        {
            GameObject splitObject = new GameObject("SplitObject_" + i);
            SkinnedMeshRenderer splitMeshRenderer = splitObject.AddComponent<SkinnedMeshRenderer>();
            splitMeshRenderer.sharedMesh = splitMeshes[i];
            splitMeshRenderer.bones = bones;
            splitMeshRenderer.materials = skinnedMeshRenderer.materials;

            // You can position and parent the split objects however you like
            splitObject.transform.position = transform.position;
            splitObject.transform.rotation = transform.rotation;
            splitObject.transform.localScale = transform.localScale;

            splitObject.AddComponent<BoxCollider>();
            splitObject.AddComponent<Rigidbody>();
            splitObject.AddComponent<VanishDestroyed>().Initialize();
        }

        // Destroy the original mesh if desired
        Destroy(gameObject);
    }

    public Mesh JoinMeshes(Mesh mesh1, Mesh mesh2)
    {
        // Combine vertices, triangles, normals, UVs, and bone weights
        Vector3[] combinedVertices = CombineArrays(mesh1.vertices, mesh2.vertices);
        int[] combinedTriangles = CombineArrays(mesh1.triangles, mesh2.triangles);
        Vector3[] combinedNormals = CombineArrays(mesh1.normals, mesh2.normals);
        Vector2[] combinedUVs = CombineArrays(mesh1.uv, mesh2.uv);
        BoneWeight[] combinedBoneWeights = CombineArrays(mesh1.boneWeights, mesh2.boneWeights);
        Matrix4x4[] combinedBindPoses = mesh1.bindposes; // Assuming the bind poses are the same for both meshes

        // Create a new mesh and assign the combined data
        Mesh combinedMesh = new Mesh();
        combinedMesh.name = "CombinedMesh";
        combinedMesh.vertices = combinedVertices;
        combinedMesh.triangles = combinedTriangles;
        combinedMesh.normals = combinedNormals;
        combinedMesh.uv = combinedUVs;
        combinedMesh.boneWeights = combinedBoneWeights;
        combinedMesh.bindposes = combinedBindPoses;
        combinedMesh.RecalculateBounds();

        // Assign the combined mesh to the SkinnedMeshRenderer component
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
}

#if UNITY_EDITOR
[CustomEditor(typeof(DestroyableSkined))]
public class DestroyableSkinedEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Destroyable script = (Destroyable)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Reimport destroyable mesh"))
        {
            ModelImporter modelImporter = (ModelImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(script.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh));
            modelImporter.SaveAndReimport();
        }
    }
}
#endif
