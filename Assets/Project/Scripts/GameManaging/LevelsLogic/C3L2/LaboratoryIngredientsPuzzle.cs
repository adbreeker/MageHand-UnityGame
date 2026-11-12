using System.Collections.Generic;
using UnityEngine;

public class LaboratoryIngredientsPuzzle : MonoBehaviour
{
    [Header("Levers:")]
    [SerializeField] LeverBehavior _leverVials;
    [SerializeField] LeverBehavior _leverJars;

    [Header("Vials:")]
    [SerializeField] List<AlteredTethered> _vials;

    [Header("Jars:")]
    [SerializeField] List<AlteredTethered> _jars;

    private void Start()
    {
        _leverVials.OnLeverPulledDownEvents += SpawnVials;
        _leverVials.OnLeverPulledUpEvents += HideVials;
        _leverJars.OnLeverPulledDownEvents += SpawnJars;
        _leverJars.OnLeverPulledUpEvents += HideJars;

        foreach (AlteredTethered vial in _vials)
        {
            vial.gameObject.SetActive(false);
        }
        foreach (AlteredTethered jar in _jars)
        {
            jar.gameObject.SetActive(false);
        }
    }

    public void SpawnVials(GameObject lever)
    {
        foreach (AlteredTethered vial in _vials)
        {
            vial.gameObject.SetActive(true);
            vial.TeleportBackToLocation();
        }
    }

    public void HideVials(GameObject lever)
    {
        foreach (AlteredTethered vial in _vials)
        {
            vial.TeleportBackToLocation();
            vial.gameObject.SetActive(false);
        }
    }

    public void SpawnJars(GameObject lever)
    {
        foreach (AlteredTethered jar in _jars)
        {
            jar.gameObject.SetActive(true);
            jar.TeleportBackToLocation();
        }
    }

    public void HideJars(GameObject lever)
    {
        foreach (AlteredTethered jar in _jars)
        {
            jar.TeleportBackToLocation();
            jar.gameObject.SetActive(false);
        }
    }
}
