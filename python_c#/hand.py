import mediapipe as mp
import cv2
import socket
import numpy as np
import pyaudio
import soundfile as sf

from faster_whisper import WhisperModel
from collections import deque


class SpeechToText:
    def __init__(self):
        self.RATE = 44100
        self.CHUNK = 1024
        self.RECORD_SECONDS = 1
        self.OUTPUT_FILE = "recording.wav"
        self.model_size = "tiny"
        self.audio = pyaudio.PyAudio()

    def speech(self):
        stream = self.audio.open(format=pyaudio.paInt16,
                    channels=1,
                    rate=self.RATE,
                    input=True,
                    frames_per_buffer=self.CHUNK)

        print("Recording started...")

        frames = []
        for _ in range(0, int(self.RATE / self.CHUNK * self.RECORD_SECONDS)):
            data = stream.read(self.CHUNK)
            frames.append(data)

        print("Recording stopped.")

        stream.stop_stream()
        stream.close()
        self.audio.terminate()

        frames_np = np.frombuffer(b''.join(frames), dtype=np.int16)        
        sf.write(self.OUTPUT_FILE, frames_np, self.RATE)

    def text(self):
        model = WhisperModel(self.model_size, device="cpu", compute_type="int8")
        segments, _ = model.transcribe(self.OUTPUT_FILE, beam_size=5)

        for segment in segments:
            print("[%.2fs -> %.2fs] %s" % (segment.start, segment.end, segment.text))

                
class HandLandmarkerDetector:
    def __init__(self):
        self.mp = mp
        self.cap = None
        self.landmarker = None
        self.options = None

        self.host = "127.0.0.1"
        self.port = 25001
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

        self.base_options = mp.tasks.BaseOptions
        self.hand_landmarker = mp.tasks.vision.GestureRecognizer
        self.hand_landmarker_options = mp.tasks.vision.GestureRecognizerOptions
        self.hand_landmarker_result = mp.tasks.vision.GestureRecognizerResult
        self.vision_running_mode = mp.tasks.vision.RunningMode

        self.data = np.zeros((21, 3), dtype=np.float32)
        self.gesture = ''
        self.prev_gesture = deque(maxlen=2)

        #self.stt = SpeechToText()

    def run(self, debug: bool = False):
        if self.landmarker is None:
            self._initialize()

        while self.cap.isOpened():
            success, image = self.cap.read()
            
            if not success:
                print("Ignoring empty camera frame.")
                continue

            timestamp_ms = self.cap.get(cv2.CAP_PROP_POS_MSEC)
            image.flags.writeable = False
            mp_image = self.mp.Image(
                image_format=self.mp.ImageFormat.SRGB, data=image)
            self.landmarker.recognize_async(
                mp_image, timestamp_ms=int(timestamp_ms))
            self.prev_gesture.append(self.gesture)

            # if self.gesture == 'Thumb_Up' and (self.prev_gesture[0] != self.prev_gesture[1]):
            #     self.stt.speech()
            #     self.stt.text()
            #     continue
            self.sock.sendto(str.encode(';'.join([f'{x:.5f},{y:.5f},{z:.5f}' for x, y, z in self.data])), (self.host, self.port))
        
            # NOTE: this is to visualize the point
            if debug:
                cv2.imshow('MediaPipe Hands', cv2.flip(image, 1))
            if cv2.waitKey(5) & 0xFF == 27:
                break

        self.cap.release()
        cv2.destroyAllWindows()

    def _callback(self, result, output_image: mp.Image, timestamp_ms: int):        
        if result.hand_landmarks != []:
            self.gesture = result.gestures[0][0].category_name
            for i in range(21):
                self.data[i] = [result.hand_landmarks[0][i].x, 
                                result.hand_landmarks[0][i].y, 
                                result.hand_landmarks[0][i].z]
    def _initialize(self):
        self.cap = cv2.VideoCapture(0)

        self.options = self.hand_landmarker_options(
            base_options=self.base_options(
                model_asset_path='gesture_recognizer.task'),
            running_mode=self.vision_running_mode.LIVE_STREAM,
            result_callback=self._callback)

        self.landmarker = self.hand_landmarker.create_from_options(
            self.options)
        
        print('connected to unity')

if __name__ == '__main__':
    detector = HandLandmarkerDetector()
    detector.run(debug=True)
