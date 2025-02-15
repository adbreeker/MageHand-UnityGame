using FMODUnity;
using System;
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
    float _startingMS;
    public float rotationSpeed = 360f;
    float _startingRS;
    float _rotationSpeedPerMS;

    [Header("Attack:")]
    public float attackRange;
    public float attackCooldown;

    [Header("Teleportation effect")]
    [SerializeField] GameObject _teleportationEffect;

    public event Action<GameObject> OnWolfHitted; 

    Coroutine _movementCoroutine = null;
    //cooldowns
    bool _isAttackOnCooldown = false;
    bool _isDamageOnCooldown = false;
    bool _isDead = false;

    private void Awake()
    {
        _startingMS = movementSpeed;
        _startingRS = rotationSpeed;
        _rotationSpeedPerMS = rotationSpeed / movementSpeed;
    }

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
            AdjustMovement(adjustMovementToPlayer, _startingMS, _startingRS);
            _movementCoroutine = StartCoroutine(MovementCoroutine(movementPath, adjustMovementToPlayer, destroyOnLastTile));
        }
        else
        {
            StopCoroutine(_movementCoroutine);
            AdjustMovement(adjustMovementToPlayer, _startingMS, _startingRS);
            _movementCoroutine = StartCoroutine(MovementCoroutine(movementPath, adjustMovementToPlayer, destroyOnLastTile));
        }
    }

    public void SetWolfMovement(List<Transform> movementPath, float newMovementSpeed, float newRotationSpeed, bool destroyOnLastTile = false)
    {
        if (_movementCoroutine == null)
        {
            AdjustMovement(false, newMovementSpeed, newRotationSpeed);
            _movementCoroutine = StartCoroutine(MovementCoroutine(movementPath, false, destroyOnLastTile));
        }
        else
        {
            StopCoroutine(_movementCoroutine);
            AdjustMovement(false, newMovementSpeed, newRotationSpeed);
            _movementCoroutine = StartCoroutine(MovementCoroutine(movementPath, false, destroyOnLastTile));
        }
    }

    IEnumerator MovementCoroutine(List<Transform> movementPath, bool adjustMovementToPlayer, bool destroyOnLastTile)
    {
        foreach (Transform t in movementPath) 
        {
            Vector3 pathRelativePos = new Vector3(t.position.x, transform.position.y, t.position.z);

            Quaternion targetRotation;
            if (pathRelativePos - transform.position != Vector3.zero) { targetRotation = Quaternion.LookRotation(pathRelativePos - transform.position); }
            else { targetRotation = transform.rotation; }
            
            while(transform.rotation != targetRotation)
            {
                yield return new WaitForFixedUpdate();
                AdjustMovement(adjustMovementToPlayer, movementSpeed, rotationSpeed);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            yield return new WaitForFixedUpdate();

            while(transform.position != pathRelativePos)
            {
                yield return new WaitForFixedUpdate();
                AdjustMovement(adjustMovementToPlayer, movementSpeed, rotationSpeed);
                transform.position = Vector3.MoveTowards(transform.position, pathRelativePos, movementSpeed * Time.deltaTime);
            }
        }

        _animator.SetBool("run", false);
        _animator.SetBool("walk", false);
        _movementCoroutine = null;

        if (destroyOnLastTile) { Destroy(gameObject); }
    }

    void AdjustMovement(bool toPlayer, float ms, float rs)
    {
        if(_isDead)
        {
            movementSpeed = 0;
            rotationSpeed = 0;
            return;
        }
        else
        {
            movementSpeed = ms;
            rotationSpeed = rs;
        }

        if(toPlayer)
        {
            if (PlayerParams.Controllers.playerMovement.movementSpeed >= movementSpeed)
            {
                //if adjusted wolf is 10% faster than player
                movementSpeed = PlayerParams.Controllers.playerMovement.movementSpeed + 0.2f * PlayerParams.Controllers.playerMovement.movementSpeed;
                rotationSpeed = movementSpeed * _rotationSpeedPerMS;
            }
        }

        if(movementSpeed > 3.0f)
        {
            _animator.SetBool("run", true);
        }
        else
        {
            _animator.SetBool("walk", true);
        }
    }

    void AttackPlayerInRange()
    {
        if (!_isDead &&!_isAttackOnCooldown)
        {
            Vector3 attackPoint = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
            Debug.DrawRay(attackPoint, transform.forward * attackRange, Color.yellow);

            if (Physics.Raycast(attackPoint, transform.forward, attackRange, LayerMask.GetMask("Player")))
            {
                _isAttackOnCooldown = true;
                _animator.SetTrigger("attack");
                StartCoroutine(Cooldown(cooldown => _isAttackOnCooldown = cooldown, attackCooldown));
            }
        }
    }

    void GetHit(GameObject hittedBy)
    {
        if(!_isDead)
        {
            OnWolfHitted?.Invoke(hittedBy);

            if (hittedBy.layer == LayerMask.NameToLayer("Item"))
            {
                if (hittedBy.GetComponent<KnifeBehavior>() != null || hittedBy.GetComponent<FmsBehavior>() != null)
                {

                    _animator.SetTrigger("dead");
                    StartCoroutine(DestroyAfterTime(2.0f));
                }
                else
                {
                    if (!_isDamageOnCooldown)
                    {
                        _isDamageOnCooldown = true;
                        _animator.SetTrigger("damage");
                        StartCoroutine(Cooldown(cooldown => _isDamageOnCooldown = cooldown, 0.7f));
                    }
                }
            }
            else if (hittedBy.layer == LayerMask.NameToLayer("Spell"))
            {
                if (hittedBy.GetComponent<LightSpellBehavior>() != null)
                {
                    if (!_isDamageOnCooldown)
                    {
                        _isDamageOnCooldown = true;
                        _animator.SetTrigger("damage");
                        StartCoroutine(Cooldown(cooldown => _isDamageOnCooldown = cooldown, 0.7f));
                    }
                    return;
                }
                if (hittedBy.GetComponent<FireSpellBehavior>() != null)
                {
                    _animator.SetTrigger("dead");
                    StartCoroutine(DestroyAfterTime(0.1f));
                    return;
                }
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
            _animator.SetBool("run", false);
            _animator.SetBool("walk", false);
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
            _animator.SetBool("run", false);
            _animator.SetBool("walk", false);
            _movementCoroutine = null;
        }

        GameParams.Managers.audioManager.PlayOneShotOccluded(GameParams.Managers.fmodEvents.SFX_Teleport, transform.position);
        GameParams.Managers.audioManager.PlayOneShotOccluded(GameParams.Managers.fmodEvents.SFX_Teleport, transform);

        transform.position = tpDestination;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, tpRotation, transform.rotation.eulerAngles.z);

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

    IEnumerator DestroyAfterTime(float deley)
    {
        _isDead = true;
        _meshCollider.enabled = false;
        yield return new WaitForSeconds(deley);
        gameObject.AddComponent<Destroyable>().Destroy();
    }

    private void OnCollisionEnter(Collision collision)
    {
        GetHit(collision.gameObject);
    }
}
