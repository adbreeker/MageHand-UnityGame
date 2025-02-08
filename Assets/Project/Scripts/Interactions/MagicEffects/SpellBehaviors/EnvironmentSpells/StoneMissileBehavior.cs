using UnityEngine;

public class StoneMissileBehavior : SpellBehavior
{
    public float pushForce;

    GameObject _instantiatedEffect;

    void Start()
    {

    }

    public override void OnImpact(GameObject impactTarget)
    {
        _instantiatedEffect = Instantiate(specialEffectPrefab, transform.position, Quaternion.identity);


        //managing all pushing
        if (impactTarget.layer == LayerMask.NameToLayer("Player")) //player
        {
            ManagePushingPlayer();
        }
        else if (impactTarget.layer == LayerMask.NameToLayer("Interaction")) //interaction
        {
            impactTarget.SendMessage("OnClick");
        }
        else if (impactTarget.GetComponent<ItemBehavior>() != null)  //item
        {
            ManagePushingItem(impactTarget);
        }
        else if (impactTarget.GetComponent<SpellBehavior>() != null) // spell
        {
            ManagePushingSpell(impactTarget);
        }

        Destroy(gameObject);
    }

    void ManagePushingPlayer()
    {
        if (PlayerParams.Objects.player.GetComponent<PotionSpeedEffect>() == null)
        {

        }
    }

    void ManagePushingItem(GameObject item)
    {
        if (item.tag == "Shield")
        {
            //shield is protecting from magic missiles
        }
        else
        {
            if (item.transform.parent == null)
            {
                Rigidbody rb = item.GetComponent<Rigidbody>();
                if (rb == null) { rb = item.AddComponent<Rigidbody>(); }

                rb.AddExplosionForce(pushForce, transform.position, Vector3.Distance(item.transform.position, transform.position) * 2f);
            }
            else if (PlayerParams.Controllers.handInteractions.inHand == item)
            {
                ManagePushingPlayer();
            }
            else
            {

            }
        }
    }

    void ManagePushingSpell(GameObject spell)
    {
        if (PlayerParams.Controllers.handInteractions.inHand == spell)
        {
            Destroy(spell);
            PlayerParams.Controllers.handInteractions.inHand = null;
            PlayerParams.Controllers.spellCasting.currentSpell = "None";
            ManagePushingPlayer();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnImpact(collision.collider.gameObject);
    }
}
