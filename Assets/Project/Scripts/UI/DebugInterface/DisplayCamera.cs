using System.IO;
using System.IO.MemoryMappedFiles;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCamera : MonoBehaviour
{
    const int Width = 640, Height = 480;
    const int FrameBytes = Width * Height * 3;
    const string MapName = "magehand_video_unity";

    RawImage image;
    Texture2D texture;
    byte[] buffer;

    MemoryMappedFile map;
    MemoryMappedViewStream stream;

    float nextFrame, nextConnect;

    void Awake()
    {
        image = GetComponent<RawImage>();
        if (image == null)
        {
            enabled = false;
            return;
        }

        buffer = new byte[FrameBytes];
        texture = new Texture2D(Width, Height, TextureFormat.RGB24, false);
        texture.name = "DisplayCamera_Webcam";

        image.texture = texture;
        image.color = Color.black;
    }

    void Update()
    {
        float now = Time.unscaledTime;
        if (now < nextFrame) return;
        nextFrame = now + 1f / 30f;

        if (stream == null)
        {
            if (now < nextConnect) return;
            nextConnect = now + 1f;

            try
            {
                map = MemoryMappedFile.OpenExisting(MapName);
                stream = map.CreateViewStream(
                    0, FrameBytes, MemoryMappedFileAccess.Read);
            }
            catch (IOException)
            {
                CloseMap();
                return;
            }
        }

        try
        {
            stream.Position = 0;

            int offset = 0;
            while (offset < FrameBytes)
            {
                int count = stream.Read(
                    buffer, offset, FrameBytes - offset);

                if (count == 0)
                {
                    CloseMap();
                    return;
                }

                offset += count;
            }

            texture.LoadRawTextureData(buffer);
            texture.Apply(false, false);
            image.color = Color.white;
        }
        catch (IOException)
        {
            CloseMap();
            nextConnect = now + 1f;
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

        if (image != null && image.texture == texture)
            image.texture = null;

        if (texture != null)
            Destroy(texture);
    }
}