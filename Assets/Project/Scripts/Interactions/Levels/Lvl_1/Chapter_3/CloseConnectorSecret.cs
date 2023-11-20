using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseConnectorSecret : MonoBehaviour
{
    [SerializeField] OpenWallPassage _wallPassage;
    [SerializeField] GameObject _tileTriggeringClose;

    // Update is called once per frame
    void Update()
    {
        if(_wallPassage.passageOpen)
        {
            Vector3 pos = new Vector3(_tileTriggeringClose.transform.position.x, PlayerParams.Objects.player.transform.position.y, _tileTriggeringClose.transform.position.z);
            if(PlayerParams.Objects.player.transform.position == pos)
            {
                _wallPassage.Interaction();
            }
        }
    }
}
