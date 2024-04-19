using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrunkRoomBehavior : MonoBehaviour
{
    [SerializeField] Transform _playerSpawnpoint;
    [SerializeField] RawImage _blackoutScreen;
    [SerializeField] OpenDialogue _openDialogue;
    [SerializeField] GameObject _dialogueTp;

    Vector3 playerPreviousPos;

    private void Awake()
    {
        playerPreviousPos = PlayerParams.Controllers.playerMovement.currentTilePos;
        StartCoroutine(WaitForAnimationAndDialogue());
    }

    IEnumerator WaitForAnimationAndDialogue()
    {
        GameObject player = PlayerParams.Objects.player;

        _blackoutScreen.color = new Color(0, 0, 0, 0f);

        float alpha;

        while (true)
        {
            alpha = _blackoutScreen.color.a;
            alpha += 0.01f;
            _blackoutScreen.color = new Color(0, 0, 0, alpha);

            if (alpha >= 1) break;

            yield return new WaitForFixedUpdate();
        }

        PlayerParams.Controllers.playerMovement.TeleportTo(_playerSpawnpoint.position);
        yield return new WaitForSeconds(3.0f);

        PlayerParams.Controllers.pointsManager.minPlotPoints += -3;
        PlayerParams.Controllers.pointsManager.plotPoints += -3;

        while (true)
        {
            alpha = _blackoutScreen.color.a;
            alpha -= 0.01f;
            _blackoutScreen.color = new Color(0, 0, 0, alpha);

            if (alpha <= 0) break;

            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(5.0f);
        _openDialogue.gameObject.SetActive(true);

        while(!_dialogueTp.activeSelf)
        {
            yield return null;
        }

        while(_dialogueTp.activeSelf)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2.0f);

        PlayerParams.Controllers.playerMovement.TeleportTo(playerPreviousPos, null);
        Destroy(gameObject);
    }
}
