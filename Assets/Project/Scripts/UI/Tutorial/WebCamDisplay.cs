using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class WebCamDisplay : MonoBehaviour
{
    const int Width = 640;
    const int Height = 480;
    const int FrameBytes = Width * Height * 3;
    const string MapName = "magehand_video_unity";

    [Header("Webcam display")]
    public RawImage webcamVideoDisplay;
    public RectTransform webcamVideoDisplayFrame;
    [Space]
    [SerializeField] private bool displayAutomaticaly = true;

    Texture2D texture;
    byte[] buffer;
    MemoryMappedFile map;
    MemoryMappedViewStream stream;
    float nextFrame;

    void Awake()
    {
        if (webcamVideoDisplay == null)
            webcamVideoDisplay = GetComponent<RawImage>();
        if (webcamVideoDisplay == null)
        {
            enabled = false;
            return;
        }

        if (webcamVideoDisplayFrame == null)
            webcamVideoDisplayFrame = webcamVideoDisplay.rectTransform;

        buffer = new byte[FrameBytes];
        texture = new Texture2D(Width, Height, TextureFormat.RGB24, false);
        texture.name = "WebCamDisplay_Webcam";

        webcamVideoDisplay.texture = texture;
        webcamVideoDisplay.color = Color.black;
    }

    void Update()
    {
        if (displayAutomaticaly) ShowCamera();
    }

    public void ShowCamera()
    {
        if (webcamVideoDisplay == null || webcamVideoDisplayFrame == null)
            return;

        float now = Time.unscaledTime;
        if (now < nextFrame) return;
        nextFrame = now + 1f / 30f;

        if (stream == null)
        {
            try
            {
                map = MemoryMappedFile.OpenExisting(MapName);
                stream = map.CreateViewStream(0, FrameBytes, MemoryMappedFileAccess.Read);
            }
            catch (IOException)
            {
                CloseMap();
                webcamVideoDisplay.gameObject.SetActive(false);
                return;
            }
        }

        try
        {
            stream.Position = 0;

            int offset = 0;
            while (offset < FrameBytes)
            {
                int count = stream.Read(buffer, offset, FrameBytes - offset);
                if (count == 0)
                {
                    CloseMap();
                    webcamVideoDisplay.gameObject.SetActive(false);
                    return;
                }

                offset += count;
            }

            texture.LoadRawTextureData(buffer);
            texture.Apply(false, false);

            float scale = (float)Height / (float)Width;
            webcamVideoDisplayFrame.sizeDelta = new Vector2(webcamVideoDisplayFrame.sizeDelta.x, webcamVideoDisplayFrame.sizeDelta.x * scale);

            webcamVideoDisplayFrame.gameObject.SetActive(true);
            webcamVideoDisplay.gameObject.SetActive(true);
            webcamVideoDisplay.color = Color.white;
        }
        catch (IOException)
        {
            CloseMap();
            webcamVideoDisplay.gameObject.SetActive(false);
        }
    }

    void CloseMap()
    {
        stream?.Dispose();
        stream = null;

        map?.Dispose();
        map = null;
    }

    void OnDestroy()
    {
        CloseMap();

        if (webcamVideoDisplay != null && webcamVideoDisplay.texture == texture)
            webcamVideoDisplay.texture = null;

        if (texture != null)
            Destroy(texture);
    }
} 
