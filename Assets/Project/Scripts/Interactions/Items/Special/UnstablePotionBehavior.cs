using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnstablePotionBehavior : ItemBehavior
{
    [Space(20f), Header("Impact prefabs:")]
    [SerializeField] GameObject _potionImpactPrefab;
    [SerializeField] GameObject _dispelImpactPrefab;
    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        Instantiate(_potionImpactPrefab, transform.position, Quaternion.identity).transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        Collider[] colliders = Physics.OverlapSphere(transform.position, 3, LayerMask.GetMask("Spell"), QueryTriggerInteraction.Collide);
        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Wall")
            {
                Instantiate(_dispelImpactPrefab,
                    new Vector3(collider.transform.position.x, collider.transform.position.y + 2, collider.transform.position.z),
                    Quaternion.identity);
            }
            else { Instantiate(_dispelImpactPrefab, collider.transform.position, Quaternion.identity); }
            Destroy(collider.gameObject);
        }

        Destroy(gameObject);
    }
}
