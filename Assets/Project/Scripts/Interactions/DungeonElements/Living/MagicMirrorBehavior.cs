using System.IO.MemoryMappedFiles;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MagicMirrorBehavior : MonoBehaviour
{
    [SerializeField] RawImage _mirrorImage;
    [SerializeField] GameObject _transitionEffect;

    //image showing
    Texture2D _defaultTexture;
    bool _showPlayer = false;
    float _minTransparency = 0.05f;
    float _maxTransparency = 0.96f;

    private void Start()
    {
        _defaultTexture = (Texture2D)_mirrorImage.texture;
    }

    private void FixedUpdate()
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
        StartCoroutine(SwitchImage(showPlayer));
    }

    IEnumerator SwitchImage(bool showPlayer)
    {
        Color newColor = _mirrorImage.color;
        WaitForFixedUpdate deley = new WaitForFixedUpdate();
        Instantiate(_transitionEffect, transform);

        while(newColor.a > _minTransparency)
        {
            newColor.a -= Time.fixedDeltaTime;
            if(newColor.a < _minTransparency) { newColor.a = _minTransparency; }
            _mirrorImage.color = newColor;
            yield return deley;
        }

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

        while (newColor.a < _maxTransparency)
        {
            newColor.a += Time.fixedDeltaTime;
            if (newColor.a > _maxTransparency) { newColor.a = _maxTransparency; }
            _mirrorImage.color = newColor;
            yield return deley;
        }
    }
}
