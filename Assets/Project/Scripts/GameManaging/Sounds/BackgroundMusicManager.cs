using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    [SerializeField] private EventReference musicRef;
    void Awake()
    {
        BackgroundMusic backgroundMusic = FindAnyObjectByType<BackgroundMusic>();
        if (backgroundMusic == null)
        {
            GameObject backgroundMusicGameObject = new GameObject("BackgroundMusic");
            DontDestroyOnLoad(backgroundMusicGameObject);
            backgroundMusic = backgroundMusicGameObject.AddComponent<BackgroundMusic>();
            backgroundMusic.musicRef = musicRef;
        }
        GameParams.Managers.audioManager.backgroundMusic = backgroundMusic;
    }
}
