using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChapterEntryCube : LevelExitCube
{

    protected override void ChangeLevel()
    {
        StartCoroutine(ChangeChapterAnimation());
    }

    IEnumerator ChangeChapterAnimation()
    {
        PlayerParams.Controllers.playerMovement.movementSpeed = 0;
        float stairsHigh = PlayerParams.Objects.player.transform.position.y + -2.5f;
        int stairCounter = 0;
        while (true)
        {
            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForFixedUpdate();
                Vector3 destination = transform.position;
                destination.y = PlayerParams.Objects.player.transform.position.y;
                PlayerParams.Objects.player.transform.position = Vector3.MoveTowards(PlayerParams.Objects.player.transform.position, destination, 0.03f);
            }

            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForFixedUpdate();
                Vector3 destination = PlayerParams.Objects.player.transform.position;
                destination.y = stairsHigh;
                PlayerParams.Objects.player.transform.position = Vector3.MoveTowards(PlayerParams.Objects.player.transform.position, destination, 0.05f);
            }

            stairCounter++;

            if (stairCounter >= 6)
            {
                break;
            }
        }

        SaveProgress();
        GameParams.Managers.fadeInOutManager.ChangeScene(chapter, fadeOutAndChangeMusic: changeMusic);
    }
}
