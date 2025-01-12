using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

public class WolfMedallionBehavior : ItemBehavior
{
    List<GameObject> _secretTriggers = new List<GameObject>();

    public override void OnPickUp() //fix shield rotation on pick up
    {
        base.OnPickUp();
        transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        _secretTriggers = FindObjectsOfType<SecretPlace>().Select(sp => sp.gameObject).ToList();
    }

    private void Update()
    {
        if(PlayerParams.Controllers.handInteractions.inHand == gameObject)
        {
            GameObject nearest = GetNearestSecret();
            if(nearest != null && Vector3.Distance(nearest.transform.position, gameObject.transform.position) < 8f)
            {
                float distance = Vector3.Distance(nearest.transform.position, gameObject.transform.position);

                transform.localPosition = Vector3.MoveTowards(
                    transform.localPosition, 
                    new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), transform.localPosition.z), 
                    Mathf.Lerp(15, 0, distance/8f) * Time.unscaledDeltaTime * (PlayerParams.Controllers.pauseMenu.freezeTime ? 0 : 1));
            }
            else
            {
                transform.localPosition = new Vector3(0f, 0f, transform.localPosition.z);
            }
        }
    }

    GameObject GetNearestSecret()
    {
        GameObject nearest = null;
        for (int i = _secretTriggers.Count - 1; i >= 0; i--)
        {
            if (_secretTriggers[i] == null)
            {
                _secretTriggers.RemoveAt(i);
            }
            else if (nearest == null ||
                Vector3.Distance(_secretTriggers[i].transform.position, gameObject.transform.position) <
                Vector3.Distance(nearest.transform.position, gameObject.transform.position))
            {
                nearest = _secretTriggers[i];
            }
        }
        return nearest;
    }
}
