using UnityEngine;

public class StoneMissileBehavior : SpellBehavior
{
    public float pushForce;

    void Start()
    {

    }

    public override void OnImpact(GameObject impactTarget)
    {
        Instantiate(specialEffectPrefab, transform.position, Quaternion.identity);

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
        if (PlayerParams.Objects.player.GetComponent<PushPlayerEffect>() == null)
        {
            PushPlayerEffect ppe = PlayerParams.Objects.player.AddComponent<PushPlayerEffect>();

            Vector3 pushDirection = GetComponent<Rigidbody>().linearVelocity;
            pushDirection.y = 0;
            pushDirection = SingleDirectionNormalization(pushDirection);

            Vector3 pushDestination = PlayerParams.Controllers.playerMovement.currentOnTilePos + pushDirection * PlayerParams.Controllers.playerMovement.tilesLenght;

            ppe.Initialize(pushDestination, pushForce);
        }
    }

    Vector3 SingleDirectionNormalization(Vector3 direction)
    {
        direction = direction.normalized; //direction normalization
        int xsign, zsign;

        //geting sign of x and z variable
        if (direction.x >= 0) { xsign = 1; }
        else { xsign = -1; }

        if (direction.z >= 0) { zsign = 1; }
        else { zsign = -1; }

        //single directioning (dominant is direction we are closer to)
        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.z))
        {
            direction.x = 1 * xsign;
            direction.z = 0;
        }
        else
        {
            direction.x = 0;
            direction.z = 1 * zsign;
        }
        return direction;
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
