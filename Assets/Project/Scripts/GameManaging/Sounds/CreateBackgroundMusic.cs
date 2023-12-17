using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBackgroundMusic : MonoBehaviour
{
    public SoundManager.Sound startMusic;
    public SoundManager.Sound loopMusic;
    void Awake()
    {
        if(FindObjectOfType<BackgroundMusic>() == null)
        {
            GameObject backgroundMusicGameObject = new GameObject("BackgroundMusic");
            BackgroundMusic backgroundMusic = backgroundMusicGameObject.AddComponent<BackgroundMusic>();
            backgroundMusic.startMusic = startMusic;
            backgroundMusic.loopMusic = loopMusic;
        }
    }
}
