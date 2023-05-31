using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChapterExitCube : MonoBehaviour
{
    public LayerMask playerMask;
    public string chapter;
    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(new Vector3(transform.position.x, 2f, transform.position.z), 0.5f, playerMask);
        if (colliders.Length > 0)
        {
            SceneManager.LoadScene(chapter);
        }
    }
}
