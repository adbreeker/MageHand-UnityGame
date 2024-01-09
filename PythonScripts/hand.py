from multiprocessing.shared_memory import SharedMemory

import os
import sys
import itertools
import concurrent.futures

import numpy as np
import mediapipe as mp
import cv2
import torch
import torch.nn as nn
import torch.nn.functional as F

base_path = getattr(sys, '_MEIPASS', os.path.dirname(os.path.abspath(__file__)))
task_file_path = os.path.join(base_path, 'gesture_recognizer.task')
torch_file_path = os.path.join(base_path, 'hand_classifier6.pth')


class HandDetector:
    def __init__(self):
        self.mp = mp
        self.cap = None
        self.landmarker = None
        self.recognizer = None

        self.base_options = mp.tasks.BaseOptions
        self.hand_landmarker = mp.tasks.vision.GestureRecognizer
        self.hand_landmarker_options = mp.tasks.vision.GestureRecognizerOptions
        self.hand_landmarker_result = mp.tasks.vision.GestureRecognizerResult
        self.vision_running_mode = mp.tasks.vision.RunningMode

        self.data = np.zeros((21, 3), dtype=np.float32)
        self.gesture = 'None'
        self.frame_shape = frame_shape = (480, 640, 3)

        self.shared_mem_video = SharedMemory(name='magehand_video_unity', create=True,
                                             size=frame_shape[0] * frame_shape[1] * frame_shape[2])
        self.shared_mem_gestures = SharedMemory(name='magehand_gestures', create=True,
                                                size=12)
        self.shared_mem_points = SharedMemory(name='magehand_points', create=True,
                                              size=700)

        self.eps = 0.013
        ##["Open palm", "Closed palm", "Ok", "Finger", "Claw", "Thumb Up", "Thumb Down"]
        self.hand_labels = ["Open_palm", "Closed_Fist", "ILoveYou", "Pointing_Up", "Victory", "Thumb_Up", "Thumb_Down"]
        self.thread_pool = concurrent.futures.ThreadPoolExecutor(max_workers=1)
        self.model_thread = None
        self.model_args = None
        self.model_busy = False
        self.image = None

    def run(self):
        if self.landmarker is None:
            self._initialize()
        while self.cap.isOpened():
            success, image = self.cap.read()

            if image.shape == (1080, 1920, 3):
                image = cv2.resize(image, (640, 480))
            
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

            self.image = image

            self.landmarker.recognize_async(
                mp_image, timestamp_ms=int(timestamp_ms))

            points_hand = ';'.join([f'{x:.5f},{y:.5f},{z:.5f}' for x, y, z in self.data]) + ';' + (
                        'a' * (700 - len(';'.join([f'{x:.5f},{y:.5f},{z:.5f}' for x, y, z in self.data])) - 1))

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

    def _callback(self, result, timestamp_ms: int, image):
        if result.hand_landmarks:
            # self.gesture = result.gestures[0][0].category_name + ';' + ('a' * (12 - len(result.gestures[0][0].category_name)-1))

            for i in range(21):
                """
                This calculates the difference between last updated hand position points and their current position 
                points. If the absolute value of the difference is greater than the epsilon, then the position is 
                adjusted. 
                """
                x_diff = result.hand_landmarks[0][i].x - self.data[i][0]
                y_diff = result.hand_landmarks[0][i].y - self.data[i][1]

                abs_diff = (x_diff**2 + y_diff**2)**0.5

                if abs_diff > self.eps:
                    self.data[i][0] += (x_diff - self.eps) if x_diff > self.eps else (
                                x_diff + self.eps) if -x_diff > self.eps else 0
                    self.data[i][1] += (y_diff - self.eps) if y_diff > self.eps else (
                                y_diff + self.eps) if -y_diff > self.eps else 0

            self._torch_call()

    def _torch_call(self):
        pre_processed = self.pre_process_landmark()
        self.gesture = self.hand_labels[self.recognizer(pre_processed)] + ';' + \
                       ('a' * (12 - len(self.hand_labels[self.recognizer(pre_processed)]) - 1))

    def _initialize(self):
        self.cap = cv2.VideoCapture(0)
        self.recognizer = KeyPointClassifier()

        self.options = self.hand_landmarker_options(
            base_options=self.base_options(
                model_asset_path=task_file_path),
            running_mode=self.vision_running_mode.LIVE_STREAM,
            min_hand_detection_confidence= 0.5,
            min_hand_presence_confidence= 0.5,
            min_tracking_confidence=0.5,
            result_callback=self._callback)

        self.landmarker = self.hand_landmarker.create_from_options(
            self.options)

    def pre_process_landmark(self):
        image_width, image_height = self.image.shape[1], self.image.shape[0]

        temp_landmark_list = []

        for coordinate in self.data:
            x, y, z = coordinate
            landmark_x = min(int(x * image_width), image_width - 1)
            landmark_y = min(int(y * image_height), image_height - 1)

            temp_landmark_list.append([landmark_x, landmark_y])

        base_x, base_y = 0, 0
        for index, landmark_point in enumerate(temp_landmark_list):
            if index == 0:
                base_x, base_y = landmark_point[0], landmark_point[1]

            temp_landmark_list[index][0] = temp_landmark_list[index][0] - base_x
            temp_landmark_list[index][1] = temp_landmark_list[index][1] - base_y

        temp_landmark_list = list(
            itertools.chain.from_iterable(temp_landmark_list))

        max_value = max(list(map(abs, temp_landmark_list)))

        def normalize_(n):
            return n / max_value

        temp_landmark_list = list(map(normalize_, temp_landmark_list))

        return temp_landmark_list


class KeyPointClassifier(object):
    def __init__(self, model_path=torch_file_path):
        self.device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
        checkpoint = torch.load(model_path, map_location=self.device)
        self.model = HandClassifier()
        self.model.load_state_dict(checkpoint.state_dict())
        self.model.to(self.device)
        self.model.eval()

    def __call__(self, landmark_list):
        input_tensor = torch.tensor([landmark_list], dtype=torch.float32)

        with torch.no_grad():
            output = self.model(input_tensor)

        probabilities = F.softmax(output, dim=1)

        confidence, predicted_class = torch.max(probabilities, 1)

        result_index = np.argmax(np.squeeze(output.numpy()))

        if confidence > 0.8:
            return result_index
        else:
            return 0


class HandClassifier(nn.Module):
    def __init__(self):
        super(HandClassifier, self).__init__()
        self.dropout1 = nn.Dropout(0.2)
        self.fc1 = nn.Linear(21 * 2, 20)
        self.dropout2 = nn.Dropout(0.4)
        self.fc2 = nn.Linear(20, 10)
        self.fc3 = nn.Linear(10, 7)

    def forward(self, x):
        x = self.dropout1(x)
        x = torch.relu(self.fc1(x))
        x = self.dropout2(x)
        x = torch.relu(self.fc2(x))
        x = self.fc3(x)
        return x


if __name__ == "__main__":
    detector = HandDetector()
    detector.run()
