using UnityEngine;

public class SecretPickUp : MonoBehaviour
{
    private bool notPicked = true;

    private void Awake()
    {
        GameParams.Managers.levelInfoManager.AddSecretOnLevel();
    }

    public void OnPickUp()
    {
        if(notPicked)
        {
            notPicked = false;
            GameParams.Managers.levelInfoManager.SecretFoundPopUp();
            Destroy(this);
        }
    }
}
