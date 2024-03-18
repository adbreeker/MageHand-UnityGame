using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCamera : MonoBehaviour //displaying web camera on debug canvas
{
    RawImage camTexture;

    private void Awake()
    {
        camTexture = GetComponent<RawImage>();
    }
    private void Update()
    {
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

                    camTexture.color = Color.white;
                    camTexture.texture = texture;
                }
            }
        }
        catch (FileNotFoundException)
        {
            camTexture.color = Color.black;
            Debug.Log("video_unity doesn't exist");
        }
    }
}
