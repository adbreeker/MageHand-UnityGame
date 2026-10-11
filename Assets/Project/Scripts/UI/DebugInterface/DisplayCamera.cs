
using System.IO;
using System.IO.MemoryMappedFiles;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCamera : MonoBehaviour
{
    private const int Width = 640;
    private const int Height = 480;
    private const int FrameBytes = Width * Height * 3;
    private const string MapName = "magehand_video_unity";

    private RawImage camTexture;
    private Texture2D videoTexture;
    private byte[] frameBuffer;
    private bool missingLogged;

    private void Awake()
    {
        camTexture = GetComponent<RawImage>();

        if (camTexture == null)
        {
            Debug.LogError("DisplayCamera requires a RawImage.", this);
            enabled = false;
            return;
        }

        // Allocate the frame buffer only once.
        frameBuffer = new byte[FrameBytes];

        // Create exactly one texture and reuse it for every frame.
        videoTexture = new Texture2D(
            Width,
            Height,
            TextureFormat.RGB24,
            false
        );

        videoTexture.name = "DisplayCamera_Webcam";
        camTexture.texture = videoTexture;
    }

    private void Update()
    {
        try
        {
            using (MemoryMappedFile mmf =
                   MemoryMappedFile.OpenExisting(MapName))
            using (MemoryMappedViewStream stream =
                   mmf.CreateViewStream(
                       0,
                       FrameBytes,
                       MemoryMappedFileAccess.Read))
            {
                // Read into the existing buffer instead of allocating
                // a new byte[] every frame.
                if (!ReadFrame(stream, frameBuffer))
                    return;
            }

            // Update the existing Texture2D.
            videoTexture.LoadRawTextureData(frameBuffer);
            videoTexture.Apply(false, false);

            if (camTexture.color != Color.white)
                camTexture.color = Color.white;

            missingLogged = false;
        }
        catch (FileNotFoundException)
        {
            camTexture.color = Color.black;

            // Avoid logging the same error on every frame.
            if (!missingLogged)
            {
                Debug.LogWarning(
                    "Shared memory 'magehand_video_unity' does not exist."
                );
                missingLogged = true;
            }
        }
    }

    private static bool ReadFrame(
        Stream stream,
        byte[] buffer)
    {
        int offset = 0;

        while (offset < buffer.Length)
        {
            int count = stream.Read(
                buffer,
                offset,
                buffer.Length - offset
            );

            if (count <= 0)
                return false;

            offset += count;
        }

        return true;
    }

    private void OnDestroy()
    {
        if (camTexture != null &&
            camTexture.texture == videoTexture)
        {
            camTexture.texture = null;
        }

        if (videoTexture != null)
        {
            Destroy(videoTexture);
            videoTexture = null;
        }
    }
}