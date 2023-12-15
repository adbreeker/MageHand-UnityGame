using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class BackgroundMusic : MonoBehaviour
{
    public SoundManager.Sound startMusic;
    public SoundManager.Sound loopMusic;

    public bool modifyBackgroundMusicVolume = false;

    [Header("Read only")]
    public AudioSource startMusicAS;
    public AudioSource loopMusicAS;

    private SoundManager soundManager;

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();

        startMusicAS = soundManager.CreateAudioSource(startMusic);
        loopMusicAS = soundManager.CreateAudioSource(loopMusic);

        if(!GameSettings.muteMusic) startMusicAS.Play();
    }
    void Update()
    {
        if (!GameSettings.muteMusic && !startMusicAS.isPlaying && !loopMusicAS.isPlaying) loopMusicAS.Play();

        if (GameSettings.muteMusic && startMusicAS.volume != 0) startMusicAS.volume = 0;
        if (GameSettings.muteMusic && loopMusicAS.volume != 0) loopMusicAS.volume = 0;

        if (!GameSettings.muteMusic && PlayerParams.Controllers.pauseMenu == null && !modifyBackgroundMusicVolume)
        {
            if(startMusicAS.volume != soundManager.GetBaseVolume(soundManager.GetFirstSoundEnumByAudioClip(startMusicAS.clip)) * GameSettings.soundVolume)
            {
                startMusicAS.volume = soundManager.GetBaseVolume(soundManager.GetFirstSoundEnumByAudioClip(startMusicAS.clip)) * GameSettings.soundVolume;
            }

            if (loopMusicAS.volume != soundManager.GetBaseVolume(soundManager.GetFirstSoundEnumByAudioClip(loopMusicAS.clip)) * GameSettings.soundVolume)
            {
                loopMusicAS.volume = soundManager.GetBaseVolume(soundManager.GetFirstSoundEnumByAudioClip(loopMusicAS.clip)) * GameSettings.soundVolume;
            }
        }  
    }
}
