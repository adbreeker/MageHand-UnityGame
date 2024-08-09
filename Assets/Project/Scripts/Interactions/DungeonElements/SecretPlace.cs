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

    private void Update()
    {
        Bounds cubeBounds = GetComponent<BoxCollider>().bounds;
        if (cubeBounds.Contains(PlayerParams.Objects.player.transform.position) && notVisited)
        {
            notVisited = false;
            GameParams.Managers.levelInfoManager.SecretFoundPopUp();
            Destroy(gameObject);
        }
    }
}
