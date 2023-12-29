from multiprocessing.shared_memory import SharedMemory

import os
import sys
import itertools

import numpy as np
import mediapipe as mp
import cv2
import tensorflow as tf

base_path = getattr(sys, '_MEIPASS', os.path.dirname(os.path.abspath(__file__)))
task_file_path = os.path.join(base_path, 'hand_classifier.tflite')


class HandDetector:
    def __init__(self):
        self.mp = mp
        self.cap = None
        self.landmarker = None
        self.recognizer = None

        self.data = np.zeros((21, 3), dtype=np.float32)
        self.gesture = 'None'
        self.frame_shape = frame_shape = (480, 640, 3)

        self.shared_mem_video = SharedMemory(name='video_unity', create=True,
                                             size=frame_shape[0] * frame_shape[1] * frame_shape[2])
        self.shared_mem_gestures = SharedMemory(name='gestures', create=True,
                                                size=12)
        self.shared_mem_points = SharedMemory(name='points', create=True,
                                              size=700)

        self.eps = 0.013
        ##["Open palm", "Closed palm", "Ok", "Finger", "Claw", "Thumb Up", "Thumb Down"]
        self.hand_labels = ["Open_palm", "Closed_Fist", "ILoveYou", "Pointing_Up", "Victory", "Thumb_Up", "Thumb_Down"]

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

            image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)

            image.flags.writeable = False
            self._callback(self.landmarker.process(image), timestamp_ms=int(timestamp_ms), image=image)
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
        if result.multi_hand_landmarks:
            # self.gesture = result.gestures[0][0].category_name + ';' + ('a' * (12 - len(result.gestures[0][0].category_name)-1))

            for i in range(21):
                """
                This calculates the difference between last updated hand position points and their current position 
                points. If the absolute value of the difference is greater than the epsilon, then the position is 
                adjusted. 
                """
                landmark = result.multi_hand_landmarks[0].landmark[i]
                x_diff = landmark.x - self.data[i][0]
                y_diff = landmark.y - self.data[i][1]
                abs_diff = (x_diff ** 2 + y_diff ** 2) ** 0.5

                if abs_diff > self.eps:
                    self.data[i][0] += (x_diff - self.eps) if x_diff > self.eps else (
                            x_diff + self.eps) if -x_diff > self.eps else 0
                    self.data[i][1] += (y_diff - self.eps) if y_diff > self.eps else (
                            y_diff + self.eps) if -y_diff > self.eps else 0

                    self.data[i][2] = landmark.z

            for hand_landmarks, handedness in zip(result.multi_hand_landmarks,
                                                  result.multi_handedness):
                pre_processed = self.pre_process_landmark(hand_landmarks, image)
                self.gesture = self.hand_labels[self.recognizer(pre_processed)] + ';' + \
                               ('a' * (12 - len(self.hand_labels[self.recognizer(pre_processed)]) - 1))
                print(self.gesture)

    def _initialize(self):
        self.cap = cv2.VideoCapture(0)
        self.recognizer = KeyPointClassifier()

        mp_hands = mp.solutions.hands

        self.landmarker = mp_hands.Hands(
            static_image_mode=True,
            max_num_hands=1,
            min_detection_confidence=0.7,
            min_tracking_confidence=0.5
        )

        print('connected to unity')

    def pre_process_landmark(self, landmarks, image):
        image_width, image_height = image.shape[1], image.shape[0]

        temp_landmark_list = []

        for _, landmark in enumerate(landmarks.landmark):
            landmark_x = min(int(landmark.x * image_width), image_width - 1)
            landmark_y = min(int(landmark.y * image_height), image_height - 1)

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
    def __init__(
            self,
            model_path=task_file_path,
            num_threads=1,
    ):
        self.interpreter = tf.lite.Interpreter(model_path=model_path, num_threads=num_threads)
        self.interpreter.allocate_tensors()
        self.input_details = self.interpreter.get_input_details()
        self.output_details = self.interpreter.get_output_details()

    def __call__(self, landmark_list):
        input_tensor_index = self.input_details[0]['index']
        self.interpreter.set_tensor(input_tensor_index, np.array([landmark_list], dtype=np.float32))
        self.interpreter.invoke()

        output_tensor_index = self.output_details[0]['index']
        output = self.interpreter.tensor(output_tensor_index)()

        result_index = np.argmax(np.squeeze(output))

        return result_index


if __name__ == "__main__":
    detector = HandDetector()
    detector.run()
