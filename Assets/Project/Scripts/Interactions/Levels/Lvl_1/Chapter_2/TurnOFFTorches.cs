using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOFFTorches : MonoBehaviour
{
    public List<GameObject> torches;
    public List<GameObject> walls;
    public GameObject torchNoLightPrefab;
    void Start()
    {
        StartCoroutine(ChangeTorches());
    }

    IEnumerator ChangeTorches()
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
