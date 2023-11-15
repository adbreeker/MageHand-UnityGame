using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public SoundManager.Sound startMusic;
    public SoundManager.Sound loopMusic;

    private AudioSource startMusicAS;
    private AudioSource loopMusicAS;

    public bool playBackgroundMusic = true;

    void Start()
    {
        startMusicAS = FindObjectOfType<SoundManager>().CreateAudioSource(startMusic);
        loopMusicAS = FindObjectOfType<SoundManager>().CreateAudioSource(loopMusic);

        if(playBackgroundMusic) startMusicAS.Play();
    }
    void Update()
    {
        if(playBackgroundMusic && !PlayerParams.Controllers.pauseMenu.menuOpened && !startMusicAS.isPlaying && !loopMusicAS.isPlaying) loopMusicAS.Play();

        if(!playBackgroundMusic)
        {
            startMusicAS.Stop();
            loopMusicAS.Stop();
        }
    }
}
