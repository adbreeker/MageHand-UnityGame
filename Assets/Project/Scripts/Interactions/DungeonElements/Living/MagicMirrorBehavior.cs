using System.IO.MemoryMappedFiles;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MagicMirrorBehavior : MonoBehaviour
{
    const int Width = 640;
    const int Height = 480;
    const int FrameBytes = Width * Height * 3;
    const string MapName = "magehand_video_unity";

    [SerializeField] RawImage _mirrorImage;
    [SerializeField] GameObject _transitionEffect;

    //image showing
    Texture2D _defaultTexture;
    Texture2D _playerTexture;
    byte[] _frameBuffer;
    MemoryMappedFile _map;
    MemoryMappedViewStream _stream;
    GameObject _transitionInstance;
    bool _showPlayer = false;
    float _minTransparency = 0.05f;
    float _maxTransparency = 0.96f;

    private void Start()
    {
        _defaultTexture = (Texture2D)_mirrorImage.texture;
        _playerTexture = new Texture2D(Width, Height, TextureFormat.RGB24, false);
        _frameBuffer = new byte[FrameBytes];
    }

    private void FixedUpdate()
    {
        if(_showPlayer)
        {
            if (_stream == null)
            {
                try
                {
                    _map = MemoryMappedFile.OpenExisting(MapName);
                    _stream = _map.CreateViewStream(0, FrameBytes, MemoryMappedFileAccess.Read);
                }
                catch (IOException)
                {
                    CloseMap();
                    _mirrorImage.texture = _defaultTexture;
                    return;
                }
            }

            try
            {
                _stream.Position = 0;
                int offset = 0;
                while (offset < _frameBuffer.Length)
                {
                    int count = _stream.Read(_frameBuffer, offset, _frameBuffer.Length - offset);
                    if (count == 0)
                    {
                        CloseMap();
                        _mirrorImage.texture = _defaultTexture;
                        return;
                    }

                    offset += count;
                }

                _playerTexture.LoadRawTextureData(_frameBuffer);
                _playerTexture.Apply(false, false);
                _mirrorImage.texture = _playerTexture;
            }
            catch (IOException)
            {
                CloseMap();
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
        _transitionInstance = Instantiate(_transitionEffect, transform);

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

        if (_transitionInstance != null)
        {
            Destroy(_transitionInstance);
            _transitionInstance = null;
        }
    }

    void CloseMap()
    {
        _stream?.Dispose();
        _stream = null;

        _map?.Dispose();
        _map = null;
    }

    private void OnDestroy()
    {
        CloseMap();

        if (_transitionInstance != null)
        {
            Destroy(_transitionInstance);
            _transitionInstance = null;
        }

        if (_mirrorImage != null && _mirrorImage.texture == _playerTexture)
            _mirrorImage.texture = _defaultTexture;

        if (_playerTexture != null)
            Destroy(_playerTexture);
    }
}
