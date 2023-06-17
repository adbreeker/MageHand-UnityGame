using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whisper.Utils;
using Whisper;
using System.Diagnostics;


public class DemoSTT : MonoBehaviour
{
    MoveHandPoints handController;
    public MicrophoneRecord microphoneRecord;
    public WhisperManager whisper;

    int cooldown = 0;

    // Start is called before the first frame update
    void Start()
    {
        handController = this.GetComponent<MoveHandPoints>();
        whisper.InitModel();
        print(whisper.IsLoading);
    }

    // Update is called once per frame
    void Update()
    {

        if (handController.gesture == "Victory" && cooldown == 0)
        {
            if (!microphoneRecord.IsRecording)
                microphoneRecord.StartRecord();
            cooldown = 1000;
        }

        if (cooldown > 0)
        {
            cooldown--;
        }

    }

    private void Awake()
        {
            microphoneRecord.OnRecordStop += Transcribe;
        }

    private async void Transcribe(float[] data, int frequency, int channels, float length)
        {   
            var sw = new Stopwatch();
            sw.Start();
            
            var res = await whisper.GetTextAsync(data, frequency, channels);

            var time = sw.ElapsedMilliseconds;
            var rate = length / (time * 0.001f);
            print($"Time: {time} ms\nRate: {rate:F1}x");
            if (res == null)
                return;

            var text = res.Result;
            print(text);
        }
}

