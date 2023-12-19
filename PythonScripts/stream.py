import re
import os
import sys

import numpy as np
import pyaudio
from faster_whisper import WhisperModel
from multiprocessing.shared_memory import SharedMemory


STEP_IN_SEC: int = 2
LENGHT_IN_SEC: int = 2    
NB_CHANNELS = 1
RATE = 16000
CHUNK = RATE

WHISPER_LANGUAGE = "en"
WHISPER_THREADS = 2

running: str = 'False'

base_path = getattr(sys, '_MEIPASS', os.path.dirname(os.path.abspath(__file__)))
task_file_path = os.path.join(base_path, 'models')
  
whisper = WhisperModel("tiny", device="cpu", compute_type="int8", cpu_threads=WHISPER_THREADS, download_root=task_file_path, local_files_only=True)

shared_mem_gestures = SharedMemory(name='gestures', create=False, size=12)
shared_mem_whsiper = SharedMemory(name='whisper', create=True, size=15)

running = 'None' + ';' + ('a' * (9 - len('None')))
shared_mem_whsiper.buf[:len(running)] = bytearray(running.encode('utf-8'))

audio = pyaudio.PyAudio()
stream = audio.open(
    format=pyaudio.paInt16,
    channels=NB_CHANNELS,
    rate=RATE,
    input=True,
    frames_per_buffer=CHUNK,    
)

while True:
    if shared_mem_gestures.buf[0:7].tobytes().decode('utf-8') == "Victory":
        audio_data = b""
        for _ in range(STEP_IN_SEC):
            chunk = stream.read(RATE)    
            audio_data += chunk

        audio_data_array: np.ndarray = np.frombuffer(audio_data, np.int16).astype(np.float32) / 255.0

        segments, _ = whisper.transcribe(audio_data_array,
                                        language=WHISPER_LANGUAGE,
                                        temperature=0.2,
                                        beam_size=10,
                                        vad_filter=True,
                                        vad_parameters=dict(min_silence_duration_ms=1000),
                                        initial_prompt=[2764,1572,12037,847],
                                        suppress_tokens=[32825, 19657, 4092, 25900, 4621, 6650])
        
        segments = [s.text for s in segments]
        transcription = re.sub(r"[^a-zA-Z0-9\s]", "", "".join(segments)).lower()
        
        if len(transcription) != 0 and len(transcription) <= 9 and transcription != '':
            transcription = transcription + ';' + ('a' * (9 - len(transcription)))
            shared_mem_whsiper.buf[:len(transcription)] = bytearray(transcription.encode('utf-8'))
        elif len(transcription) > 10 and transcription != '':
            transcription = transcription[:10] + ';'
            shared_mem_whsiper.buf[:10] = bytearray(transcription[:10].encode('utf-8'))

        print(transcription)
        print()
        
        
    else:
        continue
    
