using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whisper.Utils;
using Whisper;

public class DemoSTT : MonoBehaviour
{
    MoveHandPoints handController;
    public MicrophoneRecord microphoneRecord;
    public WhisperManager whisper;
    public bool streamSegments = true;
    public bool printLanguage = true;

    private string _buffer;

    // Start is called before the first frame update
    void Start()
    {
    handController = this.GetComponent<MoveHandPoints>();   
    }

    // Update is called once per frame
    void Update()
    {
        print(handController.gesture);
    }
}

