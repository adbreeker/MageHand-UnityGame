using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PotionEffect : MonoBehaviour
{
    [Range(0, 600),Header("Potion duration time")]
    public float duration;
    protected Coroutine _potionEffect;
    public abstract void Drink();
    public abstract void ActivatePotionEffect();
}
