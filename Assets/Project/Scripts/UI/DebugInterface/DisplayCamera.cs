using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCamera : MonoBehaviour
{
    private WebCamTexture webCamTexture;

    void Awake()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        Debug.Log("Devices l: " + devices.Length);

        if(devices.Length > 0)
        {
            webCamTexture = new WebCamTexture(devices[0].name);
            GetComponent<RawImage>().texture = webCamTexture;
            Debug.Log("lapie obraz");
            webCamTexture.Play();
            Debug.Log("gram obraz");
        }
    }

    private void OnApplicationQuit()
    {
        if(webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }
    }

}
