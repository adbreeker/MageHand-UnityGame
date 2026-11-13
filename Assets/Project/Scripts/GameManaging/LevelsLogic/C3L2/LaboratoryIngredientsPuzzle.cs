using System.Collections.Generic;
using UnityEngine;

public class LaboratoryIngredientsPuzzle : MonoBehaviour
{
    [Header("Levers:")]
    [SerializeField] LeverBehavior _leverVials;
    [SerializeField] LeverBehavior _leverJars;

    [Header("Cauldron")]
    [SerializeField] CauldronBehavior _cauldron;

    [Header("Vials:")]
    [SerializeField] List<AlteredTethered> _vials;
    List<AlteredTethered> _spawnedVials = new List<AlteredTethered>();

    [Header("Jars:")]
    [SerializeField] List<AlteredTethered> _jars;

    [Header("Laboratory bounds")]
    [SerializeField] Collider _laboratoryBounds;

    private void Start()
    {
        _leverVials.OnLeverPulledDownEvents += SpawnVials;
        _leverVials.OnLeverPulledUpEvents += HideVials;
        _leverJars.OnLeverPulledDownEvents += SpawnJars;
        _leverJars.OnLeverPulledUpEvents += HideJars;

        _cauldron.OnBrewingResultSpawned += SetupBrewedObject;

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
            vial.TeleportBackToLocation(false);
        }
    }

    public void HideVials(GameObject lever)
    {
        foreach (AlteredTethered vial in _vials)
        {
            bool tpEffectAtOrigin = vial.gameObject.activeSelf || PlayerParams.Controllers.inventory.inventory.Contains(vial.gameObject);
            vial.TeleportBackToLocation(tpEffectAtOrigin);
            vial.gameObject.SetActive(false);
        }
        foreach (AlteredTethered spawnedVial in _spawnedVials)
        {
            bool tpEffectAtOrigin = spawnedVial.gameObject.activeSelf || PlayerParams.Controllers.inventory.inventory.Contains(spawnedVial.gameObject);
            spawnedVial.TeleportBackToLocation(tpEffectAtOrigin);
            //no set active false, cuz they use tethered with destroy on tether
        }
    }

    public void SpawnJars(GameObject lever)
    {
        foreach (AlteredTethered jar in _jars)
        {
            jar.gameObject.SetActive(true);
            jar.TeleportBackToLocation(false);
        }
    }

    public void HideJars(GameObject lever)
    {
        foreach (AlteredTethered jar in _jars)
        {
            bool tpEffectAtOrigin = jar.gameObject.activeSelf || PlayerParams.Controllers.inventory.inventory.Contains(jar.gameObject);
            jar.TeleportBackToLocation(tpEffectAtOrigin);
            jar.gameObject.SetActive(false);
        }
    }

    public void SetupBrewedObject(GameObject brewed)
    {
        if(brewed.GetComponent<ItemBehavior>().itemName.Contains("Vial") && brewed.GetComponent<AlteredTethered>() == null)
        {
            AlteredTethered alteredTethered = brewed.AddComponent<AlteredTethered>();
            alteredTethered.SetupTether(Vector3.one * 10000f, brewed.transform.rotation, _laboratoryBounds, float.PositiveInfinity, true);
            _spawnedVials.Add(alteredTethered);
        }
    }
}
