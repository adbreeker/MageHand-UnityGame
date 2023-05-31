import mediapipe as mp
import cv2
import socket
import numpy as np



class HandLandmarkerDetector:
    '''
    To run it with unity
    1. Run the unity game (button start should be blue)
    2. Run this script
    3. Wait for 'connected to unity' message
    '''
    def __init__(self):
        self.mp = mp
        self.cap = None
        self.landmarker = None
        self.options = None

        self.host = "127.0.0.1"
        self.port = 25001
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

        self.base_options = mp.tasks.BaseOptions
        self.hand_landmarker = mp.tasks.vision.HandLandmarker
        self.hand_landmarker_options = mp.tasks.vision.HandLandmarkerOptions
        self.hand_landmarker_result = mp.tasks.vision.HandLandmarkerResult
        self.vision_running_mode = mp.tasks.vision.RunningMode

        self.x = np.zeros(21, dtype=np.float32)
        self.y = np.zeros(21, dtype=np.float32)
        self.z = np.zeros(21, dtype=np.float32)

    def run(self, debug: bool = False):
        '''
        debug: if True, it will show the camera feed and the point
        '''
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
            self.landmarker.detect_async(
                mp_image, timestamp_ms=int(timestamp_ms))
            
            posString = ';'.join([f'{x:.5f},{y:.5f},{z:.5f}' for x, y, z in zip(self.x, self.y, self.z)])

            self.sock.sendall(posString.encode("UTF-8"))
            
            # NOTE: this is to visualize the point
            if debug:
                cv2.imshow('MediaPipe Hands', cv2.flip(image, 1))
            if cv2.waitKey(5) & 0xFF == 27:
                break

        self.cap.release()
        self.cv2.destroyAllWindows()

    def _callback(self, result, output_image: mp.Image, timestamp_ms: int):
        if result.hand_landmarks != []:  
            for i in range(21):
                self.x[i] = result.hand_landmarks[0][i].x
                self.y[i] = result.hand_landmarks[0][i].y
                self.z[i] = result.hand_landmarks[0][i].z

    def _initialize(self):
        self.cap = cv2.VideoCapture(0)

        self.options = self.hand_landmarker_options(
            base_options=self.base_options(
                model_asset_path='hand_landmarker.task'),
            running_mode=self.vision_running_mode.LIVE_STREAM,
            result_callback=self._callback)

        self.landmarker = self.hand_landmarker.create_from_options(
            self.options)

        self.sock.connect((self.host, self.port))
        print('connected to unity')

if __name__ == '__main__':
    detector = HandLandmarkerDetector()
    detector.run(debug=True)
