from multiprocessing.shared_memory import SharedMemory
import os
import sys

import numpy as np
import mediapipe as mp
import cv2

base_path = getattr(sys, '_MEIPASS', os.path.dirname(os.path.abspath(__file__)))
task_file_path = os.path.join(base_path, 'gesture_recognizer.task')


class HandDetector:
    def __init__(self):
        self.mp = mp
        self.cap = None
        self.landmarker = None
        self.options = None

        self.base_options = mp.tasks.BaseOptions
        self.hand_landmarker = mp.tasks.vision.GestureRecognizer
        self.hand_landmarker_options = mp.tasks.vision.GestureRecognizerOptions
        self.hand_landmarker_result = mp.tasks.vision.GestureRecognizerResult
        self.vision_running_mode = mp.tasks.vision.RunningMode

        self.data = np.zeros((21, 3), dtype=np.float32)
        self.gesture = 'None'
        self.frame_shape = frame_shape = (480, 640, 3)  

        self.shared_mem_video = SharedMemory(name='video_unity', create=True, 
                                   size=frame_shape[0] * frame_shape[1] * frame_shape[2])
        self.shared_mem_gestures = SharedMemory(name='gestures', create=True,
                                      size=12)
        self.shared_mem_points = SharedMemory(name='points', create=True,
                                      size=530)
        
        self.eps = 0.013
        
    def run(self):
        if self.landmarker is None:
            self._initialize()
        while self.cap.isOpened():
            success, image = self.cap.read()

            np.copyto(np.frombuffer(self.shared_mem_video.buf, 
                                    dtype=np.uint8).reshape(self.frame_shape), 
                                    cv2.cvtColor(image, cv2.COLOR_BGR2RGB))
            
            if not success:
                print("Ignoring empty camera frame.")
                continue

            timestamp_ms = self.cap.get(cv2.CAP_PROP_POS_MSEC)

            image.flags.writeable = False
            
            mp_image = self.mp.Image(
                image_format=self.mp.ImageFormat.SRGB, data=image)
            
            self.landmarker.recognize_async(
                mp_image, timestamp_ms=int(timestamp_ms))
            
            points_hand = ';'.join([f'{x:.5f},{y:.5f},{z:.5f}' for x, y, z in self.data]) + ';' + ('a' * (530 - len(';'.join([f'{x:.5f},{y:.5f},{z:.5f}' for x, y, z in self.data]))-1))
            self.shared_mem_points.buf[:len(points_hand)] = bytearray(points_hand.encode('utf-8'))
            self.shared_mem_gestures.buf[:len(self.gesture)] = bytearray(self.gesture.encode('utf-8'))
        
            if cv2.waitKey(5) & 0xFF == 27:
                break

        self.cap.release()
        cv2.destroyAllWindows()

        self.shared_mem_video.close()
        self.shared_mem_points.close()
        self.shared_mem_gestures.close()

        self.shared_mem_video.unlink()
        self.shared_mem_points.unlink()
        self.shared_mem_gestures.unlink()

    def _callback(self, result, output_image: mp.Image, timestamp_ms: int):        
        if result.hand_landmarks:
            self.gesture = result.gestures[0][0].category_name + ';' + ('a' * (12 - len(result.gestures[0][0].category_name)-1))
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
                #model_asset_path='Assets\StreamingAssets\Mediapipe\gesture_recognizer.task'),
                model_asset_path=task_file_path),
            running_mode=self.vision_running_mode.LIVE_STREAM,
            min_hand_detection_confidence= 0.5,
            min_hand_presence_confidence= 0.5,
            min_tracking_confidence=0.5,
            result_callback=self._callback)

        self.landmarker = self.hand_landmarker.create_from_options(
            self.options)
        
        print('connected to unity')

if __name__ == "__main__":    
    detector = HandDetector()
    detector.run()