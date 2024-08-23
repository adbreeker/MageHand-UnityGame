using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretPlace : MonoBehaviour
{
    private bool notVisited = true;

    private void Awake()
    {
        GameParams.Managers.levelInfoManager.AddSecretOnLevel();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerParams.Objects.player && notVisited)
        {
            notVisited = false;
            GameParams.Managers.levelInfoManager.SecretFoundPopUp();
            Destroy(gameObject);
        }
    }
}
