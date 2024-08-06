using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CaveWolfController : MonoBehaviour
{

    Animation _animation;
    SkinnedMeshRenderer _meshRenderer;
    MeshCollider _meshCollider;

    public float movementSpeed = 10f;
    public float rotationSpeed = 360f;

    public LayerMask obstacleMask;

    Coroutine _movementCoroutine = null;

    private void Start()
    {
        _animation = GetComponentInChildren<Animation>();
        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _meshCollider = GetComponent<MeshCollider>();
    }

    private void FixedUpdate()
    {
        UpdateCollider();
    }

    public void SetWolfMovement(List<Transform> movementPath)
    {
        if(_movementCoroutine == null)
        {
            _movementCoroutine = StartCoroutine(MovementCoroutine(movementPath));
        }
        else
        {
            StopCoroutine(_movementCoroutine);
            _movementCoroutine = StartCoroutine(MovementCoroutine(movementPath));
        }
    }

    IEnumerator MovementCoroutine(List<Transform> movementPath)
    {
        _animation.CrossFade("run");
        foreach (Transform t in movementPath) 
        {
            Vector3 pathRelativePos = new Vector3(t.position.x, transform.position.y, t.position.z);

            Quaternion targetRotation = Quaternion.LookRotation(pathRelativePos - transform.position);
            while(transform.rotation != targetRotation)
            {
                yield return new WaitForFixedUpdate();
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            yield return new WaitForFixedUpdate();

            while(transform.position != pathRelativePos)
            {
                yield return new WaitForFixedUpdate();
                transform.position = Vector3.MoveTowards(transform.position, pathRelativePos, movementSpeed * Time.deltaTime);
            }
        }
        _animation.CrossFade("idle");
        _movementCoroutine = null;
    }


    bool CanMove(bool ghostmodeActive)
    {
        if (ghostmodeActive) //if ghostmode is active then allways can move
        {
            return true;
        }

        //get obstacles near player
        Collider[] colliders = Physics.OverlapSphere(new Vector3(transform.position.x, 1.25f, transform.position.z), 0.8f, obstacleMask);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "Wall" || collider.gameObject.tag == "Obstacle")
            {
                //if obstacle near player then can't move
                AudioSource collisionSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_Collision2);
                collisionSound.Play();
                Destroy(collisionSound.gameObject, collisionSound.clip.length);

                return false;
            }
        }
        return true;
    }

    void UpdateCollider()
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
