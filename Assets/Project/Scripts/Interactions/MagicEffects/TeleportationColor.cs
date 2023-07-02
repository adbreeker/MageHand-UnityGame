using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportationColor : MonoBehaviour
{
    public List<ParticleSystem> particles;

    public void ChangeColorOfEffect(Color color)
    {
        foreach(ParticleSystem particle in particles)
        {
            color.a = particle.startColor.a;
            particle.startColor = color;
        }
    }
}
