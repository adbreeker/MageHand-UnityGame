using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkSpellBehavior : SpellBehavior
{
    [Header("Particles:")]
    [SerializeField] List<GameObject> _particlesToTurnOff;
    [SerializeField] ParticleSystem _trailForThrow;

    public override void OnThrow()
    {
        _trailForThrow.gameObject.SetActive(true);
        _trailForThrow.Play();
    }


    public override void OnImpact(GameObject impactTarget)
    {
        if (PlayerParams.Controllers.spellCasting.magicMark != null)
        {
            Instantiate(specialEffectPrefab, PlayerParams.Controllers.spellCasting.magicMark.transform.position, PlayerParams.Controllers.spellCasting.magicMark.transform.rotation);
            Destroy(PlayerParams.Controllers.spellCasting.magicMark);
        }
        PlayerParams.Controllers.spellCasting.magicMark = gameObject;

        foreach (GameObject gameObject in _particlesToTurnOff)
        {
            gameObject.SetActive(false);
        }
        _trailForThrow.gameObject.SetActive(false);

        transform.SetParent(gameObject.transform);
        Destroy(GetComponent<Rigidbody>());
        GetComponent<Collider>().enabled = false;

        Instantiate(specialEffectPrefab, transform.position, Quaternion.identity);
        StartCoroutine(IncreaseSize());

        base.OnImpact(impactTarget);
    }

    public override void Reactivation()
    {
        Instantiate(specialEffectPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
        PlayerParams.Controllers.handInteractions.inHand = null;
        PlayerParams.Controllers.spellCasting.currentSpell = "None";
    }

    IEnumerator IncreaseSize()
    {
        Vector3 sizeToReach = transform.localScale * 2;

        while(transform.localScale != sizeToReach)
        {
            yield return new WaitForFixedUpdate();
            transform.localScale = Vector3.MoveTowards(transform.localScale, sizeToReach, 0.01f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        transform.rotation = Quaternion.LookRotation(collision.contacts[0].point - gameObject.transform.position);
    }
}
