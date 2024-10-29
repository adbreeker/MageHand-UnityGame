using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class WebCamDisplay : MonoBehaviour
{
    [Header("Webcam display")]
    public RawImage webcamVideoDisplay;
    public RectTransform webcamVideoDisplayFrame;
    [Space]
    [SerializeField] private bool displayAutomaticaly = true; 

    void Update()
    {
        if (displayAutomaticaly) ShowCamera();
    }

    public void ShowCamera()
    {
        webcamVideoDisplayFrame.gameObject.SetActive(true);

        try
        {
            using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("magehand_video_unity"))
            {
                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {
                    BinaryReader reader = new BinaryReader(stream);
                    byte[] frameBuffer = reader.ReadBytes(480 * 640 * 3);

                    int width = 640;
                    int height = 480;

                    Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
                    texture.LoadRawTextureData(frameBuffer);
                    texture.Apply();

                    float scale = (float)texture.height / (float)texture.width;
                    webcamVideoDisplayFrame.sizeDelta = new Vector2(webcamVideoDisplayFrame.sizeDelta.x, webcamVideoDisplayFrame.sizeDelta.x * scale);

                    webcamVideoDisplay.gameObject.SetActive(true);

                    webcamVideoDisplay.texture = texture;

                }
            }
        }
        catch (FileNotFoundException)
        {
            webcamVideoDisplay.gameObject.SetActive(false);
            Debug.Log("video_unity doesn't exist");
        }
    }
}
