using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportationColor : MonoBehaviour
{
    [Header("All particles in teleportation effect")]
    public List<ParticleSystem> particles;

    [System.Obsolete("This class is using deprecated method")]
    public void ChangeColorOfEffect(Color color) //change color of teleportation effect
    {
        foreach(ParticleSystem particle in particles)
        {
            color.a = particle.startColor.a;
            particle.startColor = color;
        }
    }
}
