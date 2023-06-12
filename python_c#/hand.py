import mediapipe as mp
import cv2
import socket
import numpy as np

                
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
        self.gesture = None

        self.eps = 0.013  # Lower values make the hand more "stable", but hand points movement can look artificial

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
            
            self.sock.sendto(str.encode(f'{self.gesture};'+';'.join([f'{x:.5f},{y:.5f},{z:.5f}' for x, y, z in self.data])), (self.host, self.port))
        
            # NOTE: this is to visualize the point
            if debug:
                # for x, y, z in self.data:
                #    center = (int(x * image.shape[1]), int(y * image.shape[0]))
                #    cv2.circle(image, center, 5, (0, 0, 255), -1)
                cv2.imshow('MediaPipe Hands', cv2.flip(image, 1))
            if cv2.waitKey(5) & 0xFF == 27:
                break

        self.cap.release()
        cv2.destroyAllWindows()

    def _callback(self, result, output_image: mp.Image, timestamp_ms: int):        
        if result.hand_landmarks:
            self.gesture = result.gestures[0][0].category_name
            for i in range(21):
                """
                This calculates the difference between last updated hand position points and their current position 
                points. If the absolute value of the difference is greater than the epsilon, then the position is 
                adjusted. 
                """
                x_diff = result.hand_landmarks[0][i].x - self.data[i][0]
                y_diff = result.hand_landmarks[0][i].y - self.data[i][1]

                self.data[i][0] += (x_diff - self.eps) if x_diff > self.eps else (
                            x_diff + self.eps) if -x_diff > self.eps else 0
                self.data[i][1] += (y_diff - self.eps) if y_diff > self.eps else (
                            y_diff + self.eps) if -y_diff > self.eps else 0

                self.data[i][2] = result.hand_landmarks[0][i].z


    def _initialize(self):
        self.cap = cv2.VideoCapture(0)
        
        self.options = self.hand_landmarker_options(
            base_options=self.base_options(
                model_asset_path='gesture_recognizer.task'),
            running_mode=self.vision_running_mode.LIVE_STREAM,
            min_hand_detection_confidence= 0.5,
            min_hand_presence_confidence= 0.5,
            min_tracking_confidence=0.5,
            result_callback=self._callback)

        self.landmarker = self.hand_landmarker.create_from_options(
            self.options)
        
        print('connected to unity')


if __name__ == '__main__':
    detector = HandLandmarkerDetector()
    detector.run(debug=True)
