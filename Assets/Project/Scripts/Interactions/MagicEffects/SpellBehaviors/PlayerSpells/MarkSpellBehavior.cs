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

        foreach(GameObject gameObject in _particlesToTurnOff) 
        {
            gameObject.SetActive(false);
        }
        _trailForThrow.gameObject.SetActive(false);

        transform.SetParent(gameObject.transform);
        Destroy(GetComponent<Rigidbody>());
        GetComponent<Collider>().enabled = false;

        Instantiate(specialEffectPrefab, transform.position, Quaternion.identity);
        StartCoroutine(IncreaseSize());
    }

    public override void Reactivation()
    {
        PlayerParams.Controllers.handInteractions.inHand = null;
        PlayerParams.Controllers.spellCasting.currentSpell = "None";

        OnThrow();
        Vector3 destination = PlayerParams.Objects.player.transform.position;
        destination.y = 0.01f;

        transform.SetParent(null);

        transform.rotation = Quaternion.LookRotation(destination - gameObject.transform.position);
        StartCoroutine(MoveTowardsGround(destination));
    }

    IEnumerator MoveTowardsGround(Vector3 destinaion)
    {
        while(transform.position != destinaion)
        {
            yield return new WaitForFixedUpdate();
            gameObject.transform.position = Vector3.MoveTowards(transform.position, destinaion, 0.1f);
        }

        if (PlayerParams.Controllers.spellCasting.magicMark != null)
        {
            Destroy(PlayerParams.Controllers.spellCasting.magicMark);
        }
        PlayerParams.Controllers.spellCasting.magicMark = gameObject;

        foreach (GameObject gameObject in _particlesToTurnOff)
        {
            gameObject.SetActive(false);
        }
        _trailForThrow.gameObject.SetActive(false);

        transform.rotation = Quaternion.Euler(90, 0, 0);
        GetComponent<Collider>().enabled = false;

        Instantiate(specialEffectPrefab, transform.position, Quaternion.Euler(90,0,0));
        StartCoroutine(IncreaseSize());
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
}
