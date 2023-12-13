using UnityEngine;

public class PythonCrashed : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FindObjectOfType<SoundManager>().CreateAudioSource(SoundManager.Sound.UI_SelectOption).Play();
            FindObjectOfType<FadeInFadeOut>().CloseGame();
        }
    }
}
