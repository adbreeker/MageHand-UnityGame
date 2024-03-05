using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretPlace : MonoBehaviour
{
    private AudioSource sound;
    private bool notVisited = true;
    private string text;

    [TextArea(1, 2)]
    public float timeToFadeOut = 2;
    public float timeOfFadingOut = 0.007f;

    private void Awake()
    {
        FindObjectOfType<LevelInfoDisplay>().secretsOnLevel += 1;
    }
    private void Start()
    {
        PlayerParams.Controllers.pointsManager.maxFoundSecrets += 1;
        sound = FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.SFX_SecretFound);
    }
    private void Update()
    {
        Bounds cubeBounds = GetComponent<Renderer>().bounds;
        if (cubeBounds.Contains(PlayerParams.Objects.player.transform.position) && notVisited)
        {
            sound.Play();
            notVisited = false;
            PlayerParams.Controllers.pointsManager.foundSecrets += 1;
            text = "Secret found!<br>" + PlayerParams.Controllers.pointsManager.foundSecrets + "/" + FindObjectOfType<LevelInfoDisplay>().secretsOnLevel;
            FindObjectOfType<HUD>().SpawnPopUp(text, timeToFadeOut, timeOfFadingOut, false);
            Destroy(sound, sound.clip.length);
            Destroy(this, sound.clip.length);
        }
    }
}
