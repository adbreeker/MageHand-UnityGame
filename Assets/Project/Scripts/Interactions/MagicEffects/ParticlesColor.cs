using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesColor : MonoBehaviour
{
    [Header("All particles in teleportation effect")]
    public List<ParticleSystem> particles;

    public void ChangeColorOfEffect(Color color) //change color of teleportation effect
    {
        foreach(ParticleSystem particle in particles)
        {
            ParticleSystem.MainModule mainModule = particle.main;
            color.a = mainModule.startColor.color.a;
            mainModule.startColor = color;
        }
    }

    public void ChangeLooping(bool looping)
    {
        foreach (ParticleSystem particle in particles)
        {
            ParticleSystem.MainModule mainModule = particle.main;
            mainModule.loop = looping;
        }
    }
}
