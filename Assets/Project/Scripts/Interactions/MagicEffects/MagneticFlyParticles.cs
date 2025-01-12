using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticFlyParticles : MonoBehaviour
{
    [SerializeField] Transform _attractingTransform;
    [SerializeField] List<ParticleSystem> _particlesToAttract;

    public void Initialize(Transform attractingTransform)
    {
        _attractingTransform = attractingTransform;

        foreach (ParticleSystem particle in _particlesToAttract)
        {
            ParticleSystem.TriggerModule trigger = particle.trigger;
            trigger.enabled = true;
            trigger.AddCollider(attractingTransform.GetComponent<Collider>());

            ParticleSystem.ExternalForcesModule externalForces = particle.externalForces;
            externalForces.enabled = true;
            externalForces.AddInfluence(attractingTransform.GetComponent<ParticleSystemForceField>());
        }
    }
}
