using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public SoundManager.Sound startMusic;
    public SoundManager.Sound loopMusic;

    public bool playBackgroundMusic = true;

    [Header("Read only")]
    public AudioSource startMusicAS;
    public AudioSource loopMusicAS;

    void Start()
    {
        startMusicAS = FindObjectOfType<SoundManager>().CreateAudioSource(startMusic);
        loopMusicAS = FindObjectOfType<SoundManager>().CreateAudioSource(loopMusic);

        if(playBackgroundMusic) startMusicAS.Play();
    }
    void Update()
    {
        if (playBackgroundMusic && !startMusicAS.isPlaying && !loopMusicAS.isPlaying)
        {
            if (PlayerParams.Controllers.pauseMenu != null)
            {
                if (!PlayerParams.Controllers.pauseMenu.menuOpened) loopMusicAS.Play();
            }
            else loopMusicAS.Play();
        }
        if(!playBackgroundMusic)
        {
            startMusicAS.Stop();
            loopMusicAS.Stop();
        }
    }
}
