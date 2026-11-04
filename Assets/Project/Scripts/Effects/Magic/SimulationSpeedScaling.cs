using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationSpeedScaling : MonoBehaviour
{
    ParticleSystem[] particleSystems;

    private void Awake()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        if(Time.timeScale != 0)
        {
            foreach(ParticleSystem p in particleSystems) 
            {
                ParticleSystem.MainModule mainModule = p.main;
                mainModule.simulationSpeed = 1.0f/Time.timeScale;
            }
        }
    }
}
