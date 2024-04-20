using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretPlace : MonoBehaviour
{
    private AudioSource sound;
    private bool notVisited = true;
    private string text;

    public float timeToFadeOut = 2;
    public float timeOfFadingOut = 0.007f;

    private void Awake()
    {
        GameParams.Managers.levelInfoManager.secretsOnLevel += 1;
    }
    private void Start()
    {
        PlayerParams.Controllers.pointsManager.maxFoundSecrets += 1;
        sound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_SecretFound);
    }
    private void Update()
    {
        Bounds cubeBounds = GetComponent<BoxCollider>().bounds;
        if (cubeBounds.Contains(PlayerParams.Objects.player.transform.position) && notVisited)
        {
            sound.Play();
            notVisited = false;
            PlayerParams.Controllers.pointsManager.foundSecrets += 1;
            GameParams.Managers.levelInfoManager.foundSecretsOnLevel += 1;
            text = "Secret found!<br>" + GameParams.Managers.levelInfoManager.foundSecretsOnLevel + "/" + GameParams.Managers.levelInfoManager.secretsOnLevel;
            PlayerParams.Controllers.HUD.SpawnPopUp(text, timeToFadeOut, timeOfFadingOut, false);
            Destroy(sound, sound.clip.length);
            Destroy(this, sound.clip.length);
        }
    }
}
