using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionSpeedBehavior : MonoBehaviour
{
    public float duration = 30;

    public void OnPickUp()
    {

    }

    public void Drink()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PotionSpeedBehavior psb = player.AddComponent<PotionSpeedBehavior>();
        psb.duration = duration;
        psb.ActivatePotionEffect();
        Destroy(gameObject);
    }

    public void ActivatePotionEffect()
    {
        GetComponent<PlayerMovement>().movementSpeed *= 2;
        GetComponent<PlayerMovement>().rotationSpeed *= 2;
        StartCoroutine(PotionDuration());
    }

    IEnumerator PotionDuration()
    {
        yield return new WaitForSeconds(duration);
        GetComponent<PlayerMovement>().movementSpeed /= 2;
        GetComponent<PlayerMovement>().rotationSpeed /= 2;
        Destroy(this);
    }
}
