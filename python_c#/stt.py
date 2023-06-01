import pyaudio
import soundfile as sf
import numpy as np
from faster_whisper import WhisperModel


RATE = 44100
CHUNK = 1024
RECORD_SECONDS = 5
OUTPUT_FILE = "recording.wav"


audio = pyaudio.PyAudio()

stream = audio.open(format=pyaudio.paInt16,
                    channels=1,
                    rate=RATE,
                    input=True,
                    frames_per_buffer=CHUNK)

print("Recording started...")

frames = []
for _ in range(0, int(RATE / CHUNK * RECORD_SECONDS)):
    data = stream.read(CHUNK)
    frames.append(data)

print("Recording stopped.")

stream.stop_stream()
stream.close()
audio.terminate()

# Convert the frames to a numpy array with int16 data type
frames_np = np.frombuffer(b''.join(frames), dtype=np.int16)

# Save the recorded audio as a WAV file
sf.write(OUTPUT_FILE, frames_np, RATE)

model_size = "tiny"

model = WhisperModel(model_size, device="cpu", compute_type="int8")

segments, info = model.transcribe(OUTPUT_FILE, beam_size=5)

print("Detected language '%s' with probability %f" % (info.language, info.language_probability))

for segment in segments:
    print("[%.2fs -> %.2fs] %s" % (segment.start, segment.end, segment.text))


