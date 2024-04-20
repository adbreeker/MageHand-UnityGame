using UnityEngine;

public class PythonCrashed : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.UI_SelectOption).Play();
            GameParams.Managers.fadeInOutManager.CloseGame();
        }
    }
}
