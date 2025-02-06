using System.IO.MemoryMappedFiles;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MagicMirrorBehavior : MonoBehaviour
{
    [SerializeField] RawImage _mirrorImage;

    //image showing
    Texture2D _defaultTexture;
    bool _showPlayer = false;

    private void Start()
    {
        _defaultTexture = (Texture2D)_mirrorImage.texture;
        ShowPlayerImage(true);
    }

    private void Update()
    {
        if(_showPlayer)
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

                        _mirrorImage.texture = texture;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                _mirrorImage.texture = _defaultTexture;
            }
        }
    }

    public void ShowPlayerImage(bool showPlayer)
    {
        if(showPlayer) 
        {
            _showPlayer = true;
            _mirrorImage.transform.localRotation = Quaternion.Euler(0, 0, 180);
        }
        else
        {
            _showPlayer = false;
            _mirrorImage.texture = _defaultTexture;
            _mirrorImage.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
