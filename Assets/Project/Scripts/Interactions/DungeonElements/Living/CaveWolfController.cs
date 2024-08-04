using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CaveWolfController : MonoBehaviour
{

    Animation _animation;
    SkinnedMeshRenderer _meshRenderer;
    MeshCollider _meshCollider;

    private void Start()
    {
        _animation = GetComponentInChildren<Animation>();
        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _meshCollider = GetComponent<MeshCollider>();
    }

    private void Update()
    {
        UpdateCollider();
    }
    
    public void UpdateCollider()
    {
        Mesh colliderMesh = new Mesh();
        _meshRenderer.BakeMesh(colliderMesh);

        Vector3[] vertices = colliderMesh.vertices;
        Vector3 scale = new Vector3(1 / transform.localScale.x, 1 / transform.localScale.y, 1 / transform.localScale.z);
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].Scale(scale);
        }
        colliderMesh.vertices = vertices;

        // Recalculate bounds and normals after scaling
        colliderMesh.RecalculateBounds();
        colliderMesh.RecalculateNormals();

        _meshCollider.sharedMesh = null;
        _meshCollider.sharedMesh = colliderMesh;
    }
}
