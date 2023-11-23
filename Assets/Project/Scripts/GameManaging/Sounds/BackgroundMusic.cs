using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public SoundManager.Sound startMusic;
    public SoundManager.Sound loopMusic;

    public bool muteBackgroundMusic = false;

    [Header("Read only")]
    public AudioSource startMusicAS;
    public AudioSource loopMusicAS;

    void Start()
    {
        startMusicAS = FindObjectOfType<SoundManager>().CreateAudioSource(startMusic);
        loopMusicAS = FindObjectOfType<SoundManager>().CreateAudioSource(loopMusic);

        if(!muteBackgroundMusic) startMusicAS.Play();
    }
    void Update()
    {
        if (!muteBackgroundMusic && !startMusicAS.isPlaying && !loopMusicAS.isPlaying)
        {
            if (PlayerParams.Controllers.pauseMenu != null)
            {
                if (!PlayerParams.Controllers.pauseMenu.menuOpened) loopMusicAS.Play();
            }
            else loopMusicAS.Play();
        }

        if(muteBackgroundMusic)
        {
            startMusicAS.Stop();
            loopMusicAS.Stop();
        }
    }
}
