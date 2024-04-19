using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WakingUpAnimation : MonoBehaviour
{
    GameObject player;
    public bool playAnimation = true;

    [Header("Speed")]
    public float rotationSpeed = 15f;
    public float movementSpeed = 0.5f;
    public float blackoutSpeed = 0.3f;

    private bool rotationCompleted = false;
    private bool movementCompleted = false;
    private bool blackoutCompleted = false;

    private float alpha;
    private void Start()
    {
        player = PlayerParams.Objects.player;

        if (playAnimation)
        {
            player.transform.rotation = Quaternion.Euler(30, player.transform.rotation.y, player.transform.rotation.z);
            player.transform.position = new Vector3(player.transform.position.x, 0, player.transform.position.z);
            GetComponent<RawImage>().color = new Color(0, 0, 0, 1f);
        }
        else Destroy(gameObject);
    }
    private void Update()
    {
        player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, Quaternion.Euler(0, player.transform.rotation.y, player.transform.rotation.z), rotationSpeed * Time.deltaTime);
        if (Quaternion.Angle(player.transform.rotation, Quaternion.Euler(0, player.transform.rotation.y, player.transform.rotation.z)) < 0.01f)
        {
            player.transform.rotation = Quaternion.Euler(0, 0, 0);
            rotationCompleted = true;
        }

        player.transform.position = Vector3.MoveTowards(player.transform.position, new Vector3(player.transform.position.x, 1, player.transform.position.z), movementSpeed * Time.deltaTime);
        if (player.transform.position == new Vector3(player.transform.position.x, 1, player.transform.position.z))
        {
            movementCompleted = true;
        }

        alpha = GetComponent<RawImage>().color.a;
        if (alpha > 0)
        {
            alpha -= blackoutSpeed * Time.deltaTime;
            GetComponent<RawImage>().color = new Color(0, 0, 0, alpha);
        }
        else blackoutCompleted = true;



        if (rotationCompleted && movementCompleted && blackoutCompleted) Destroy(gameObject);
    }
}
