using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOFFTorches : MonoBehaviour
{
    [Header("Torchs objects")]
    public List<GameObject> torches;

    [Header("Wall objects")]
    public List<GameObject> walls;

    [Header("No light torch prefab")]
    public GameObject torchNoLightPrefab;

    void Start() //turn off torches on scene start
    {
        StartCoroutine(ChangeTorches());
    }

    IEnumerator ChangeTorches() //change torches to no light torches in pairs every moment until all torches are turned off
    {
        yield return new WaitForSeconds(2f);

        for(int i = 0; i < torches.Count/2; i++)
        {
            Destroy(torches[i * 2]);
            Instantiate(torchNoLightPrefab, walls[i * 2].transform);

            Destroy(torches[i * 2 + 1]);
            Instantiate(torchNoLightPrefab, walls[i * 2 + 1].transform);
            yield return new WaitForSeconds(0.7f);
        }
    }
}
