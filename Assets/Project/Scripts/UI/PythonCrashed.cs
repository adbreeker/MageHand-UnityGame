using FMODUnity;
using UnityEngine;

public class PythonCrashed : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RuntimeManager.PlayOneShot(GameParams.Managers.fmodEvents.UI_SelectOption);
            GameParams.Managers.fadeInOutManager.CloseGame();
        }
    }
}
