using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CaveWolfController : MonoBehaviour
{

    Animator _animator;
    SkinnedMeshRenderer _meshRenderer;
    MeshCollider _meshCollider;


    [Header("Movement:")]
    public float movementSpeed = 10f;
    public float rotationSpeed = 360f;

    [Header("Attack:")]
    public float attackRange;
    public float attackCooldown;
    bool _isAttackOnCooldown = false;

    [Header("Teleportation effect")]
    [SerializeField] GameObject _teleportationEffect;

    Coroutine _movementCoroutine = null;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _meshRenderer = GetComponent<SkinnedMeshRenderer>();
        _meshCollider = GetComponent<MeshCollider>();
    }

    private void FixedUpdate()
    {
        UpdateCollider();
        AttackPlayerInRange();
    }

    public void SetWolfMovement(List<Transform> movementPath, bool adjustMovementToPlayer = true, bool destroyOnLastTile = false)
    {
        if(_movementCoroutine == null)
        {
            _movementCoroutine = StartCoroutine(MovementCoroutine(movementPath, adjustMovementToPlayer, destroyOnLastTile));
        }
        else
        {
            StopCoroutine(_movementCoroutine);
            _movementCoroutine = StartCoroutine(MovementCoroutine(movementPath, adjustMovementToPlayer, destroyOnLastTile));
        }
    }

    IEnumerator MovementCoroutine(List<Transform> movementPath, bool adjustMovementToPlayer, bool destroyOnLastTile)
    {
        float ms = movementSpeed;
        float rs = rotationSpeed;

        _animator.SetBool("run", true);

        if (adjustMovementToPlayer)
        {
            if(PlayerParams.Controllers.playerMovement.movementSpeed > movementSpeed) 
            { 
                ms = PlayerParams.Controllers.playerMovement.movementSpeed + 0.1f * PlayerParams.Controllers.playerMovement.movementSpeed; 
            }

            if(PlayerParams.Controllers.playerMovement.rotationSpeed > rotationSpeed)
            {
                rs = PlayerParams.Controllers.playerMovement.rotationSpeed + 0.1f * PlayerParams.Controllers.playerMovement.rotationSpeed;
            }
        }

        foreach (Transform t in movementPath) 
        {
            Vector3 pathRelativePos = new Vector3(t.position.x, transform.position.y, t.position.z);

            Quaternion targetRotation;
            if (pathRelativePos - transform.position != Vector3.zero) { targetRotation = Quaternion.LookRotation(pathRelativePos - transform.position); }
            else { targetRotation = transform.rotation; }
            
            while(transform.rotation != targetRotation)
            {
                yield return new WaitForFixedUpdate();
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rs * Time.deltaTime);
            }

            yield return new WaitForFixedUpdate();

            while(transform.position != pathRelativePos)
            {
                yield return new WaitForFixedUpdate();
                transform.position = Vector3.MoveTowards(transform.position, pathRelativePos, ms * Time.deltaTime);
            }
        }

        if(destroyOnLastTile) { Destroy(gameObject); }

        _animator.SetBool("run", false);
        _movementCoroutine = null;
    }

    void AttackPlayerInRange()
    {
        if (!_isAttackOnCooldown)
        {
            Vector3 attackPoint = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
            Debug.DrawRay(attackPoint, transform.forward * attackRange, Color.yellow);

            if (Physics.Raycast(attackPoint, transform.forward, attackRange, LayerMask.GetMask("Player")))
            {
                _isAttackOnCooldown = true;
                _animator.SetTrigger("attack");
                StartCoroutine(Cooldown(v => _isAttackOnCooldown = v, attackCooldown));
            }
        }
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

    public void TeleportTo(Vector3 tpDestination, float tpRotation)
    {
        if (_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
            _movementCoroutine = null;
        }
        transform.position = tpDestination;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, tpRotation, transform.rotation.eulerAngles.z);
    }

    public void TeleportTo(Vector3 tpDestination, float tpRotation, Color? tpEffectColor)
    {
        if(_movementCoroutine != null)
        {
            StopCoroutine( _movementCoroutine );
            _movementCoroutine = null;
        }
        transform.position = tpDestination;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, tpRotation, transform.rotation.eulerAngles.z);

        AudioSource tpSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_MagicalTeleportation);
        tpSound.Play();
        Destroy(tpSound, tpSound.clip.length);

        if (tpEffectColor != null)
        {
            Instantiate(_teleportationEffect, transform)
                    .GetComponent<ParticlesColor>().ChangeColorOfEffect(tpEffectColor.Value);
        }
        else
        {
            Instantiate(_teleportationEffect, transform);
        }
    }

    IEnumerator Cooldown(System.Action<bool> isOnCooldown, float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown(false);
    }
}
